using GDMiniJSON;
using System.Collections.Generic;

namespace MappingHelper.PropertyCollection
{
    public class Property_Json : Property
    {
        private readonly Dictionary<string, object> json;

        public Property_Json(Dictionary<string, object> json)
        {
            this.json = json;
        }

        public Property_Json(string json)
        {
            this.json = Json.Deserialize(json) as Dictionary<string, object>;
        }

        public override Dictionary<string, object> ToData()
        {
            return json;
        }
    }
}
