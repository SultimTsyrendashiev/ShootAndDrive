using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SD
{
    class LanguageTable
    {
        Dictionary<string, Dictionary<string, string>> languages; // (key, (language, value))

        /// <summary>
        /// Get all languages
        /// </summary>
        public string[] LanguageNames { get; private set; }

        /// <summary>
        /// Creates dictionaries and fills them
        /// </summary>
        /// <param name="values">
        /// first row must contain language names (except 0),
        /// first column must contain keys
        /// </param>
        public void Parse(string[][] values)
        {
            var firstRow = values[0];
            int languagesAmount = firstRow.Length - 1;

            // load language names
            LanguageNames = new string[languagesAmount];
            for (int j = 1; j < firstRow.Length; j++)
            {
                LanguageNames[j - 1] = firstRow[j];
            }

            // create main dictionary
            languages = new Dictionary<string, Dictionary<string, string>>(values.Length - 1);

            // foreach key
            for (int i = 1; i < values.Length; i++)
            {
                // create dictionary of languages for this key
                var ls = new Dictionary<string, string>(languagesAmount);

                for (int j = 1; j < firstRow.Length; j++) 
                {
                    // add language name,
                    // and value for current key on this language
                    ls.Add(firstRow[j], values[i][j]);
                }

                // add current key and dictionary
                languages.Add(values[i][0], ls);
            }
        }

        public string GetValue(string languageName, string key)
        {
            if (!languages.ContainsKey(key))
            {
                Debug.Log("Can't find key in language list: " + key);
                return key;
            }

            var keys = languages[key];

            if (!keys.ContainsKey(languageName))
            {
                Debug.Log("Can't find language: " + languageName + ". For key: " + key);
                return key;
            }

            return keys[languageName];
        }

        public bool Exist(string languageName)
        {
            return LanguageNames.Contains(languageName);
        }
    }
}
