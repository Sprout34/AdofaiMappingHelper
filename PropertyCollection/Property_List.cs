using System.Collections.Generic;

namespace MappingHelper.PropertyCollection
{
    public class Property_List : Property
    {
        private readonly List<string> list;
        private readonly string value_default;

        public Property_List(string name, List<string> list, string value_default = default, string key = null, bool canBeDisabled = false, bool startEnabled = false, Dictionary<string, string> enableIf = null, Dictionary<string, string> disableIf = null)
            : base(name, key, canBeDisabled, startEnabled, enableIf, disableIf)
        {
            data["type"] = "Enum:" + typeof(Dummy).AssemblyQualifiedName;
            data["unit"] = string.Join(";", list);
            data["default"] = value_default.IsNullOrEmpty() ? list[0] : value_default;
        }

        internal enum Dummy { }
    }
}
