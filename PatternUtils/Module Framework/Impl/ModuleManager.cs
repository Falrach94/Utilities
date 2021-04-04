using PatternUtils.Dependency_Graph;
using PatternUtils.Module_Framework.Data;
using PatternUtils.Module_Framework.Impl;
using PatternUtils.Module_Framework.Interfaces;
using SyncUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PatternUtils.Module_Framework
{
    public class ModuleManager : IModuleManager
    {
        private readonly SemaphoreLock _lock = new();

        private readonly Dictionary<Type, IManagerInterface> _managerInterfaceDic = new();
        private readonly Dictionary<Type, Tuple<InterfaceInfo, IModuleInterfaceWrapper>> _interfaceTupleDic = new();

        private readonly Dictionary<IModuleInterface, ModuleHeader> _interfaceToModuleDic = new();

        private readonly DependencyGraph<ModuleHeader> _dependencyGraph = new();

        private HashSet<ModuleHeader> _modules = new();


        public TimeSpan TransitionTimeout { get; set; } = TimeSpan.FromMilliseconds(100);
        public IEnumerable<ModuleHeader> Modules => _modules;


        private void AddManagerInterface(IManagerInterface interf)
        {
            _managerInterfaceDic.Add(interf.ManagedInterfaceInfo.Type, interf);

            //add present managed interfaces to new manager
            foreach(var module in Modules)
            {
                foreach(var mi in module.ManagedInterfaces)
                {
                    if(interf.ManagedInterfaceInfo.IsCompatibleWith(mi.Info))
                    {
                        mi.RegisterTo(interf);
                    }
                }
            }
        }


        #region Register/UnRegister

        public async Task RegisterModuleAsync(ModuleHeader module)
        {

            using var token = await _lock.LockAsync();

            if (module is null)
            {
                throw new ArgumentNullException(nameof(module));
            }
            var report = CheckModuleCompatibility(module, token);
            if (!report.IsCompatible)
            {
                throw new ModuleIncompatibleException(report);
            }
            if (module.State != ModuleState.Uninitialized)
            {
                throw new InvalidModuleStateException("Module must be in state 'Unitialized' before beeing registered to a manager!");
            }
            if (_modules.Contains(module))
            {
                throw new ArgumentException("Module is already registered!");
            }

            await module.Control.InitializeAsync(this, TransitionTimeout, token);

            foreach (var interf in module.ProvidedInterfaces)
            {
                var info = interf.Info;
                _interfaceTupleDic.Add(info.Type, Tuple.Create(info, interf));
                _interfaceToModuleDic.Add(interf, module);
            }

            foreach (var interf in module.ManagerInterfaces)
            {
                AddManagerInterface(interf);
            }

            foreach (var interf in module.ManagedInterfaces)
            {
                Type type = interf.Info.Type;
                if (_managerInterfaceDic.ContainsKey(type))
                {
                    var manager = _managerInterfaceDic[type];

                    if (manager.ManagedInterfaceInfo.IsCompatibleWith(interf.Info))
                    {
                        interf.RegisterTo(_managerInterfaceDic[type]);
                    }
                }
            }

            _dependencyGraph.AddObject(module, report.ModuleDependencies);

            _modules.Add(module);
        }
        public async Task UnregisterModuleAsync(ModuleHeader module)
        {
            if (module is null)
            {
                throw new ArgumentNullException(nameof(module));
            }
            using var token = await _lock.LockAsync();
            if (!Modules.Contains(module))
            {
                throw new ArgumentException("Module is not registered on this manager!");
            }
            if (module.State != ModuleState.Stopped)
            {
                throw new InvalidModuleStateException("Module must be in state 'stopped' before being unregistered!");
            }
            if (_dependencyGraph.IsUsed(module))
            {
                throw new DependencyException();
            }

            _modules.Remove(module);

            foreach (var interf in module.ProvidedInterfaces)
            {
                _interfaceTupleDic.Remove(interf.Info.Type);
                _interfaceToModuleDic.Remove(interf);
            }

            foreach (var interf in module.ManagerInterfaces)
            {
                _managerInterfaceDic.Remove(interf.ManagedInterfaceInfo.Type);
                interf.UnregisterAllManagedInterfaces();
            }

            foreach (var interf in module.ManagedInterfaces)
            {
                if (interf.IsRegistered)
                {
                    interf.Unregister();
                }
            }

            _dependencyGraph.RemoveObject(module);

            await module.Control.TransitionStateAsync(ModuleState.Uninitialized, TransitionTimeout);

        }


        #endregion

        #region start/stop/reset single

        private async Task StopModuleAsync(ModuleHeader module, bool stopDependentModules, LockToken token)
        {
            if (module is null)
            {
                throw new ArgumentNullException(nameof(module));
            }
            if (!Modules.Contains(module))
            {
                throw new ArgumentException("Module is not registered on this manager!");
            }
            var dependents = _dependencyGraph.GetDependents(module);

            foreach (var m in dependents)
            {
                if (m.State == ModuleState.Running)
                {
                    if (stopDependentModules)
                    {
                        try
                        {
                            await StopModuleAsync(m, true, token);
                        }
                        catch (Exception ex)
                        {
                            throw new DependencyException("Dependent module could not be stopped!", ex);
                        }
                    }
                    else
                    {
                        throw new DependencyException("Dependent module is still running!");
                    }
                }
            }

            await module.Control.TransitionStateAsync(ModuleState.Stopped, TransitionTimeout);
        }

        public async Task StopModuleAsync(ModuleHeader module, bool stopDependentModules)
        {
            using var token = await _lock.LockAsync();
            await StopModuleAsync(module, stopDependentModules, token);
        }

        private async Task StartModuleAsync(ModuleHeader module, bool startDependingModules, LockToken token)
        {
            if (module is null)
            {
                throw new ArgumentNullException(nameof(module));
            }
            if (!Modules.Contains(module))
            {
                throw new ArgumentException("Module is not registered on this manager!");
            }
            var depending = _dependencyGraph.GetDependencies(module);

            foreach (var m in depending)
            {
                if (m.State != ModuleState.Running)
                {
                    if (startDependingModules)
                    {
                        try
                        {
                            await StartModuleAsync(m, true, token);
                        }
                        catch (Exception ex)
                        {
                            throw new DependencyException("Depending module could not be started!", ex);
                        }
                    }
                    else
                    {
                        throw new DependencyException("Depending module is not running!");
                    }
                }
            }

            await module.Control.TransitionStateAsync(ModuleState.Running, TransitionTimeout);
        }

        public async Task StartModuleAsync(ModuleHeader module, bool startDependingModules)
        {
            using var token = await _lock.LockAsync();
            await StartModuleAsync(module, startDependingModules, token);
        }

        public async Task ResetModuleAsync(ModuleHeader module)
        {
            if (module is null)
            {
                throw new ArgumentNullException(nameof(module));
            }
            using var token = await _lock.LockAsync();
            if (!Modules.Contains(module))
            {
                throw new ArgumentException("Module is not registered on this manager!");
            }
            await module.Control.TransitionStateAsync(ModuleState.Resetting, TransitionTimeout);
        }

        #endregion

        #region start/stop/reset all

        public async Task<bool> StopAllModulesAsync()
        {
            using var token = await _lock.LockAsync();

            var modules = new List<ModuleHeader>(_dependencyGraph.DependentList);

            modules.Reverse();

            foreach (var m in modules)
            {
                if (m.State == ModuleState.Stopped)
                {
                    continue;
                }
                try
                {
                    await StopModuleAsync(m, false, token);
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> StartAllModulesAsync()
        {
            using var token = await _lock.LockAsync();

            var modules = _dependencyGraph.DependentList;

            foreach (var m in modules)
            {
                if (m.State == ModuleState.Running)
                {
                    continue;
                }
                try
                {
                    await StartModuleAsync(m, false, token);
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public async Task ResetAllModulesAsync()
        {
            using var token = await _lock.LockAsync();

            if (Modules.Where(m => m.State != ModuleState.Stopped).Any())
            {
                throw new InvalidOperationException("ResetAllModulesAsync may only be called, if all modules are in stopped state!");
            }

            foreach (var module in _modules)
            {
                await module.Control.TransitionStateAsync(ModuleState.Resetting, TransitionTimeout);
            }

        }

        #endregion

        public async Task<T> GetInterfaceAsync<T>(LockToken token)
        {
            LockToken secondaryToken = null;

            if(token == null)
            {
                secondaryToken = await _lock.LockAsync();
            }

            if (!_interfaceTupleDic.ContainsKey(typeof(T)))
            {
                throw new InterfaceNotFoundException();
            }
            var result = (T)_interfaceTupleDic[typeof(T)].Item2.Data;

            if(token == null)
            {
                secondaryToken.Dispose();
            }

            return result;
        }


        public async Task<CompatibilityReport> CheckModuleCompatibilityAsync(ModuleHeader module)
        {
            using var token = await _lock.LockAsync();
            return CheckModuleCompatibility(module, token);
        }

        private CompatibilityReport CheckModuleCompatibility(ModuleHeader module, LockToken token)
        {
            if (module is null)
            {
                throw new ArgumentNullException(nameof(module));
            }

            ReportBuilder builder = new();

            foreach (var dep in module.InterfaceDependencies)
            {
                if (!_interfaceTupleDic.ContainsKey(dep.Type))
                {
                    //dependency not found
                    builder.AddUnsatisfiedDependencyError(dep);
                }
                else if (!_interfaceTupleDic[dep.Type].Item1.Version.IsLegacyCompatibleWith(dep.Version))
                {
                    //interface is not compatible
                    builder.AddIncompatibleDependencyError(dep, _interfaceTupleDic[dep.Type].Item1);
                }
                else
                {
                    builder.AddDependencyModule(_interfaceToModuleDic[_interfaceTupleDic[dep.Type].Item2]);
                }
            }

            foreach(var interf in module.ProvidedInterfaces)
            {
                if (_interfaceTupleDic.ContainsKey(interf.Info.Type))
                {
                    //interface with this type is already provided by other module
                    var wrapper = _interfaceTupleDic[interf.Info.Type].Item2;
                    if (wrapper.Data is IModuleInterface data)
                    {
                        builder.AddConflict(data);
                    }
                    else
                    {
                        builder.AddConflict(wrapper);
                    }
                }
            }

            foreach(var interf in module.ManagerInterfaces)
            {
                if (_managerInterfaceDic.ContainsKey(interf.ManagedInterfaceInfo.Type))
                {
                    //managed interface of this type is already used by other manager
                    builder.AddManagerConflict(interf);
                }
            }

            return builder.CreateReport();
        }


        public async Task<List<ModuleHeader>> GetDependentModules(ModuleHeader module)
        {
            if (module is null)
            {
                throw new ArgumentNullException(nameof(module));
            }
            using var token = await _lock.LockAsync();
            if (Modules.Contains(module))
            {
                throw new ArgumentException("Module is not registered on this manager!");
            }
            return _dependencyGraph.GetDependencies(module);
        }

        public async Task<List<ModuleHeader>> GetDependingModules(ModuleHeader module)
        {
            if (module is null)
            {
                throw new ArgumentNullException(nameof(module));
            }
            using var token = await _lock.LockAsync();
            if (Modules.Contains(module))
            {
                throw new ArgumentException("Module is not registered on this manager!");
            }
            return _dependencyGraph.GetDependents(module);
        }

    }
}
