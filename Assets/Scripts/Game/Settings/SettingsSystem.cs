using System;
using System.Collections.Generic;

namespace SD.Game.Settings
{
    class SettingsSystem
    {
        Dictionary<string, ASetting> allSettings;

        public SettingsSystem(GlobalSettings globalSettings)
        {
            allSettings = new Dictionary<string, ASetting>();

            var ss = SettingsList.GetAllSettings(globalSettings);

            foreach (var s in ss)
            {
                allSettings.Add(s.GetSettingsKey(), s);
            }

            foreach (var s in ss)
            {
                s.Init(this);
            }
        }

        public void ChangeSetting(string settingName)
        {
            if (allSettings.TryGetValue(settingName, out ASetting setting))
            {
                setting.ChangeSetting();
            }
        }

        public string GetSettingValue(string settingName)
        {
            if (allSettings.TryGetValue(settingName, out ASetting setting))
            {
                return setting.GetTranslatedValue();
            }

            return string.Empty;
        }

        /// <summary>
        /// Subscribe to setting change event
        /// </summary>
        /// <param name="settingName">setting's identifier</param>
        /// <param name="action">this method called when setting was changed</param>
        /// <returns>true, if setting exist</returns>
        public bool Subscribe(string settingName, Action<GlobalSettings> action)
        {
            if (allSettings.TryGetValue(settingName, out ASetting setting))
            {
                setting.OnSettingUpdate += action;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Unsubscribe from setting change event
        /// </summary>
        public bool Unsubscribe(string settingName, Action<GlobalSettings> action)
        {
            if (allSettings.TryGetValue(settingName, out ASetting setting))
            {
                setting.OnSettingUpdate -= action;

                return true;
            }

            return false;
        }
    }
}
