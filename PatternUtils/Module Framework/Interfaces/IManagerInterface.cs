using PatternUtils.Module_Framework.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternUtils.Module_Framework.Interfaces
{
    /// <summary>
    /// Users can register and unregister managed interfaces to implementations of this interface.
    /// </summary>
    public interface IManagerInterface : IModuleInterface, IUnsubscribeable<IManagedInterface>
    {
        /// <summary>
        /// interface type that can be registered onto this manager interface
        /// </summary>
        InterfaceInfo ManagedInterfaceInfo { get; }

        /// <summary>
        /// Register interface of type specified in ManagedInterface to this manager.
        /// </summary>
        /// <param name="obj">instance of a compatible implementation</param>
        /// <returns>Unsubscriber</returns>
        /// <exception cref="ArgumentException">obj is not of compatible</exception>
        IDisposable RegisterManagedInterface(IManagedInterface obj);

        /// <summary>
        /// Calls unregister on all registered interfaces.
        /// </summary>
        void UnregisterAllManagedInterfaces();

    }
}
