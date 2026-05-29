using System.Collections.Generic;

namespace MappingHelper.PropertyCollection
{
    public class Property_TextArea : Property
    {
        private readonly string value_default;

        public Property_TextArea(string name, string value_default = null, string key = null, bool canBeDisabled = false, bool startEnabled = false, Dictionary<string, string> enableIf = null, Dictionary<string, string> disableIf = null)
            : base(name, key, canBeDisabled, startEnabled, enableIf, disableIf)
        {
            data["type"] = "Text";
            data["default"] = value_default ?? string.Empty;
        }
    }
}
