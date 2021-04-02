using PatternUtils.Module_Framework.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternUtils.Module_Framework.Interfaces
{
    public interface IModuleInterface
    {
        InterfaceInfo Info { get; }
        //Version Version { get; }
    }
}
