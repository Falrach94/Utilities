using PatternUtils.Module_Framework.Data;
using PatternUtils.Module_Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternUtils.Module_Framework
{
    /// <summary>
    /// Manages a collection of modules. 
    /// A module may be added at any time.
    /// Circular dependencies are not allowed.
    /// All functions have to be implemented threadsafe.
    /// </summary>
    public interface IModuleManager : IInterfaceProvider
    {
        IEnumerable<ModuleHeader> Modules { get;}

        /// <summary>
        /// Registeres and starts module.
        /// </summary>
        /// <param name="module"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">module is already registered</exception>
        /// <exception cref="ModuleIncompatibleException"></exception>
        /// <exception cref="InvalidModuleStateException">module was not in state 'Uninitialized'</exception>
        Task RegisterModuleAsync(ModuleHeader module);
        /// <summary>
        /// Unregisteres given module.
        /// Module must be stopped and all other modules that depend on interfaces from this module must be unregistered first.
        /// </summary>
        /// <param name="module"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">module is not registered on this manager</exception>
        /// <exception cref="InvalidModuleStateException">module was not in state 'Stopped'</exception>
        /// <exception cref="DependencyException">other registered modules are dependent of this module</exception>
        Task UnregisterModuleAsync(ModuleHeader module);

        /// <summary>
        /// Gets all modules that depend on a given module.
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">module is not registered on this manager</exception>
        Task<List<ModuleHeader>> GetDependentModules(ModuleHeader module);
        /// <summary>
        /// Gets all modules a given module depends on.
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">module is not registered on this manager</exception>
        Task<List<ModuleHeader>> GetDependingModules(ModuleHeader module);

        /// <summary>
        /// Stops given module.
        /// </summary>
        /// <param name="module"></param>
        /// <param name="stopDependentModules">true: dependent modules are also stopped, false: exception is thrown if dependent modules are still running</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">Module not found</exception>
        /// <exception cref="InvalidModuleStateException">module was not in state 'Running'</exception>
        /// <exception cref="DependencyException">other modules are dependent of this module and still running</exception>
        Task StopModuleAsync(ModuleHeader module, bool stopDependentModules);
        /// <summary>
        /// Starts module.
        /// </summary>
        /// <param name="module"></param>
        /// <param name="startDependentModules">true: depending modules are also started, false: exception is thrown if depending modules are not running</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">Module not found</exception>
        /// <exception cref="InvalidModuleStateException">module was not in state 'Stopped'</exception>
        /// <exception cref="DependencyException">this module depends on other modules which are not running</exception>
        Task StartModuleAsync(ModuleHeader module, bool startDependingModules);
        /// <summary>
        /// Starts module.
        /// </summary>
        /// <param name="module"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">Module not found</exception>
        /// <exception cref="InvalidModuleStateException">module was not in state 'Stopped'</exception>
        Task ResetModuleAsync(ModuleHeader module);


        /// <summary>
        /// Stops all modules.
        /// </summary>
        /// <returns>false: at least one module did not transition to state 'Stopped'</returns>
        Task<bool> StopAllModulesAsync();
        /// <summary>
        /// Resets all modules.
        /// </summary>
        /// <exception cref="InvalidOperationException">At least one module was not in state 'Stopped'</exception>
        Task ResetAllModulesAsync();
        /// <summary>
        /// Starts all modules.
        /// </summary>
        /// <returns>false: at least one module did not transition to state 'Running'</returns>
        Task<bool> StartAllModulesAsync();

        /// <summary>
        /// Checks wether dependencies are met and provided interfaces don't create conflicts.
        /// </summary>
        /// <param name="module"></param>
        /// <returns>CompatibilityReport</returns>
        /// <exception cref="ArgumentNullException"></exception>
        Task<CompatibilityReport> CheckModuleCompatibilityAsync(ModuleHeader module);


    }
}
