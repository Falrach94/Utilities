using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternUtils.Module_Framework.Interfaces
{
    public interface IManagedInterface : IModuleInterface
    {
        bool IsRegistered { get; }
        /// <summary>
        /// Registers to a given manager interface.
        /// </summary>
        /// <param name="manager"></param>
        /// <exception cref="InvalidOperationException">Interface was already registered</exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InterfaceIncompatibleException"></exception>
        void RegisterTo(IManagerInterface manager);
        /// <summary>
        /// Unregisters from current manager.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        void Unregister();
    }
}
