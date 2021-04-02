using PatternUtils.Module_Framework.Data;
using PatternUtils.Module_Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternUtils.Module_Framework.Impl
{
    public class ModuleInterface : IModuleInterface
    {
        public InterfaceInfo Info { get; }
        //public Version Version { get; }

        public ModuleInterface(InterfaceInfo info)
        {
            Info = info;
        }
    }
}
