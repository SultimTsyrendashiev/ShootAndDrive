using UnityEngine;
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

        bool initialized = false;

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

            initialized = true;
        }

        void OnEnable()
        {
            if (!initialized)
            {
                SetMovementInputType(GameController.Instance.Settings);
            }

            SetActiveHUD(true);
            SetActiveWeaponSelectionMenu(false);
        }

        void OnDestroy()
        {
            GameController.Instance.SettingsSystem.Unsubscribe(SettingsList.Setting_Key_Input_MovementType, SetMovementInputType);
        }

        void SetMovementInputType(GlobalSettings settings)
        {
            print("SetMovementInputType: " + settings.InputMovementType);

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

        public void SetActiveWeaponSelectionMenu(bool active)
        {
            weaponSelection.SetActive(active);
        }

        public void SetActiveHUD(bool active)
        {
            hud.SetActive(active);
            interactive.SetActive(active);
        }
    }
}