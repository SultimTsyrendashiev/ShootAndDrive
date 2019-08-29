using System;
using System.Collections.Generic;

namespace SD.Game.Settings
{
    static class SettingsList
    {
        #region all settings key
        // actually, it's not necssary to list them here,
        // but it's not comfortable to check each script

        public const string Setting_Key_Game_Language          = "Game.Language";
        public const string Setting_Key_Game_EnableSubtitles   = "Game.EnableSubtitles";
        public const string Setting_Key_Game_ShowCutscene      = "Game.ShowCutscene";
        public const string Setting_Key_Game_ShowTutorial      = "Game.ShowTutorial";

        public const string Setting_Key_HUD_Diegetic           = "HUD.Diegetic";
        public const string Setting_Key_HUD_Hide               = "HUD.Hide";
        public const string Setting_Key_HUD_ShowPauseBtn       = "HUD.ShowPauseBtn";

        public const string Setting_Key_Input_MovementType     = "Input.MovementType";
        public const string Setting_Key_Input_MoveBtnsSize     = "Input.MoveBtnsSize";
        public const string Setting_Key_Input_MoveBtnsDistance = "Input.MoveBtnsDistance";
        public const string Setting_Key_Input_LeftHanded       = "Input.LeftHanded";

        public const string Setting_Key_Perf_Preset            = "Perf.Preset";
        public const string Setting_Key_Perf_LodMult           = "Perf.LodMult";
        public const string Setting_Key_Perf_Msaa              = "Perf.Msaa";
        public const string Setting_Key_Perf_RagdollAmount     = "Perf.RagdollAmount";
        public const string Setting_Key_Perf_ResolutionMult    = "Perf.ResolutionMult";
        public const string Setting_Key_Perf_ShaderQuality     = "Perf.ShaderQuality";
        public const string Setting_Key_Perf_ShadowQuality     = "Perf.ShadowQuality";
        #endregion

        /// <summary>
        /// Get list of all registered settings
        /// </summary>
        public static List<ASetting> GetAllSettings(GlobalSettings globalSettings)
        {
            var settings = new List<ASetting>();

            settings.Add(new Set_Game_EnableCutscnene(globalSettings));
            settings.Add(new Set_Game_EnableSubtitles(globalSettings));
            settings.Add(new Set_Game_EnableTutorial(globalSettings));
            settings.Add(new Set_Game_Language(globalSettings));

            settings.Add(new Set_HUD_Diegetic(globalSettings));
            settings.Add(new Set_HUD_Hide(globalSettings));
            settings.Add(new Set_HUD_ShowPauseBtn(globalSettings));

            settings.Add(new Set_Input_MoveType(globalSettings));
            settings.Add(new Set_Input_MoveBtnsDistance(globalSettings));
            settings.Add(new Set_Input_MoveBtnsSize(globalSettings));

            settings.Add(new Set_Perf_LodMult(globalSettings));
            settings.Add(new Set_Perf_Msaa(globalSettings));
            settings.Add(new Set_Perf_Preset(globalSettings));
            settings.Add(new Set_Perf_RagdollAmount(globalSettings));
            settings.Add(new Set_Perf_ResolutionMult(globalSettings));
            settings.Add(new Set_Perf_ShaderQuality(globalSettings));
            settings.Add(new Set_Perf_ShadowQuality(globalSettings));

            return settings;
        }
    }
}
