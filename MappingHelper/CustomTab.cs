using ADOFAI;
using System;
using System.Collections.Generic;

using UnityEngine;

namespace MappingHelper
{
    internal class CustomTab
    {
        internal Sprite icon;
        internal int type;
        internal string name;
        internal string title;
        internal Type page;
        internal int index;
        internal List<Dictionary<string, object>> properties;
        internal Action onFocused;
        internal Action onUnFocused;
        internal Func<LevelEvent, string, object, object, bool> onChange;
        internal bool saveSetting;
        internal CustomTab(){}
    }
}
