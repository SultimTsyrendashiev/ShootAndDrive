namespace SD
{
    /// <summary>
    /// Holds all settings for the game
    /// </summary>
    [System.Serializable]
    class GlobalSettings
    {
        public const string DefaultLanguage = "English";

        /// <summary>
        /// This event is called, when new language is set.
        /// New language's name is a parameter.
        /// </summary>
        public static event System.Action<string> OnLanguageChange;

        string gameLanguage;
        public string               GameLanguage
        {
            get
            {
                // if language pack doesn't contain current languge
                if (!GameController.Instance.Languages.Exist(gameLanguage))
                {
                    // reset it
                    gameLanguage = DefaultLanguage;
                }

                return gameLanguage;
            }
            set
            {
                if (value != gameLanguage)
                {
                    gameLanguage = value;
                    OnLanguageChange(value);
                }
            }
        }
        public bool                 GameEnableSubtitles;
        public bool                 GameShowCutscene;
        public bool                 GameShowTutorial;

        // performance
        public PerformancePreset    PerfPreset;
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
            ResetLanguage();
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
            PerfPreset = PerformancePreset.Low;
            PerfLODMultiplier = 1;
            PerfMsaa = 0;
            PerfRagdollAmount = 1;
            PerfResolutionMult = 1;
            PerfShaderQuality = ShaderQuality.Performance;
            PerfShadowQuality = ShadowQuality.None;
        }


        public void SetPerformanceDefault()
        {
            PerfPreset = PerformancePreset.Default;
            PerfLODMultiplier = 1;
            PerfMsaa = 0;
            PerfRagdollAmount = 3;
            PerfResolutionMult = 1;
            PerfShaderQuality = ShaderQuality.PhysicallyBased;
            PerfShadowQuality = ShadowQuality.Medium;
        }

        public void ResetLanguage()
        {
            GameLanguage = DefaultLanguage;
        }
    }
}
