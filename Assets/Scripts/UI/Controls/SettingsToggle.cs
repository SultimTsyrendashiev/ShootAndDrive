using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SD.UI.Menus;
using SD.Game.Settings;

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
            settingsMenu = FindSettingsMenu();

            text = GetComponentInChildren<Text>();

            // GlobalSettings.OnLanguageChange += UpdateTextS;
            GameController.Instance.SettingsSystem.Subscribe(SettingsList.Setting_Key_Game_Language, UpdateTextS);

            // also subscribe to this event, as it changes other settings too
            GameController.Instance.SettingsSystem.Subscribe(SettingsList.Setting_Key_Perf_Preset, UpdateTextS);

            UpdateText();
        }

        protected virtual SettingsMenu FindSettingsMenu()
        {
            return GetComponentInParent<SettingsMenu>();
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
            // GlobalSettings.OnLanguageChange -= UpdateTextS;
            GameController.Instance.SettingsSystem.Unsubscribe(SettingsList.Setting_Key_Game_Language, UpdateTextS);
            GameController.Instance.SettingsSystem.Unsubscribe(SettingsList.Setting_Key_Perf_Preset, UpdateTextS);
        }

        // update text on language change
        void UpdateTextS(GlobalSettings s) => UpdateText();
        #endregion
    }
}
