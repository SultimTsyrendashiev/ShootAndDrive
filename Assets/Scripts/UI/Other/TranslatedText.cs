using System;
using System.Collections.Generic;
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

        Text text;

        public string TranslationKey => key;

        void Start()
        {
            text = GetComponentInChildren<Text>();

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
            text.text = GameController.Instance.Localization.GetValue(newLanguage, key);
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
