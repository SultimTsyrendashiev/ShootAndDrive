using UnityEngine;

namespace SD.Game.Settings
{
    class Set_Sound_MusicVolume : ASetting
    {
        public Set_Sound_MusicVolume(GlobalSettings settings) : base(settings)
        {
        }

        public override string GetSettingsKey()
        {
            return SettingsList.Setting_Key_Audio_MusicVolume;
        }

        public override float GetFloatValue()
        {
            return Settings.MusicVolume;
        }

        protected override void ChangeValue(float value)
        {
            Settings.MusicVolume = Mathf.Clamp(value, 0, 1);
        }
    }
}
