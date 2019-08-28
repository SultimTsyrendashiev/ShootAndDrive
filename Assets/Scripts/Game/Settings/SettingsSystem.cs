using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }

        public void ChangeSetting(string settingName)
        {
            if (allSettings.TryGetValue(settingName, out ASetting setting))
            {
                setting.ChangeValue();
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
    }
}
