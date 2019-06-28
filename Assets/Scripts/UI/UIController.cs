using System.Collections;
using UnityEngine;

namespace SD.UI
{
    class UIController : MonoBehaviour
    {
        [SerializeField]
        private GameObject hud;
        [SerializeField]
        private GameObject interactive;
        [SerializeField]
        private GameObject weaponSelection;
        [SerializeField]
        private GameObject pauseMenu;

        MovementInputType movementInputType;

        [SerializeField]
        private GameObject movementField;
        [SerializeField]
        private GameObject movementButtons;

        static UIController instance;
        public static UIController Instance => instance;

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            SetActiveHUD(true);
            SetActivePauseMenu(false);
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

        public void SetActivePauseMenu(bool active)
        {
            pauseMenu.SetActive(active);
        }
    }
}