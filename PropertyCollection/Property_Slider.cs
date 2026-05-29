using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MappingHelper.PropertyCollection
{
    internal class Property_Slider : Property
    {
        public Property_Slider(string name, string key = null, bool canBeDisabled = false, bool startEnabled = false, Dictionary<string, string> enableIf = null, Dictionary<string, string> disableIf = null, float value_default = 0)
    : base(name, key, canBeDisabled, startEnabled, enableIf, disableIf)
        {
            data["slider"] = true;
            data["default"] = value_default;
        }
    }
}
