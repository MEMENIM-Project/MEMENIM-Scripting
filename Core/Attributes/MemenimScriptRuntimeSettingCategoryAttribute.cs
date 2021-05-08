using System;

namespace Memenim.Scripting.Core
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MemenimScriptRuntimeSettingCategoryAttribute : Attribute
    {
        public string Name { get; }

        public MemenimScriptRuntimeSettingCategoryAttribute(
            string name = null)
        {
            Name = name;
        }
    }
}
