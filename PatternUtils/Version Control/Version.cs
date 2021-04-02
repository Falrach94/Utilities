using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternUtils
{
    public struct Version : IComparable
    {
        //not backwards compatible changes
        public int Major { get; }
        //backwards compatible changes
        public int Minor { get; }
        //backwards compatible fixes
        public int Patch { get; }

        public Version(int major, int minor, int patch)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
        }

        public static Version Create(int major, int minor = 0, int patch = 0)
        {
            return new Version(major, minor, patch);
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Patch}";
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is not Version version) return false;
            return version.Major == Major && version.Minor == Minor && version.Patch == Patch;
        }
        public override int GetHashCode()
        {
            return 10000 * Major + 100 * Minor + Patch;
        }

        public int CompareTo(object obj)
        {
            if(obj is Version version)
            {
                if(Equals(version))
                {
                    return 0;
                }
                if(version.Major > Major || version.Minor > Minor || version.Patch > Patch)
                {
                    return -1;
                }
                return 1;
            }
            return 0;
        }

        /// <summary>
        /// Checks whether this version is backwards compatible with another version.
        /// </summary>
        /// <param name="version"></param>
        /// <returns>true iff given version is of same major and smaller or equal minor version</returns>
        public bool IsLegacyCompatibleWith(Version version)
        {
            return Major == version.Major && version.Minor <= Minor;
        }

        public static bool operator ==(Version left, Version right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Version left, Version right)
        {
            return !(left == right);
        }

        public static bool operator <(Version left, Version right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(Version left, Version right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(Version left, Version right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(Version left, Version right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}
