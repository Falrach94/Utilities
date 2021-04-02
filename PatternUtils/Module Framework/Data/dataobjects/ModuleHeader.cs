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
    /// A module encapsulates the necessary functionality for exactly one (complex) task.
    /// Modules are managed by an IModuleManager and can communicate with each other via interfaces.
    /// A modules state is controlled via a ModuleControl object which implements all state related module logic and acts as a module factory.
    /// </summary>
    public record ModuleHeader
    {
        internal ModuleHeader(ModuleControl control,
                      ModuleInfo info,
                      InterfaceInfo[] interfaceDependencies,
                      IModuleInterface[] providedInterfaces,
                      IManagedInterface[] managedInterfaces,
                      IManagerInterface[] managerInterfaces)
        {
            Control = control ?? throw new ArgumentNullException(nameof(control));
            Info = info;
            InterfaceDependencies = interfaceDependencies ?? throw new ArgumentNullException(nameof(interfaceDependencies));
            ProvidedInterfaces = providedInterfaces ?? throw new ArgumentNullException(nameof(providedInterfaces));
            ManagedInterfaces = managedInterfaces ?? throw new ArgumentNullException(nameof(managedInterfaces));
            ManagerInterfaces = managerInterfaces ?? throw new ArgumentNullException(nameof(managerInterfaces));
        }

        public ModuleState State => Control.State;
        public ModuleControl Control { get; }
        public ModuleInfo Info { get; }

        /// <summary>
        /// List of interfaces this module needs to operate.
        /// </summary>
        public InterfaceInfo[] InterfaceDependencies { get; }

        /// <summary>
        /// List of interfaces that other modules can use to access this modules data or functionality.
        /// </summary>
        public IModuleInterface[] ProvidedInterfaces { get; }

        /// <summary>
        /// List of interfaces that can be registered to other modules. 
        /// </summary>
        public IManagedInterface[] ManagedInterfaces { get; }

        /// <summary>
        /// List of interfaces that other modules can register to on this module. 
        /// </summary>
        public IManagerInterface[] ManagerInterfaces { get; }

    }
}
