using PatternUtils.Module_Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternUtils.Module_Framework.Impl
{
    public class ModuleInterfaceWrapper<T> : ModuleInterface
    {
        public T Data { get; }

        public ModuleInterfaceWrapper(T data, Version version)
            :base(new(typeof(T), version))
        {
            Data = data;
        }
    }
}
