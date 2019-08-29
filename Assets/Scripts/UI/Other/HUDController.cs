using UnityEngine;
using UnityEngine.UI;
using SD.Game.Settings;

namespace SD.UI
{
    class HUDController : MonoBehaviour
    {
        [SerializeField]
        GameObject hud;
        [SerializeField]
        GameObject interactive;
        [SerializeField]
        GameObject weaponSelection;

        [SerializeField]
        GameObject movementField;
        [SerializeField]
        GameObject movementButtons;

        [SerializeField]
        Image pauseButtonImage;

        bool initialized = false;
        bool hideHud;

        void Start()
        {
            GameController.OnWeaponSelectionDisable += () => 
                {
                    SetActiveHUD(true);
                    SetActiveWeaponSelectionMenu(false);
                };

            GameController.OnWeaponSelectionEnable += () =>
                {
                    SetActiveHUD(false);
                    SetActiveWeaponSelectionMenu(true);
                };

            // when setting is changed, call handler
            GameController.Instance.SettingsSystem.Subscribe(SettingsList.Setting_Key_Input_MovementType, SetMovementInputType);
            GameController.Instance.SettingsSystem.Subscribe(SettingsList.Setting_Key_HUD_ShowPauseBtn, DrawPauseButton);
            GameController.Instance.SettingsSystem.Subscribe(SettingsList.Setting_Key_HUD_Hide, HideHud);

            initialized = true;
        }

        void OnDestroy()
        {
            GameController.Instance.SettingsSystem.Unsubscribe(SettingsList.Setting_Key_Input_MovementType, SetMovementInputType);
            GameController.Instance.SettingsSystem.Unsubscribe(SettingsList.Setting_Key_HUD_ShowPauseBtn, DrawPauseButton);
            GameController.Instance.SettingsSystem.Unsubscribe(SettingsList.Setting_Key_HUD_Hide, HideHud);
        }

        void OnEnable()
        {
            if (!initialized)
            {
                var settings = GameController.Instance.Settings;

                SetMovementInputType(settings);
                DrawPauseButton(settings);
                HideHud(settings);
            }

            SetActiveHUD(true);
            SetActiveWeaponSelectionMenu(false);
        }

        void SetMovementInputType(GlobalSettings settings)
        {
            switch (settings.InputMovementType)
            {
                case MovementInputType.Joystick:
                    movementField.SetActive(true);
                    movementButtons.SetActive(false);
                    break;
                case MovementInputType.Buttons:
                    movementField.SetActive(false);
                    movementButtons.SetActive(true);
                    break;
                case MovementInputType.Gyroscope:
                    movementField.SetActive(false);
                    movementButtons.SetActive(false);
                    break;
            }
        }

        void DrawPauseButton(GlobalSettings settings)
        {
            pauseButtonImage.enabled = settings.HUDShowPauseButton;
        }

        void HideHud(GlobalSettings settings)
        {
            hideHud = settings.HUDHide;
        }

        public void SetActiveWeaponSelectionMenu(bool active)
        {
            weaponSelection.SetActive(active);
        }

        public void SetActiveHUD(bool active)
        {
            hud.SetActive(active && !hideHud);
            interactive.SetActive(active);
        }
    }
}