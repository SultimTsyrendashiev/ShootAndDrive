using UnityEngine;
using UnityEngine.Audio;
using SD.Game.Settings;

namespace SD.UI.Menus
{
    /// <summary>
    /// Controls all settings
    /// </summary>
    class SettingsMenu : AAnimatedMenu
    {
        SettingsSystem settings;

        public void ChangeSetting(string settingName)
        {
            settings.ChangeSetting(settingName);
        }

        public string GetSettingValue(string settingName)
        {
            return settings.GetSettingValue(settingName);
        }

        public void ChangeFloatSetting(string settingName, float value)
        {
            settings.ChangeFloatSetting(settingName, value);
        }

        public float GetFloatSettingValue(string settingName)
        {
            return settings.GetFloatSettingValue(settingName);
        }

        protected override void DoInit()
        {
            settings = GameController.Instance.SettingsSystem;

            InputController.OnSettingsButton += ShowThisMenu;
        }

        protected override void DoDestroy()
        {
            InputController.OnSettingsButton -= ShowThisMenu;
        }
    }
}
