namespace SD.Game.Settings
{
    class Set_Game_Language : ASetting
    {
        public Set_Game_Language(GlobalSettings settings) : base(settings)
        { }

        public override void ChangeValue()
        {
            var ls = GameController.Instance.Languages.LanguageNames;

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
            Settings.ResetLanguage();
        }

        public override string GetSettingsKey()
        {
            return "Game.Language";
        }

        public override string GetValueTranslationKey()
        {
            return "Language.Name";
        }
    }
}
