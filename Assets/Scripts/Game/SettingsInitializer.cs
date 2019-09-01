using UnityEngine;

namespace SD.Game.Settings
{
    static class SettingsInitializer
    {
        public static void Init(SettingsSystem settingsSystem, GlobalSettings initSettings, AudioManager audioManager, AudioSettingsHandler audioSettingsHandler, TimeController timeController)
        {
            audioManager.SetTimeContoller(timeController);

            var ush = Object.FindObjectOfType<UnitySettingsHandler>();
            Debug.Assert(ush != null, "Can't find UnitySettingsHandler");

            audioSettingsHandler.Init(audioManager, settingsSystem, initSettings);
        }
    }
}
