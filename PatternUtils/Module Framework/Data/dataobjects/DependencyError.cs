using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternUtils.Module_Framework.Data
{
    public class DependencyError
    {
        public InterfaceInfo ExpectedInterface { get; set; }
        public InterfaceInfo ActualInterface { get; set; }
        public DependencyErrorType Type { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj == this) return true;
            if (obj is not DependencyError error) return false;

            return error.ExpectedInterface == ExpectedInterface
                && error.ActualInterface == ActualInterface
                && error.Type == Type;
        }

        public override int GetHashCode()
        {
            return ExpectedInterface.GetHashCode() + ActualInterface.GetHashCode() + Type.GetHashCode(); 
        }
    }
}
