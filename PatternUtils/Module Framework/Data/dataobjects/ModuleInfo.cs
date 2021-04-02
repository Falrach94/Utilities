using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternUtils.Module_Framework.Data
{
    public struct ModuleInfo
    {
        public ModuleInfo(string name, PatternUtils.Version version)
        {
            Name = name;
            Version = version;
        }

        public string Name { get; }
        public PatternUtils.Version Version { get; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is not ModuleInfo info) return false;
            return info.Name == Name && Version == info.Version;
        }

        public static bool operator ==(ModuleInfo left, ModuleInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ModuleInfo left, ModuleInfo right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return 7*Version.GetHashCode() + 31*Name.GetHashCode();
        }
    }
}
