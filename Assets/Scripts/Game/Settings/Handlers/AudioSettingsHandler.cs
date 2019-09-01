using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.Game.Settings
{
    class AudioSettingsHandler
    {
        AudioManager audioManager;
        SettingsSystem settingsSystem;

        public void Init(AudioManager audioManager, SettingsSystem settingsSystem, GlobalSettings initSettings)
        {
            this.audioManager = audioManager;
            this.settingsSystem = settingsSystem;

            // sound
            settingsSystem.Subscribe(SettingsList.Setting_Key_Audio_MusicVolume, SetMusicVolume);
            settingsSystem.Subscribe(SettingsList.Setting_Key_Audio_SoundVolume, SetSoundVolume);

            // init values
            SetMusicVolume(initSettings);
            SetSoundVolume(initSettings);
        }

        ~AudioSettingsHandler()
        {
            settingsSystem.Unsubscribe(SettingsList.Setting_Key_Audio_MusicVolume, SetMusicVolume);
            settingsSystem.Unsubscribe(SettingsList.Setting_Key_Audio_SoundVolume, SetSoundVolume);
        }

        void SetMusicVolume(GlobalSettings settings)
        {
            audioManager?.SetMusicVolume(settings.MusicVolume);
        }

        void SetSoundVolume(GlobalSettings settings)
        {
            audioManager?.SetSoundVolume(settings.SoundVolume);
        }
    }
}
