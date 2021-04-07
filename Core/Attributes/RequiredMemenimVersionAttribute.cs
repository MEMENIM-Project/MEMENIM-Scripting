using System;

namespace Memenim.Scripting.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RequiredMemenimVersionAttribute : Attribute
    {
        public MemenimVersionRange Range { get; }

        public RequiredMemenimVersionAttribute(
            string minVersion = null, string maxVersion = null)
        {
            Range = new MemenimVersionRange(
                MemenimVersion.Parse(minVersion),
                MemenimVersion.Parse(maxVersion));
        }
    }
}
