namespace SD.Game.Settings
{
    class Set_Perf_LodMult : ASetting
    {
        public Set_Perf_LodMult(GlobalSettings settings) : base(settings)
        { }

        protected override void ChangeValue()
        {
            switch (Settings.PerfLODMultiplier)
            {
                case 1.0f:
                    Settings.PerfLODMultiplier = 1.25f;
                    return;
                case 1.25f:
                    Settings.PerfLODMultiplier = 1.5f;
                    return;
                default:
                    Settings.PerfLODMultiplier = 1;
                    return;
            }
        }

        public override string GetSettingsKey()
        {
            return SettingsList.Setting_Key_Perf_LodMult;
        }

        const string Key_1x     = "Settings.Key.1x";
        const string Key_125x   = "Settings.Key.1.25x";
        const string Key_15x    = "Settings.Key.1.5x";

        public override string GetValueTranslationKey()
        {
            switch (Settings.PerfLODMultiplier)
            {
                case 1.25f:
                    return Key_125x;
                case 1.5f:
                    return Key_15x;
                default:
                    return Key_1x;
            }
        }
    }
}
