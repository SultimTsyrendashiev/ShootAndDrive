using SD.Utils;
using UnityEngine;

namespace SD.Game
{
    [CreateAssetMenu(menuName = "Language List", order = 51)]
    class LanguageList : ScriptableObject
    {
        /// <summary>
        /// CSV table, where:
        /// - first column contains keys
        /// - other ones contains values for each language
        /// - first row contains language names (except value at [0,0])
        /// </summary>
        [SerializeField]
        TextAsset csvLanguageTable;

        public TextAsset CSVLanguageTable => csvLanguageTable;
    }
}
