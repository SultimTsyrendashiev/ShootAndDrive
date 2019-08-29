namespace SD.Game.Settings
{
    class Set_Game_Language : ASetting
    {
        public Set_Game_Language(GlobalSettings settings) : base(settings)
        { }

        protected override void ChangeValue()
        {
            var ls = GameController.Instance.Localization.LanguageNames;

            for (int i = 0; i < ls.Length; i++)
            {
                if (ls[i] == Settings.GameLanguage)
                {
                    int next = i + 1 < ls.Length ? i + 1 : 0;
                    Settings.GameLanguage = ls[next];

                    return;
                }
            }

            // default
            Settings.GameLanguage = GlobalSettings.DefaultLanguage;
        }

        public override string GetSettingsKey()
        {
            return SettingsList.Setting_Key_Game_Language;
        }

        public override string GetValueTranslationKey()
        {
            return "Language.Name";
        }
    }
}
