using PatternUtils.Module_Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternUtils.Module_Framework.Impl
{
    public class ManagedInterfaceWrapper<T> : ModuleInterfaceWrapper<T>, IManagedInterface where T : class
    {
        private IDisposable _unsubscriber;

        public ManagedInterfaceWrapper(T data, Version version) : base(data, version)
        {
        }

        public bool IsRegistered => _unsubscriber != null;

        protected virtual void OnRegistration(IManagerInterface manager)
        {
        }
        protected virtual void OnUnregistration()
        {
        }

        public void RegisterTo(IManagerInterface manager)
        {
            _unsubscriber = manager.RegisterManagedInterface(this);
            OnRegistration(manager);
        }

        public void Unregister()
        {
            if(_unsubscriber != null)
            {
                _unsubscriber.Dispose();
                _unsubscriber = null;
                OnUnregistration();
            }
        }
    }
}
