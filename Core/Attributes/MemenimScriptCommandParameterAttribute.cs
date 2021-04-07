using System;

namespace Memenim.Scripting.Core
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class MemenimScriptCommandParameterAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }

        public MemenimScriptCommandParameterAttribute(
            string name = null, string description = null)
        {
            Name = name;
            Description = description;
        }
    }
}
