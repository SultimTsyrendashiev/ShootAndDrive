using UnityEngine;

namespace SD.Game.Settings
{
    class Set_Sound_SoundVolume : ASetting
    {
        public Set_Sound_SoundVolume(GlobalSettings settings) : base(settings)
        {
        }

        public override string GetSettingsKey()
        {
            return SettingsList.Setting_Key_Audio_SoundVolume;
        }

        public override float GetFloatValue()
        {
            return Settings.SoundVolume;
        }

        protected override void ChangeValue(float value)
        {
            Settings.SoundVolume = Mathf.Clamp(value, 0, 1);
        }
    }
}
