namespace SD.Game.Settings
{
    class Set_Perf_ShadowQuality : ASetting
    {
        public Set_Perf_ShadowQuality(GlobalSettings settings) : base(settings)
        { }

        protected override void ChangeValue()
        {
            int a = (int)Settings.PerfShadowQuality;

            int max = System.Enum.GetValues(typeof(ShadowQuality)).Length;

            // disable ultra quality
            max -= 1;

            a = a + 1 < max ? a + 1 : 0;

            Settings.PerfShadowQuality = (ShadowQuality)a;
        }

        public override string GetSettingsKey()
        {
            return SettingsList.Setting_Key_Perf_ShadowQuality;
        }

        const string Key_Shadow_No = "Settings.Key.Shadow.No";
        const string Key_Shadow_Low = "Settings.Key.Shadow.Low";
        const string Key_Shadow_Medium = "Settings.Key.Shadow.Medium";
        const string Key_Shadow_High = "Settings.Key.Shadow.High";
        const string Key_Shadow_Ultra = "Settings.Key.Shadow.Ultra";

        public override string GetValueTranslationKey()
        {
            switch (Settings.PerfShadowQuality)
            {
                case ShadowQuality.Low: return Key_Shadow_Low;
                case ShadowQuality.Medium: return Key_Shadow_Medium;
                case ShadowQuality.High: return Key_Shadow_High;
                case ShadowQuality.Ultra: return Key_Shadow_Ultra;
                default: return Key_Shadow_No;
            }
        }
    }
}
