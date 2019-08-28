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

        protected override void DoInit()
        {
            settings = GameController.Instance.SettingsSystem;

            InputController.OnSettingsButton += ShowThisMenu;
        }

        protected override void DoDestroy()
        {
            InputController.OnSettingsButton -= ShowThisMenu;
        }

        public void Change_Sound_MusicVolume(float volume)
        {
            volume = Mathf.Clamp(volume, 0, 1);
            // AudioMixer::SetFloat( , volume);
        }
    }
}
