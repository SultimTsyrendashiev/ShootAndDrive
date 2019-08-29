namespace SD.Game.Settings
{
    class Set_Input_MoveBtnsDistance : ASetting
    {
        public Set_Input_MoveBtnsDistance(GlobalSettings settings) : base(settings)
        { }

        public override string GetSettingsKey()
        {
            return SettingsList.Setting_Key_Input_MoveBtnsDistance;
        }

        const string Key_Small = "Settings.Key.MovementBtns.Distance.Small";
        const string Key_Default = "Settings.Key.MovementBtns.Distance.Default";
        const string Key_Big = "Settings.Key.MovementBtns.Distance.Big";

        const float Value_Default = 1.0f;
        const float Value_Small = 0.0f;
        const float Value_Big = 3.0f;

        protected override void ChangeValue()
        {
            switch (Settings.InputMovementBtnsDistance)
            {
                case Value_Default:
                    Settings.InputMovementBtnsDistance = Value_Big;
                    return;
                case Value_Big:
                    Settings.InputMovementBtnsDistance = Value_Small;
                    return;
                default:
                    Settings.InputMovementBtnsDistance = Value_Default;
                    return;
            }
        }

        public override string GetValueTranslationKey()
        {
            switch (Settings.InputMovementBtnsDistance)
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
