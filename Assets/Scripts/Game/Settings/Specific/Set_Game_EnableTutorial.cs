namespace SD.Game.Settings
{
    class Set_Game_EnableTutorial : ASetting
    {
        public Set_Game_EnableTutorial(GlobalSettings settings) : base(settings)
        { }

        protected override void ChangeValue()
        {
            Settings.GameShowTutorial = !Settings.GameShowTutorial;
        }

        public override string GetSettingsKey()
        {
            return SettingsList.Setting_Key_Game_ShowTutorial;
        }

        const string Key_Yes = "Settings.Key.Yes";
        const string Key_No = "Settings.Key.No";

        public override string GetValueTranslationKey()
        {
            return Settings.GameShowTutorial ? Key_Yes : Key_No;
        }
    }
}
