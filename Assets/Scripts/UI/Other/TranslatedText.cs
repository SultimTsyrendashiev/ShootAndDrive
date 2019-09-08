using SD.Game.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace SD.UI
{
    class TranslatedText : MonoBehaviour
    {
        bool UpdateOnEnable = false;

        /// <summary>
        /// Key in language table
        /// </summary>
        [SerializeField]
        string key;

        [SerializeField]
        bool updateTextOnStart = true;

        public string TranslationKey => key;

        public Text Text { get; private set; }

        void Start()
        {
            Text = GetComponentInChildren<Text>();
            Debug.Assert(Text != null, "Can't find text component", this);

            if (updateTextOnStart)
            {
                UpdateText(GameController.Instance.Settings.GameLanguage);
            }

            // GlobalSettings.OnLanguageChange += UpdateText;
            GameController.Instance.SettingsSystem.Subscribe(SettingsList.Setting_Key_Game_Language, UpdateTextS);
        }

        void OnDestroy()
        {
            // GlobalSettings.OnLanguageChange -= UpdateText;
            GameController.Instance.SettingsSystem.Subscribe(SettingsList.Setting_Key_Game_Language, UpdateTextS);
        }

        void OnEnable()
        {
            if (!UpdateOnEnable)
            {
                return;
            }

            UpdateText(GameController.Instance.Settings.GameLanguage);
        }

        void UpdateText(string newLanguage)
        {
            SetText(GameController.Instance.Localization.GetValue(newLanguage, key));
        }

        /// <summary>
        /// Set string to text component.
        /// Override this method to add effects (f.e. repalacing '\n' with d)
        /// </summary>
        protected virtual void SetText(string localizedText)
        {
            Text.text = localizedText;
        }

        public string GetValue()
        {
            return GetValue(GameController.Instance.Settings.GameLanguage);
        }

        public string GetValue(string language)
        {
            return GameController.Instance.Localization.GetValue(language, key);
        }

        void UpdateTextS(GlobalSettings s) => UpdateText(s.GameLanguage);
    }
}
