using System;
using System.Collections.Generic;
using UnityEngine;
using SD.Game;

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

        UnityEngine.UI.Text text;

        void Start()
        {
            text = GetComponent<UnityEngine.UI.Text>();

            ChangeText(GameController.Instance.Settings.GameLanguage);
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
    }
}
