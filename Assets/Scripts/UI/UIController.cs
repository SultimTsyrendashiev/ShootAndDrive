using System.Collections;
using UnityEngine;
using SD.Player;

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
        public MovementInputType MovementInputType => movementInputType;

        [SerializeField]
        private GameObject movementField;
        [SerializeField]
        private GameObject joystick;
        [SerializeField]
        private GameObject movementButtons;

        static UIController instance;
        public static UIController Instance => instance;

        void Start()
        {
            instance = this;
        }

        public void SetMovementInputType(MovementInputType type)
        {
            if (movementInputType == type)
            {
                return;
            }

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
                    movementButtons.SetActive(true);
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

        public void SetActivePauseMenu(bool active)
        {
            pauseMenu.SetActive(active);
        }

        //public void SetActiveInteractive(bool active)
        //{
        //    interactive.SetActive(active);
        //}
    }
}