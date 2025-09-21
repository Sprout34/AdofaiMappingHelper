using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MappingHelper
{
    internal class Localization
    {
        Dictionary<string, Dictionary<SystemLanguage,string>> localization;

        public Localization(string jsonFile)
        {
            string jsonString = File.ReadAllText(jsonFile);

            var raw = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonString);
            localization = new Dictionary<string, Dictionary<SystemLanguage, string>>();

            foreach (var outer in raw)
            {
                var innerDict = new Dictionary<SystemLanguage, string>();

                foreach (var inner in outer.Value)
                {
                    if (Enum.TryParse(inner.Key, out SystemLanguage lang))
                    {
                        innerDict[lang] = inner.Value;
                    }
                    else
                    {
                        Main.Logger.Log($"警告: 未识别语言 {inner.Key}");
                    }
                }

                localization[outer.Key] = innerDict;
            }
        }

        public bool Get(string key, out string value, Dictionary<string, object> parameters = null)
        {
            value = null;

            if (localization.TryGetValue(key, out var languageDict))
            {
                SystemLanguage currentLanguage = RDString.language;
                if (!languageDict.TryGetValue(currentLanguage, out value))
                {
                    languageDict.TryGetValue(SystemLanguage.English, out value);
                }
            }
            return value != null;
        }

        public string GetValue(string key)
        {
            if (localization.TryGetValue(key, out var languageDict))
            {
                SystemLanguage currentLanguage = RDString.language;
                if (languageDict.TryGetValue(currentLanguage, out var result))
                {
                    return result;
                }
                if (languageDict.TryGetValue(SystemLanguage.English, out result))
                {
                    return result;
                }
            }
            return null;
        }
    }
}
