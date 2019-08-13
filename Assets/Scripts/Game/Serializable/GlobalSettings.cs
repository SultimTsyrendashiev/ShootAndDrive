namespace SD
{
    /// <summary>
    /// Holds all settings for the game
    /// </summary>
    [System.Serializable]
    class GlobalSettings
    {
        /// <summary>
        /// This language must be in language packs
        /// </summary>
        public const string DefaultLanguage = "English";

        /// <summary>
        /// This event is called, when new language is set.
        /// New language's name is a parameter.
        /// </summary>
        public static event System.Action<string> OnLanguageChange;

        #region game
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
        #endregion

        #region performance
        public PerformancePreset    PerfPreset;
        public int                  PerfMsaa;
        public float                PerfResolutionMult;
        public int                  PerfRagdollAmount;
        public float                PerfLODMultiplier;
        public ShaderQuality        PerfShaderQuality;
        public ShadowQuality        PerfShadowQuality;
        #endregion

        #region sound
        public float                MusicVolume;
        #endregion

        #region input
        public MovementInputType    InputMovementType;
        public bool                 InputLeftHanded;
        #endregion

        #region hud
        public bool                 HUDShowPauseButton;
        public bool                 HUDHiding;
        public bool                 HUDDiegetic;
        #endregion


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
