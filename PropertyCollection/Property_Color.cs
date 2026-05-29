using System.Collections.Generic;
using UnityEngine;

namespace MappingHelper.PropertyCollection
{
    public class Property_Color : Property
    {
        private readonly string value_default;
        private readonly bool usesAlpha;

        public Property_Color(string name, Color? value_default = null, bool usesAlpha = true, string key = null, bool canBeDisabled = false, bool startEnabled = false, Dictionary<string, string> enableIf = null, Dictionary<string, string> disableIf = null)
            : base(name, key, canBeDisabled, startEnabled, enableIf, disableIf)
        {
            data["type"] = "Color";
            data["default"] = ToHex(value_default.GetValueOrDefault(Color.white), usesAlpha);
            data["usesAlpha"] = usesAlpha;
        }

        private static string ToHex(Color c, bool alpha)
        {
            return (alpha
                ? string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", ToByte(c.r), ToByte(c.g), ToByte(c.b), ToByte(c.a))
                : string.Format("{0:X2}{1:X2}{2:X2}", ToByte(c.r), ToByte(c.g), ToByte(c.b))).ToLower();
        }

        private static byte ToByte(float f)
        {
            return (byte)(Mathf.Clamp01(f) * 255f);
        }
    }
}
