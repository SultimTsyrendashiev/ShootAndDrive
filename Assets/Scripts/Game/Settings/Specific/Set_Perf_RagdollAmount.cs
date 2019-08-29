namespace SD.Game.Settings
{
    class Set_Perf_RagdollAmount : ASetting
    {
        public Set_Perf_RagdollAmount(GlobalSettings settings) : base(settings)
        { }

        protected override void ChangeValue()
        {
            switch (Settings.PerfRagdollAmount)
            {
                case 0:
                    Settings.PerfRagdollAmount = 1;
                    return;
                case 1:
                    Settings.PerfRagdollAmount = 3;
                    return;
                case 3:
                    Settings.PerfRagdollAmount = 5;
                    return;
                default:
                    Settings.PerfRagdollAmount = 0;
                    return;
            }
        }

        public override string GetSettingsKey()
        {
            return SettingsList.Setting_Key_Perf_RagdollAmount;
        }

        string Key_Amount_Zero =        "Settings.Key.Amount.Zero";
        string Key_Amount_Min =         "Settings.Key.Amount.Min";
        string Key_Amount_Default =     "Settings.Key.Amount.Default";
        string Key_Amount_Max =         "Settings.Key.Amount.Max";

        public override string GetValueTranslationKey()
        {
            switch (Settings.PerfRagdollAmount)
            {
                case 0:
                    return Key_Amount_Zero;
                case 1:
                    return Key_Amount_Min;
                case 3:
                    return Key_Amount_Default;
                default:
                    return Key_Amount_Max;
            }
        }
    }
}
