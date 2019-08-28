namespace SD.Game.Settings
{
    class Set_Game_EnableSubtitles : ASetting
    {
        public Set_Game_EnableSubtitles(GlobalSettings settings) : base(settings)
        { }

        public override void ChangeValue()
        {
            Settings.GameEnableSubtitles = !Settings.GameEnableSubtitles;
        }

        public override string GetSettingsKey()
        {
            return "Game.EnableSubtitles";
        }

        const string Key_Yes = "Settings.Key.Yes";
        const string Key_No = "Settings.Key.No";

        public override string GetValueTranslationKey()
        {
            return Settings.GameEnableSubtitles ? Key_Yes : Key_No;
        }
    }
}
