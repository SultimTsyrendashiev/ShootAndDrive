namespace SD.Game.Settings
{
    class Set_Perf_ShaderQuality : ASetting
    {
        public Set_Perf_ShaderQuality(GlobalSettings settings) : base(settings)
        { }

        protected override void ChangeValue()
        {
            switch (Settings.PerfShaderQuality)
            {
                case ShaderQuality.Performance:
                    Settings.PerfShaderQuality = ShaderQuality.PhysicallyBased;
                    return;
                default:
                    Settings.PerfShaderQuality = ShaderQuality.Performance;
                    return;
            }
        }

        public override string GetSettingsKey()
        {
            return SettingsList.Setting_Key_Perf_ShaderQuality;
        }

        const string Key_Shader_Perf = "Settings.Key.Shader.Performance";
        const string Key_Shader_PB = "Settings.Key.Shader.PB";

        public override string GetValueTranslationKey()
        {
            switch (Settings.PerfShaderQuality)
            {
                case ShaderQuality.PhysicallyBased:
                    return Key_Shader_PB;
                default:
                    return Key_Shader_Perf;
            }
        }
    }
}
