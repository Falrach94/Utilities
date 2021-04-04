using PatternUtils.Module_Framework.Impl;
using PatternUtils.Module_Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternUtils.Module_Framework.Data.builder
{
    public class ModuleBuilder
    {
        private ModuleControl _control;
        private ModuleInfo? _info;
        private InterfaceInfo[] _interfaceDependencies = Array.Empty<InterfaceInfo>();
        private IModuleInterfaceWrapper[] _providedInterfaces = Array.Empty<IModuleInterfaceWrapper>();
        private IManagedInterface[] _managedInterfaces = Array.Empty<IManagedInterface>();
        private IManagerInterface[] _managerInterfaces = Array.Empty<IManagerInterface>();

        public ModuleBuilder SetControl(ModuleControl control)
        {
            _control = control;
            return this;
        }
        public ModuleBuilder SetInfo(ModuleInfo info)
        {
            _info = info;
            return this;
        }
        public ModuleBuilder SetInfo(string name, PatternUtils.Version version)
        {
            return SetInfo(new ModuleInfo(name, version));
        }
        public ModuleBuilder SetDependencies(params InterfaceInfo[] dependencies)
        {
            _interfaceDependencies = dependencies;
            return this;
        }
        public ModuleBuilder SetProvidedInterfaces(params IModuleInterface[] providedInterfaces)
        {
            var wrapperAr = providedInterfaces.Select( i =>
                {
                    if(i is IModuleInterfaceWrapper)
                    {
                        return i;
                    }

                    var type = typeof(ModuleInterfaceWrapper<>).MakeGenericType(i.GetType());
                    return Activator.CreateInstance(type, i, i.Info.Version);
                }).Cast<IModuleInterfaceWrapper>().ToArray();

            _providedInterfaces = wrapperAr;
            return this;
        }
        public ModuleBuilder SetManagedInterfaces(params IManagedInterface[] managedInterfaces)
        {
            _managedInterfaces = managedInterfaces;
            return this;
        }
        public ModuleBuilder SetManagerInterfaces(params IManagerInterface[] managerInterfaces)
        {
            _managerInterfaces = managerInterfaces;
            return this;
        }

        /// <summary>
        /// Create module.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ModuleBuilderException"></exception>
        public ModuleHeader CreateModule()
        {
            if (_control == null)
            {
                throw new ModuleBuilderException("Control not set!");
            }
            if (_info == null)
            {
                throw new ModuleBuilderException("Info not set!");
            }
            if (_interfaceDependencies == null)
            {
                throw new ModuleBuilderException("Dependencies not set!");
            }
            if (_providedInterfaces == null)
            {
                throw new ModuleBuilderException("Provided interfaces not set!");
            }
            if (_managedInterfaces == null)
            {
                throw new ModuleBuilderException("Managed interfaces not set!");
            }
            if (_managerInterfaces == null)
            {
                throw new ModuleBuilderException("Manager interfaces not set!");
            }

            return new ModuleHeader(_control,
                              _info.GetValueOrDefault(),
                              _interfaceDependencies,
                              _providedInterfaces,
                              _managedInterfaces,
                              _managerInterfaces);
        }
    }
}
