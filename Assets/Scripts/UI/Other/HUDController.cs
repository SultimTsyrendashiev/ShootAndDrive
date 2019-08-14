using UnityEngine;
using UnityEngine.UI;

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

        void Start()
        {
            SetActiveHUD(true);
            SetActiveWeaponSelectionMenu(false);

            GlobalSettings.OnMovementInputTypeChange += SetMovementInputType;
        }

        void OnEnable()
        {
            SetMovementInputType(GameController.Instance.Settings.InputMovementType);
        }

        void OnDestroy()
        {
            GlobalSettings.OnMovementInputTypeChange -= SetMovementInputType;
        }

        void SetMovementInputType(MovementInputType type)
        {
            switch (type)
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