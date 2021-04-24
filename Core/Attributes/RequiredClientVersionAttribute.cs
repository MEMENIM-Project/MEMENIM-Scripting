using System;

namespace Memenim.Scripting.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RequiredClientVersionAttribute : Attribute
    {
        public ClientVersionRange Range { get; }

        public RequiredClientVersionAttribute(
            string minVersion = null, string maxVersion = null)
        {
            Range = new ClientVersionRange(
                ClientVersion.Parse(minVersion),
                ClientVersion.Parse(maxVersion));
        }
    }
}
