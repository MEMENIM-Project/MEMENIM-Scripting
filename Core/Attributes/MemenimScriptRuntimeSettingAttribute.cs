using System;

namespace Memenim.Scripting.Core
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MemenimScriptRuntimeSettingAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }

        public MemenimScriptRuntimeSettingAttribute(
            string name = null, string description = null)
        {
            Name = name;
            Description = description;
        }
    }
}
