namespace SD.Game.Settings
{
    class Set_Perf_Preset : ASetting
    {
        public Set_Perf_Preset(GlobalSettings settings) : base(settings)
        { }

        protected override void ChangeValue()
        {
            switch (Settings.PerfPreset)
            {
                case PerformancePreset.Default:
                    Settings.SetPerformanceLow();
                    return;
                default:
                    Settings.SetPerformanceDefault();
                    return;
            }
        }

        public override string GetSettingsKey()
        {
            return SettingsList.Setting_Key_Perf_Preset;
        }

        const string Key_PerfPreset_Low = "Settings.Key.PerfPreset.Low";
        const string Key_PerfPreset_Default = "Settings.Key.PerfPreset.Default";

        public override string GetValueTranslationKey()
        {
            switch (Settings.PerfPreset)
            {
                case PerformancePreset.Low:
                    return Key_PerfPreset_Low;
                default:
                    return Key_PerfPreset_Default;
            }
        }
    }
}
