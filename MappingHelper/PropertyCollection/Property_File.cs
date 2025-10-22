using ADOFAI;
using System.Collections.Generic;

namespace MappingHelper.PropertyCollection
{
    public class Property_File : Property
    {
        public Property_File(string name, string fileType, string value_default = null, string key = null, bool canBeDisabled = false, bool startEnabled = false, Dictionary<string, string> enableIf = null, Dictionary<string, string> disableIf = null)
            : base(name, key, canBeDisabled, startEnabled, enableIf, disableIf)
        {
            data["type"] = "File";
            data["default"] = value_default ?? string.Empty;
            data["fileType"] = fileType;
        }
    }
}
