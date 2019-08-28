namespace SD.Game.Settings
{
    class Set_Perf_ResolutionMult : ASetting
    {
        public Set_Perf_ResolutionMult(GlobalSettings settings) : base(settings)
        { }

        public override void ChangeValue()
        {
            switch (Settings.PerfResolutionMult)
            {
                case 0.5f:
                    Settings.PerfResolutionMult = 0.75f;
                    return;
                case 0.75f:
                    Settings.PerfResolutionMult = 0.9f;
                    return;
                case 1.0f:
                    Settings.PerfResolutionMult = 0.5f;
                    return;
                default:
                    Settings.PerfResolutionMult = 1;
                    return;
            }
        }

        public override string GetSettingsKey()
        {
            return "Perf.ResolutionMult";
        }

        const string Key_05x =      "Settings.Key.0.5x";
        const string Key_075x =     "Settings.Key.0.75x";
        const string Key_09x =      "Settings.Key.0.9x";
        const string Key_1x =       "Settings.Key.1x";

        public override string GetValueTranslationKey()
        {
            switch (Settings.PerfResolutionMult)
            {
                case 0.5f:
                    return Key_05x;
                case 0.75f:
                    return Key_075x;
                case 0.9f:
                    return Key_09x;
                default:
                    return Key_1x;
            }
        }
    }
}
