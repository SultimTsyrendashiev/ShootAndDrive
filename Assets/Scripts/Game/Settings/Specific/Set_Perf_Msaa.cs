namespace SD.Game.Settings
{
    class Set_Perf_Msaa : ASetting
    {
        public Set_Perf_Msaa(GlobalSettings settings) : base(settings)
        { }

        public override void ChangeValue()
        {
            switch (Settings.PerfMsaa)
            {
                case 0:
                    Settings.PerfMsaa = 2;
                    return;
                case 2:
                    Settings.PerfMsaa = 4;
                    return;
                default:
                    Settings.PerfMsaa = 0;
                    return;
            }
        }

        public override string GetSettingsKey()
        {
            return "Perf.Msaa";
        }

        const string Key_2x = "Settings.Key.2x";
        const string Key_4x = "Settings.Key.4x";
        const string Key_No = "Settings.Key.No";

        public override string GetValueTranslationKey()
        {
            switch (Settings.PerfMsaa)
            {
                case 2:
                    return Key_2x;
                case 4:
                    return Key_4x;
                default:
                    return Key_No;
            }
        }
    }
}
