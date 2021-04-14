using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternUtils.Module_Framework.Impl.WeakDependency
{
    /// <summary>
    /// Dependency that may or may not be present and will be set when a matching WeakDependencyProvider is found. 
    /// Using this allows for weak circular depedencies.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WeakDependency<T> where T : class
    {
        public bool IsSet => Value != null;
        public T Value { get; private set; }

        private readonly Action<WeakDependency<T>, T> _setHandler;
        private readonly Version _version;

        public WeakDependency(Version version)
            :this(version, null)
        { }

        public WeakDependency(Version version, Action<WeakDependency<T>, T> setHandler)
        {
            _setHandler = setHandler;
            _version = version;
        }

        public ManagedInterfaceWrapper<WeakDependency<T>> CreateWrapper()
        {
            return new ManagedInterfaceWrapper<WeakDependency<T>>(this, _version);
        }

        public void Set(T val)
        {
            Value = val;
            _setHandler?.Invoke(this, val);
        }

    }
}
