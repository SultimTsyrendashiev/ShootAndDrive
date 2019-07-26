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

        MovementInputType movementInputType;

        [SerializeField]
        GameObject movementField;
        [SerializeField]
        GameObject movementButtons;

        float maxVehicleHealth;

        void Start()
        {
            SetActiveHUD(true);
            SetActiveWeaponSelectionMenu(false);
        }

        public MovementInputType MovementInputType
        {
            get
            {
                return MovementInputType;
            }
            set
            {
                if (movementInputType == value)
                {
                    return;
                }

                switch (value)
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