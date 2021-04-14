using PatternUtils.Module_Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternUtils.Module_Framework.Impl
{
    public interface IModuleInterfaceWrapper : IModuleInterface
    {
        object Data { get; }
    }
    public class ModuleInterfaceWrapper<T> : ModuleInterface, IModuleInterfaceWrapper where T : class
    {
        public T Data { get; }

        object IModuleInterfaceWrapper.Data => Data;

        public ModuleInterfaceWrapper(T data, Version version)
            :base(new(typeof(T), version))
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }
    }
}
