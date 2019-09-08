using SD.Game.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace SD.UI.CutsceneSubtitles
{
    class CutsceneSubtitleText : MonoBehaviour
    {
        string translationKey;

        public Text Text { get; private set; }

        void Start()
        {
            Text = GetComponentInChildren<Text>();
            Debug.Assert(Text != null, "Can't find text component", this);

            GameController.Instance.SettingsSystem.Subscribe(SettingsList.Setting_Key_Game_Language, UpdateLanguageSettings);
        }

        void OnDestroy()
        {
            GameController.Instance.SettingsSystem.Subscribe(SettingsList.Setting_Key_Game_Language, UpdateLanguageSettings);
        }


        /// <summary>
        /// Set translation key and update text in text component
        /// </summary>
        public void SetTranslationKey(string key)
        {
            translationKey = key;
            UpdateText();
        }

        /// <summary>
        /// Reset text in text component
        /// </summary>
        public void Clear()
        {
            translationKey = string.Empty;

            if (Text != null)
            {
                Text.text = string.Empty;
            }
            else
            {
                GetComponentInChildren<Text>().text = string.Empty;
            }
        }

        void UpdateLanguageSettings(GlobalSettings s) => UpdateLanguage(s.GameLanguage);

        void UpdateLanguage(string newLanguage)
        {
            if (GameController.Instance == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(translationKey))
            {
                Text.text = string.Empty;
                return;
            }

            SetText(GameController.Instance.Localization.GetValue(newLanguage, translationKey));
        }

        void UpdateText()
        {
            // if in timeline editor
            if (GameController.Instance == null)
            {
                GetComponentInChildren<Text>().text = translationKey;
                return;
            }

            if (string.IsNullOrEmpty(translationKey))
            {
                Text.text = string.Empty;
                return;
            }

            SetText(GameController.Instance.Localization.GetValue(
                GameController.Instance.Settings.GameLanguage, translationKey));
        }

        /// <summary>
        /// Set text to text component
        /// </summary>
        /// <param name="localizedText">translated text</param>
        protected virtual void SetText(string localizedText)
        {
            Text.text = localizedText;
        }
    }
}
