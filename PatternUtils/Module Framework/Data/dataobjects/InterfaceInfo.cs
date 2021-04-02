
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternUtils.Module_Framework.Data
{
    public struct InterfaceInfo
    {
        public InterfaceInfo(Type interfaceType, Version version)
        {
            Version = version;
            Type = interfaceType;
        }
        public InterfaceInfo(Type interfaceType, int major, int minor, int patch = 0)
            :this(interfaceType, Version.Create(major, minor, patch))
        {
        }
        public Version Version { get; }
        public Type Type { get; }



        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is not InterfaceInfo info)
            {
                return false;
            }
            return info.Version == Version && info.Type == Type;
        }
        public override int GetHashCode()
        {
            return Version.GetHashCode() + (Type != null ? 31 * Type.GetHashCode() : 0);
        }

        public static bool operator ==(InterfaceInfo left, InterfaceInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(InterfaceInfo left, InterfaceInfo right)
        {
            return !(left == right);
        }


        /// <summary>
        /// Checks whether this info is backwards compatible with another info.
        /// </summary>
        /// <param name="version"></param>
        /// <returns>true iff version of given info provides necessary api version and has same type</returns>
        public bool IsCompatibleWith(InterfaceInfo info)
        {
            return info.Type == Type && info.Version.IsLegacyCompatibleWith(Version);
        }

    }
}
