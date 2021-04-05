using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternUtils.Module_Framework.Extensions
{
    public static class ModuleManagerEx
    {
        public static ModuleHeader GetModuleByName(this IModuleManager manager, string name)
        {
            return manager.Modules.ToList().Find(h => h.Info.Name == name);
        }
    }
}
