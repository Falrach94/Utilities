using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternUtils.Module_Framework.Impl.WeakDependency
{
    public class WeakDependencyProvider<T> : ManagerInterface<WeakDependency<T>> where T : class
    {
        private readonly T _object;

        public WeakDependencyProvider(T obj, Version version) : base(Version.Create(1), version)
        {
            _object = obj ?? throw new ArgumentNullException(nameof(obj));
        }

        protected override void OnRegistration(ManagedInterfaceWrapper<WeakDependency<T>> obj)
        {
            obj.Data.Set(_object);
        }
        protected override void OnUnregistration(ManagedInterfaceWrapper<WeakDependency<T>> obj)
        {
            obj.Data.Set(null);
        }

    }
}
