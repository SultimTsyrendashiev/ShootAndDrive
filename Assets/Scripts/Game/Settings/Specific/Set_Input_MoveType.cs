namespace SD.Game.Settings
{
    class Set_Input_MoveType : ASetting
    {
        public Set_Input_MoveType(GlobalSettings settings) : base(settings)
        { }

        public override void ChangeValue()
        {
            switch (Settings.InputMovementType)
            {
                case MovementInputType.Joystick:
                    Settings.InputMovementType = MovementInputType.Buttons;
                    return;
                case MovementInputType.Buttons:
                    Settings.InputMovementType = MovementInputType.Gyroscope;
                    return;
                default:
                    Settings.InputMovementType = MovementInputType.Joystick;
                    return;
            }
        }

        public override string GetSettingsKey()
        {
            return "Input.MovementType";
        }

        const string Key_Movement_Joystick = "Settings.Key.Movement.Joystick";
        const string Key_Movement_Buttons = "Settings.Key.Movement.Buttons";
        const string Key_Movement_Gyro = "Settings.Key.Movement.Gyro";

        public override string GetValueTranslationKey()
        {
            switch (Settings.InputMovementType)
            {
                case MovementInputType.Gyroscope:
                    return Key_Movement_Gyro;
                case MovementInputType.Buttons:
                    return Key_Movement_Buttons;
                default:
                    return Key_Movement_Joystick;
            }
        }
    }
}
