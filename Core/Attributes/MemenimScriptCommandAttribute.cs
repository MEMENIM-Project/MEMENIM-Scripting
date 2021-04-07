using System;

namespace Memenim.Scripting.Core
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MemenimScriptCommandAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }

        public MemenimScriptCommandAttribute(
            string name = null, string description = null)
        {
            Name = name;
            Description = description;
        }
    }
}
