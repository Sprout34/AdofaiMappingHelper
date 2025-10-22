using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MappingHelper.PropertyCollection
{
    internal class Property_FloatPair : Property
    {
        public Property_FloatPair(string name, object value_default = null, object min = null, object max = null, string key = null, bool isRange = true, bool canBeDisabled = false, bool startEnabled = false, Dictionary<string, string> enableIf = null, Dictionary<string, string> disableIf = null)
            : base(name, key, canBeDisabled, startEnabled, enableIf, disableIf)
        {
            data["type"] = "FloatPair";
            data["default"] = value_default != null ? new List<object> { ((Vector2)value_default).x, ((Vector2)value_default).y } : new List<object> { 0, 0 };
            data["min"] = min != null ? Convert.ToSingle(min) : float.NegativeInfinity;
            data["max"] = max != null ? Convert.ToSingle(max) : float.PositiveInfinity;
            data["isRange"] = isRange;
        }

    }
}
