using System;
using UnityEngine;

namespace SD.Game
{
    class TimeController
    {
        const float WeaponsSelectionMultiplier = 0.2f;
        const float TutorialPanelMultiplier = 0.0f;

        float defaultTimeScale;
        float defaultFixedDelta;

        public event Action<float> OnTimeScaleChange;

        public TimeController(float defaultFixedDelta, float defaultTimeScale = 1.0f)
        {
            this.defaultTimeScale = defaultTimeScale;
            this.defaultFixedDelta = defaultFixedDelta;

            SetDefault();

            GameController.OnGamePause += ProcessPause;
            GameController.OnGameUnpause += SetDefault;

            GameController.OnWeaponSelectionEnable += ProcessWeaponSelection;
            GameController.OnWeaponSelectionDisable += SetDefault;

            GameController.OnMainMenuActivate += SetDefault;

            TutorialManager.OnTutorialPanelActivate += ProcessTutorialPanelActivation;
            TutorialManager.OnTutorialPanelDeactivate += SetDefault;
        }

        ~TimeController()
        {
            GameController.OnGamePause -= ProcessPause;
            GameController.OnGameUnpause -= SetDefault;

            GameController.OnWeaponSelectionEnable -= ProcessWeaponSelection;
            GameController.OnWeaponSelectionDisable -= SetDefault;

            GameController.OnMainMenuActivate -= SetDefault;
        }

        public void SetDefault()
        {
            SetTimeScale(1.0f);
        }

        void ProcessPause()
        {
            SetTimeScale(0.0f);
        }

        void ProcessWeaponSelection()
        {
            SetTimeScale(WeaponsSelectionMultiplier);
        }

        void ProcessTutorialPanelActivation()
        {
            SetTimeScale(TutorialPanelMultiplier);
        }

        public void SetTimeScale(float scale)
        {
            if (scale >= 0)
            {
                Time.timeScale = defaultTimeScale * scale;

                if (scale != 0)
                {
                    Time.fixedDeltaTime = defaultFixedDelta * scale;
                }

                OnTimeScaleChange?.Invoke(scale);
            }
        }
    }
}
