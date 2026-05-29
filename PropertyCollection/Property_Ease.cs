using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MappingHelper.PropertyCollection
{
    internal class Property_Ease : Property
    {
        public Property_Ease(string name, string key = null, bool canBeDisabled = false, bool startEnabled = false, Dictionary<string, string> enableIf = null, Dictionary<string, string> disableIf = null)
    : base(name, key, canBeDisabled, startEnabled, enableIf, disableIf)
        {
            data["type"] = "Enum:Ease";
            data["default"] = "Linear";
        }
    }
}
