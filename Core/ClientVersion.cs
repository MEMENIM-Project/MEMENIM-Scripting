using System;
using System.Globalization;
using System.Text;

namespace Memenim.Scripting.Core
{
    public sealed class ClientVersion : IEquatable<ClientVersion>
    {
        public static readonly ClientVersion MinValue;
        public static readonly ClientVersion MaxValue;

        public static readonly ClientVersion Any;

        public uint? Major { get; }
        public uint? Minor { get; }
        public uint? Patch { get; }

        public bool AnyMajor
        {
            get
            {
                return !Major.HasValue;
            }
        }
        public bool AnyMinor
        {
            get
            {
                return !Minor.HasValue;
            }
        }
        public bool AnyPatch
        {
            get
            {
                return !Patch.HasValue;
            }
        }

        static ClientVersion()
        {
            MinValue = new ClientVersion(
                0, 0, 0);
            MaxValue = new ClientVersion(
                uint.MaxValue, uint.MaxValue, uint.MaxValue);

            Any = new ClientVersion(
                null, null, null);
        }

        public ClientVersion(uint? major = null,
            uint? minor = null, uint? patch = null)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
        }



        public int CompareTo(ClientVersion other)
        {
            if (other is null)
                return 1;

            if (AnyMajor != other.AnyMajor)
            {
                if (AnyMajor)
                    return 1;

                return -1;
            }
            if (Major != other.Major)
            {


                if (Major > other.Major)
                    return 1;

                return -1;
            }

            if (AnyMinor != other.AnyMinor)
            {
                if (AnyMinor)
                    return 1;

                return -1;
            }
            if (Minor != other.Minor)
            {
                if (Minor > other.Minor)
                    return 1;

                return -1;
            }

            if (AnyPatch != other.AnyPatch)
            {
                if (AnyPatch)
                    return 1;

                return -1;
            }
            else
            {
                if (Patch != other.Patch)
                {
                    if (Patch > other.Patch)
                        return 1;

                    return -1;
                }
            }

            return 0;
        }



        public bool Equals(ClientVersion other)
        {
            return CompareTo(other) == 0;
        }

        public override bool Equals(object other)
        {
            if (!(other is ClientVersion otherVersion))
                return false;

            return Equals(otherVersion);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                Major, Minor, Patch);
        }

        public override string ToString()
        {
            return $"{(Major.HasValue ? Major.Value.ToString() : "*")}." +
                   $"{(Minor.HasValue ? Minor.Value.ToString() : "*")}." +
                   $"{(Patch.HasValue ? Patch.Value.ToString() : "*")}";
        }


        private static uint? ParseComponent(string component)
        {
            StringBuilder resultBuilder = new StringBuilder(
                component.Length);

            foreach (var componentChar in component)
            {
                if (char.IsDigit(componentChar))
                    resultBuilder.Append(componentChar);
            }

            var result = resultBuilder
                .ToString();

            if (result == "0")
                return 0;

            result = result
                .TrimStart('0');

            if (string.IsNullOrEmpty(result))
                return null;

            if (uint.TryParse(result, NumberStyles.Integer,
                CultureInfo.InvariantCulture, out var resultNumber))
            {
                return resultNumber;
            }

            return null;
        }

        public static ClientVersion Parse(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
                return Any;

            uint? major = null;
            uint? minor = null;
            uint? patch = null;

            var components = version
                .Split('.');

            if (components.Length > 0)
                major = ParseComponent(components[0]);
            if (components.Length > 1)
                minor = ParseComponent(components[1]);
            if (components.Length > 2)
                patch = ParseComponent(components[2]);

            return new ClientVersion(
                major, minor, patch);
        }



        public static bool operator ==(ClientVersion left, ClientVersion right)
        {
            if (left is null)
                return right is null;

            return left.Equals(right);
        }
        public static bool operator !=(ClientVersion left, ClientVersion right)
        {
            return !(left == right);
        }

        public static bool operator <(ClientVersion left, ClientVersion right)
        {
            return left is null ? !(right is null) : left.CompareTo(right) < 0;
            //return left is null || left.CompareTo(right) < 0;
        }
        public static bool operator <=(ClientVersion left, ClientVersion right)
        {
            return left is null || left.CompareTo(right) <= 0;
            //return left is null ? !(right is null) : left.CompareTo(right) <= 0;
        }

        public static bool operator >(ClientVersion left, ClientVersion right)
        {
            return !(left is null) && left.CompareTo(right) > 0;
        }
        public static bool operator >=(ClientVersion left, ClientVersion right)
        {
            return left is null ? right is null : left.CompareTo(right) >= 0;
        }
    }
}
