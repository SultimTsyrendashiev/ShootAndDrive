namespace SD
{
    delegate void MovementInputTypeChange(MovementInputType type);

    /// <summary>
    /// Holds all settings for the game.
    /// Changing values in this class will not have effect,
    /// they must be set through 'SettingsSystem' which provides events for this settings
    /// </summary>
    [System.Serializable]
    class GlobalSettings
    {

        /// <summary>
        /// This language must be in language packs
        /// </summary>
        public const string DefaultLanguage = "English";

        #region game
        private string              gameLanguage;
        public string               GameLanguage
        {
            get
            {
                return gameLanguage;
            }
            set
            {
                // if language pack contain current languge
                if (GameController.Instance.Localization.Exist(gameLanguage))
                {
                    gameLanguage = value;
                }
                else
                {
                    // otherwise, to default
                    gameLanguage = DefaultLanguage;
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

        public float                InputMovementBtnsSize;
        public float                InputMovementBtnsDistance;
        #endregion

        #region hud
        public bool                 HUDShowPauseButton;
        public bool                 HUDHide;
        public bool                 HUDDiegetic;
        #endregion


        /// <summary>
        /// Sets default settings
        /// </summary>
        public void SetDefaults()
        {
            gameLanguage = DefaultLanguage;
            GameEnableSubtitles = false;
            GameShowCutscene = true;
            GameShowTutorial = true;

            SetPerformanceDefault();

            MusicVolume = 1;

            InputMovementType = MovementInputType.Joystick;
            InputMovementBtnsSize = 1;
            InputMovementBtnsDistance = 1;
            InputLeftHanded = false;

            HUDDiegetic = false;
            HUDHide = false;
            HUDShowPauseButton = true;
        }

        public void SetPerformanceLow()
        {
            PerfPreset = PerformancePreset.Low;
            PerfLODMultiplier = 1;
            PerfMsaa = 0;
            PerfRagdollAmount = 3;
            PerfResolutionMult = 1;
            PerfShaderQuality = ShaderQuality.PhysicallyBased;
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
            PerfShadowQuality = ShadowQuality.Low;
        }
    }
}
