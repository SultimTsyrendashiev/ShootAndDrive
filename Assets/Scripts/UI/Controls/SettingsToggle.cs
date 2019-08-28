using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SD.UI.Menus;

namespace SD.UI.Controls
{
    /// <summary>
    /// Attach this script to button to change settings.
    /// 'SettingsMenu' must be contained by any parent,
    /// also, 'Text' component must be on any child object
    /// </summary>
    class SettingsToggle : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        string settingName;

        SettingsMenu settingsMenu;
        Text text;

        void UpdateText()
        {
            if (settingsMenu != null && text != null)
            {
                text.text = settingsMenu.GetSettingValue(settingName);
            }
        }

        void Start()
        {
            settingsMenu = GetComponentInParent<SettingsMenu>();
            text = GetComponentInChildren<Text>();

            GlobalSettings.OnLanguageChange += UpdateTextS;

            UpdateText();
        }

        void OnEnable()
        {
            UpdateText();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            settingsMenu.ChangeSetting(settingName);
            UpdateText();
        }

        #region on language change
        void OnDestroy()
        {
            GlobalSettings.OnLanguageChange -= UpdateTextS;
        }

        void UpdateTextS(string s) => UpdateText();
        #endregion
    }
}
