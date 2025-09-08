using System.Collections.Generic;
using System.Linq;

namespace MappingHelper.PropertyCollection
{
    public abstract class Property
    {
        protected readonly string name;
        protected readonly string key;
        protected readonly bool canBeDisabled;
        protected readonly bool startEnabled;
        protected readonly List<object> enableIf;
        protected readonly List<object> disableIf;
        protected readonly Dictionary<string, object> data;

        internal Property() {
        }

        public Property(string name, string key = null, bool canBeDisabled = false, bool startEnabled = false, Dictionary<string, string> enableIf = null, Dictionary<string, string> disableIf = null)
        {
            this.name = name;
            this.key = key;
            this.canBeDisabled = canBeDisabled;
            this.startEnabled = startEnabled;
            this.enableIf = enableIf?.SelectMany(pair => new object[] { pair.Key, pair.Value }).ToList() ?? new List<object>();
            this.disableIf = disableIf?.SelectMany(pair => new object[] { pair.Key, pair.Value }).ToList() ?? new List<object>();
            data = new Dictionary<string, object>();
        }

        public Property With(string key, object value)
        {
            data[key] = value;
            return this;
        }

        public virtual Dictionary<string, object> ToData()
        {
            var data = new Dictionary<string, object>()
            {
                { "name", name },
                { "canBeDisabled", canBeDisabled },
                { "startEnabled", startEnabled },
                { "enableIf", enableIf },
                { "disableIf", disableIf },
                { "key", key }
            };
            foreach (var pair in this.data)
                data.Add(pair.Key, pair.Value);
            return data;
        }
    }
}
