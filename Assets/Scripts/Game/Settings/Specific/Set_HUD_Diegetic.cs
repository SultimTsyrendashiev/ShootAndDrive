namespace SD.Game.Settings
{
    class Set_HUD_Diegetic : ASetting
    {
        public Set_HUD_Diegetic(GlobalSettings settings) : base(settings)
        { }

        public override void ChangeValue()
        {
            Settings.HUDDiegetic = !Settings.HUDDiegetic;
        }

        public override string GetSettingsKey()
        {
            return "HUD.Diegetic";
        }

        const string Key_Yes = "Settings.Key.Yes";
        const string Key_No = "Settings.Key.No";

        public override string GetValueTranslationKey()
        {
            return Settings.HUDDiegetic ? Key_Yes : Key_No;
        }
    }
}
