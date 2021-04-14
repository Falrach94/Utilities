using PatternUtils.Module_Framework.Data;
using PatternUtils.Module_Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternUtils.Module_Framework.Impl
{
    public class ManagerInterface<TManagedInterface> : ModuleInterface, IManagerInterface where TManagedInterface : class
    {
        private readonly List<ManagedInterfaceWrapper<TManagedInterface>> _managedInterfaces = new();

        private readonly Action<TManagedInterface> _registerAction;
        private readonly Action<TManagedInterface> _unregisterAction;

        public ManagerInterface(Version version, Version managedVersion) : base(new(typeof(TManagedInterface), version))
        {
            ManagedInterfaceInfo = new InterfaceInfo(typeof(TManagedInterface), managedVersion);
        }
        public ManagerInterface(Version version,
                                Version managedVersion,
                                Action<TManagedInterface> registerAction,
                                Action<TManagedInterface> unregisterAction)
            : base(new(typeof(TManagedInterface), version))
        {
            if (registerAction is null)
            {
                throw new ArgumentNullException(nameof(registerAction));
            }

            if (unregisterAction is null)
            {
                throw new ArgumentNullException(nameof(unregisterAction));
            }

            _registerAction = registerAction;
            _unregisterAction = unregisterAction;

            ManagedInterfaceInfo = new InterfaceInfo(typeof(TManagedInterface), managedVersion);
        }

        public InterfaceInfo ManagedInterfaceInfo { get; }
        public IReadOnlyList<ManagedInterfaceWrapper<TManagedInterface>> ManagedInterfaces => _managedInterfaces;

        protected virtual void OnRegistration(ManagedInterfaceWrapper<TManagedInterface> obj)
        {
        }
        protected virtual void OnUnregistration(ManagedInterfaceWrapper<TManagedInterface> obj)
        {
        }

        public IDisposable RegisterManagedInterface(IManagedInterface obj)
        {
            if(!ManagedInterfaceInfo.IsCompatibleWith(obj.Info))
            {
                throw new ArgumentException("Given interface is not compatible with this manager!");
            }
            var wrapper = (ManagedInterfaceWrapper<TManagedInterface>)obj;
            _managedInterfaces.Add(wrapper);
            OnRegistration(wrapper);
            _registerAction?.Invoke(wrapper.Data);
            return new Unsubscriber<IManagedInterface>(obj, this);
        }

        public void UnregisterAllManagedInterfaces()
        {
            var copy = new List<ManagedInterfaceWrapper<TManagedInterface>>(_managedInterfaces);
            foreach(var i in copy)
            {
                i.Unregister();
            }
        }

        public void Unsubscribe(IManagedInterface subscribedObject)
        {
            var wrapper = (ManagedInterfaceWrapper<TManagedInterface>)subscribedObject;
            _managedInterfaces.Remove(wrapper);
            OnUnregistration(wrapper);
            _unregisterAction?.Invoke(wrapper.Data);
        }

    }
}
