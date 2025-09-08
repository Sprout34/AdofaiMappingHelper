using System;
using System.Collections.Generic;
using UnityEngine;

namespace MappingHelper.PropertyCollection
{
    public class Property_InputField : Property
    {
        public enum InputType
        {
            Float, Int, String, Vector2
        }

        public Property_InputField(string name, InputType type, object value_default = null, object min = null, object max = null, string unit = null, string key = null, bool canBeDisabled = false, bool startEnabled = false, Dictionary<string, string> enableIf = null, Dictionary<string, string> disableIf = null)
            : base(name, key, canBeDisabled, startEnabled, enableIf, disableIf)
        {
            data["type"] = Enum.GetName(typeof(InputType), type);
            data["unit"] = unit;
            switch (type)
            {
                case InputType.Float:
                    data["default"] = Convert.ToSingle(value_default);
                    data["min"] = min != null ? Convert.ToSingle(min) : float.NegativeInfinity;
                    data["max"] = max != null ? Convert.ToSingle(max) : float.PositiveInfinity;
                    break;
                case InputType.Int:
                    data["default"] = Convert.ToInt32(value_default);
                    data["min"] = min != null ? Convert.ToInt32(min) : int.MinValue;
                    data["max"] = max != null ? Convert.ToInt32(max) : int.MaxValue;
                    break;
                case InputType.String:
                    data["default"] = value_default as string ?? string.Empty;
                    data["min"] = min != null ? Convert.ToInt32(min) : int.MinValue;
                    data["max"] = max != null ? Convert.ToInt32(max) : int.MaxValue;
                    break;
                case InputType.Vector2:
                    data["default"] = value_default != null ? new List<object> { ((Vector2)value_default).x, ((Vector2)value_default).y } : new List<object> { 0, 0 };
                    data["min"] = min != null ? new List<object>() { ((Vector2)min).x, ((Vector2)min).y } : new List<object> { float.NegativeInfinity, float.NegativeInfinity };
                    data["max"] = max != null ? new List<object>() { ((Vector2)max).x, ((Vector2)max).y } : new List<object> { float.PositiveInfinity, float.PositiveInfinity };
                    break;
            }
        }
    }
}
