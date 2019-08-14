using SD.Utils;

namespace SD.Game
{
    class CSVLanguageTable
    {
        /// <summary>
        /// Contains values for each language and key
        /// </summary>
        public LanguageTable Languages { get; private set; }
        public string[] LanguageNames => Languages.LanguageNames;

        public CSVLanguageTable()
        {
            Languages = new LanguageTable();
        }

        public void Parse(UnityEngine.TextAsset csvLanguageTable)
        {
            string content = System.Text.Encoding.Default.GetString(csvLanguageTable.bytes);
            var values = CSVParser.Parse(content);

            Languages.Parse(values);
        }
    }
}
