namespace SD.Game.Settings
{
    class Set_HUD_Hide : ASetting
    {
        public Set_HUD_Hide(GlobalSettings settings) : base(settings)
        { }

        public override void ChangeValue()
        {
            Settings.HUDHide = !Settings.HUDHide;
        }

        public override string GetSettingsKey()
        {
            return "HUD.Hide";
        }

        const string Key_Yes = "Settings.Key.Yes";
        const string Key_No = "Settings.Key.No";

        public override string GetValueTranslationKey()
        {
            return Settings.HUDHide ? Key_Yes : Key_No;
        }
    }
}
