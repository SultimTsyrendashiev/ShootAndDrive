namespace SD.Game.Settings
{
    class Set_Game_EnableCutscnene : ASetting
    {
        public Set_Game_EnableCutscnene(GlobalSettings settings) : base(settings)
        { }

        protected override void ChangeValue()
        {
            Settings.GameShowCutscene = !Settings.GameShowCutscene;
        }

        public override string GetSettingsKey()
        {
            return SettingsList.Setting_Key_Game_ShowCutscene;
        }

        const string Key_Yes = "Settings.Key.Yes";
        const string Key_No = "Settings.Key.No";

        public override string GetValueTranslationKey()
        {
            return Settings.GameShowCutscene ? Key_Yes : Key_No;
        }
    }
}
