using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternUtils.Module_Framework.Data
{
    public enum ModuleState
    {
        Uninitialized, //-> Stopped, Error
        Stopped, //-> Resetting, Running, Uninitialized, Error
        Resetting, //-> Stopped, Error
        Running, //-> Stopped, Error
        Error
    }
}
