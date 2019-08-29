namespace SD.Game.Settings
{
    class Set_HUD_ShowPauseBtn : ASetting
    {
        public Set_HUD_ShowPauseBtn(GlobalSettings settings) : base(settings)
        { }

        protected override void ChangeValue()
        {
            Settings.HUDShowPauseButton = !Settings.HUDShowPauseButton;
        }

        public override string GetSettingsKey()
        {
            return SettingsList.Setting_Key_HUD_ShowPauseBtn;
        }

        const string Key_Yes = "Settings.Key.Yes";
        const string Key_No = "Settings.Key.No";

        public override string GetValueTranslationKey()
        {
            return Settings.HUDShowPauseButton ? Key_Yes : Key_No;
        }
    }
}
