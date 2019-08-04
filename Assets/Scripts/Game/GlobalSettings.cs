namespace SD
{
    /// <summary>
    /// Holds all settings for the game
    /// </summary>
    [System.Serializable]
    class GlobalSettings
    {
        public string               GameLanguage;
        public bool                 GameEnableSubtitles;
        public bool                 GameShowCutscene;
        public bool                 GameShowTutorial;

        // performance
        public int                  PerfMsaa;
        public float                PerfResolutionMult;
        public int                  PerfRagdollAmount;
        public float                PerfLODMultiplier;
        public ShaderQuality        PerfShaderQuality;
        public ShadowQuality        PerfShadowQuality;

        // sound
        public float                MusicVolume;

        // input
        public MovementInputType    InputMovementType;
        public bool                 InputLeftHanded;

        // hud
        public bool                 HUDShowPauseButton;
        public bool                 HUDHiding;
        public bool                 HUDDiegetic;


        /// <summary>
        /// Sets default settings
        /// </summary>
        public void SetDefaults()
        {
            GameLanguage = "English";
            GameEnableSubtitles = false;
            GameShowCutscene = true;
            GameShowTutorial = true;

            SetPerformanceDefault();

            MusicVolume = 1;

            InputMovementType = MovementInputType.Joystick;
            InputLeftHanded = false;

            HUDDiegetic = false;
            HUDHiding = false;
            HUDShowPauseButton = true;
        }

        public void SetPerformanceLow()
        {
            PerfLODMultiplier = 0.7f;
            PerfMsaa = 0;
            PerfRagdollAmount = 1;
            PerfResolutionMult = 1;
            PerfShaderQuality = ShaderQuality.Performance;
            PerfShadowQuality = ShadowQuality.None;
        }


        public void SetPerformanceDefault()
        {
            PerfLODMultiplier = 1;
            PerfMsaa = 4;
            PerfRagdollAmount = 5;
            PerfResolutionMult = 1;
            PerfShaderQuality = ShaderQuality.PhysicallyBased;
            PerfShadowQuality = ShadowQuality.Medium;
        }
    }
}
