namespace SD.Game.Settings
{
    class Set_Input_MoveBtnsSize : ASetting
    {
        public Set_Input_MoveBtnsSize(GlobalSettings settings) : base(settings)
        { }

        public override string GetSettingsKey()
        {
            return SettingsList.Setting_Key_Input_MoveBtnsSize;
        }

        const string Key_Small = "Settings.Key.MovementBtns.Size.Small";
        const string Key_Default = "Settings.Key.MovementBtns.Size.Default";
        const string Key_Big = "Settings.Key.MovementBtns.Size.Big";

        const float Value_Default = 1.0f;
        const float Value_Small = 0.75f;
        const float Value_Big = 1.5f;

        protected override void ChangeValue()
        {
            switch (Settings.InputMovementBtnsSize)
            {
                case Value_Default:
                    Settings.InputMovementBtnsSize = Value_Big;
                    return;
                case Value_Big:
                    Settings.InputMovementBtnsSize = Value_Small;
                    return;
                default:
                    Settings.InputMovementBtnsSize = Value_Default;
                    return;
            }
        }

        public override string GetValueTranslationKey()
        {
            switch (Settings.InputMovementBtnsSize)
            {
                case Value_Small:
                    return Key_Small;
                case Value_Big:
                    return Key_Big;
                default:
                    return Key_Default;
            }
        }
    }
}
