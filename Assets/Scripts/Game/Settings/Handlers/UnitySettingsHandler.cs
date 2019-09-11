using System;
using UnityEngine;
using UnityEngine.Rendering;
using URPA = UnityEngine.Rendering.LWRP.LightweightRenderPipelineAsset;

namespace SD.Game.Settings
{
    /// <summary>
    /// Handles all settings that depends on Unity
    /// </summary>
    class UnitySettingsHandler : MonoBehaviour
    {
        const float DefaultShadowDistance = 70;
        const float CutsceneShadowDistance = 10;

        [SerializeField]
        URPA shadowsNone;
        [SerializeField]
        URPA shadowsLow;
        [SerializeField]
        URPA shadowsMedium;
        [SerializeField]
        URPA shadowsHigh;

        URPA currentPipelineAsset;

        SettingsSystem settingsSystem;


        public void Init(SettingsSystem settingsSystem, GlobalSettings initSettings)
        {
            if (GraphicsSettings.renderPipelineAsset != null)
            {
                GraphicsSettings.renderPipelineAsset = shadowsLow;
            }

            this.settingsSystem = settingsSystem;
            this.currentPipelineAsset = (URPA)GraphicsSettings.renderPipelineAsset;

            // when preset is changed this settings must be set
            settingsSystem.Subscribe(SettingsList.Setting_Key_Perf_Preset, SetShadows);
            settingsSystem.Subscribe(SettingsList.Setting_Key_Perf_Preset, SetMSAA);
            settingsSystem.Subscribe(SettingsList.Setting_Key_Perf_Preset, SetLOD);
            settingsSystem.Subscribe(SettingsList.Setting_Key_Perf_Preset, SetRenderScale);

            // performance
            settingsSystem.Subscribe(SettingsList.Setting_Key_Perf_ShadowQuality, SetShadows);
            settingsSystem.Subscribe(SettingsList.Setting_Key_Perf_Msaa, SetMSAA);
            settingsSystem.Subscribe(SettingsList.Setting_Key_Perf_LodMult, SetLOD);
            settingsSystem.Subscribe(SettingsList.Setting_Key_Perf_ResolutionMult, SetRenderScale);

            // when cutscene starts or ends, set specific shadow distance
            CutsceneManager.OnCutsceneStart += SetCutsceneShadowDistance;
            CutsceneManager.OnCutsceneEnd += SetDefaultShadowDistance;

            // apply initSettings, as actual settings are not applied at the start
            SetShadows(initSettings);
            SetMSAA(initSettings);
            SetLOD(initSettings);
            SetRenderScale(initSettings);
            SetDefaultShadowDistance();
        }

        void OnDestroy()
        {
            if (settingsSystem == null)
            {
                return;
            }

            settingsSystem.Unsubscribe(SettingsList.Setting_Key_Perf_ShadowQuality, SetShadows);
            settingsSystem.Unsubscribe(SettingsList.Setting_Key_Perf_Msaa, SetMSAA);

            CutsceneManager.OnCutsceneStart -= SetCutsceneShadowDistance;
            CutsceneManager.OnCutsceneEnd -= SetDefaultShadowDistance;
        }

        #region performance
        void SetRenderScale(GlobalSettings settings)
        {
            shadowsNone.renderScale = settings.PerfResolutionMult;
            shadowsLow.renderScale = settings.PerfResolutionMult;
            shadowsMedium.renderScale = settings.PerfResolutionMult;
            shadowsHigh.renderScale = settings.PerfResolutionMult;

            Debug.Log("Setting RenderScale: " + settings.PerfResolutionMult
                + ". Result: " + currentPipelineAsset.renderScale);
        }

        void SetLOD(GlobalSettings settings)
        {
            QualitySettings.lodBias = settings.PerfLODMultiplier;

            Debug.Log("Setting LOD: " + settings.PerfLODMultiplier
                + ". Result: " + QualitySettings.lodBias);
        }

        void SetMSAA(GlobalSettings settings)
        {
            int msaa = settings.PerfMsaa;

            if (msaa != 0 && msaa != 2 && msaa != 4)
            {
                msaa = 0;
            }

            shadowsNone.msaaSampleCount = msaa;
            shadowsLow.msaaSampleCount = msaa;
            shadowsMedium.msaaSampleCount = msaa;
            shadowsHigh.msaaSampleCount = msaa;
            QualitySettings.antiAliasing = msaa;

            Debug.Log("Setting MSAA: " + settings.PerfMsaa
                + ". Result: " + currentPipelineAsset.msaaSampleCount);
        }

        void SetShadows(GlobalSettings settings)
        {
            switch (settings.PerfShadowQuality)
            {
                case ShadowQuality.Low:
                    currentPipelineAsset = shadowsLow;
                    break;

                case ShadowQuality.Medium:
                    currentPipelineAsset = shadowsMedium;
                    break;

                case ShadowQuality.High:
                    currentPipelineAsset = shadowsHigh;
                    break;

                //case ShadowQuality.Ultra:
                //    currentPipelineAsset = shadowsNone;
                //    break;

                default:
                    currentPipelineAsset = shadowsNone;
                    break;
            }

            GraphicsSettings.renderPipelineAsset = currentPipelineAsset;

            Debug.Log(settings.PerfShadowQuality + ". GraphicsAsset: " +
                GraphicsSettings.renderPipelineAsset + ". CurrentAsset: " +
                currentPipelineAsset);
        }

        /// <summary>
        /// Set shadow distance for cutscene
        /// </summary>
        public void SetCutsceneShadowDistance()
        {
            shadowsNone.shadowDistance = CutsceneShadowDistance;
            shadowsLow.shadowDistance = CutsceneShadowDistance;
            shadowsMedium.shadowDistance = CutsceneShadowDistance;
            shadowsHigh.shadowDistance = CutsceneShadowDistance;
        }

        /// <summary>
        /// Set shadow distance for cutscene
        /// </summary>
        public void SetDefaultShadowDistance()
        {
            shadowsNone.shadowDistance = DefaultShadowDistance;
            shadowsLow.shadowDistance = DefaultShadowDistance;
            shadowsMedium.shadowDistance = DefaultShadowDistance;
            shadowsHigh.shadowDistance = DefaultShadowDistance;
        }
        #endregion
    }
}
