using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.Game.Settings
{

    static class SettingsList
    {
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
