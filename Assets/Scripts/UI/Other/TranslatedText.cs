using System;
using System.Collections.Generic;
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
                ChangeText(GameController.Instance.Settings.GameLanguage);
            }

            GlobalSettings.OnLanguageChange += ChangeText;
        }

        void OnDestroy()
        {
            GlobalSettings.OnLanguageChange -= ChangeText;
        }

        void OnEnable()
        {
            if (!UpdateOnEnable)
            {
                return;
            }

            ChangeText(GameController.Instance.Settings.GameLanguage);
        }

        void ChangeText(string newLanguage)
        {
            text.text = GameController.Instance.Languages.GetValue(newLanguage, key);
        }

        public string GetValue()
        {
            return GetValue(GameController.Instance.Settings.GameLanguage);
        }

        public string GetValue(string language)
        {
            return GameController.Instance.Languages.GetValue(language, key);
        }
    }
}
