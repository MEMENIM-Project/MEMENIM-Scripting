using System;

namespace Memenim.Scripting.Core
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MemenimScriptSettingAttribute : Attribute
    {
        public string Name { get; }

        public MemenimScriptSettingAttribute(
            string name = null)
        {
            Name = name;
        }
    }
}
