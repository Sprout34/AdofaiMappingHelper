using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MappingHelper.Utils
{
    public static class RDStringEx
    {
        public static string GetOrOrigin(string key, Dictionary<string, object> parameters = null)
        {
            string result = RDString.GetWithCheck(key, out bool exists, parameters);
            return exists ? result : key;
        }
    }
}
