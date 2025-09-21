using System;
using System.Collections.Generic;

namespace MappingHelper.PropertyCollection
{
    public class Property_Enum<T> : Property where T : Enum
    {
        public Property_Enum(string name, T value_default = default, string key = null, bool canBeDisabled = false, bool startEnabled = false, Dictionary<string, string> enableIf = null, Dictionary<string, string> disableIf = null)
            : base(name, key, canBeDisabled, startEnabled, enableIf, disableIf)
        {
            data["type"] = "Enum:" + typeof(T).AssemblyQualifiedName;
            data["default"] = value_default.ToString();
        }
    }
}
