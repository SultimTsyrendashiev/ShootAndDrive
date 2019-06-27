using System;
using UnityEngine;
using SD.Player;
using SD.Weapons;

namespace SD.UI
{
    enum MovementInputType
    {
        Joystick,
        Buttons,
        Gyroscope
    }

    class InputController : MonoBehaviour
    {
        private static bool fireButtonDown;
        public static bool FireButton => fireButtonDown;

#if UNITY_EDITOR
        [SerializeField]
        bool useEditorInput;
        int currentWeaponIndex = 0;

        void Update()
        {
            if (!useEditorInput)
            {
                return;
            }

            if (Input.GetMouseButton(0))
            {
                if (WeaponsController.Instance.GetCurrentWeaponState() == Weapons.WeaponState.Ready)
                {
                    OnFireDown();
                }
            }
            else if (!fireButtonDown)
            {
                OnFireUp();
            }

            if (Input.GetKey(KeyCode.E))
            {
                if (WeaponsController.Instance.GetCurrentWeaponState() == Weapons.WeaponState.Ready)
                {
                    currentWeaponIndex++;
                    if (currentWeaponIndex >= Enum.GetValues(typeof(WeaponIndex)).Length)
                    {
                        currentWeaponIndex = 0;
                    }

                    OnWeaponUp((int)currentWeaponIndex);
                }
            }
            else if (Input.GetKey(KeyCode.Q))
            {
                if (WeaponsController.Instance.GetCurrentWeaponState() == Weapons.WeaponState.Ready)
                {
                    currentWeaponIndex--;
                    if (currentWeaponIndex < 0)
                    {
                        currentWeaponIndex = Enum.GetValues(typeof(WeaponIndex)).Length - 1;
                    }

                    OnWeaponUp((int)currentWeaponIndex);
                }
            }
        }
#endif

        #region event subscribers
        /// <summary>
        /// Called on pointer down on fire button
        /// </summary>
        public void OnFireDown()
        {
            fireButtonDown = true;

            // also call weapon controller
            WeaponsController.Instance.Fire();
        }

        /// <summary>
        /// Called on pointer up from fire button
        /// </summary>
        public void OnFireUp()
        {
            fireButtonDown = false;
        }

        /// <summary>
        /// Called on pointer down on movement field
        /// </summary>
        public void OnMovementDown()
        {
            // TODO: 
            // show joystick
            // if gyroscope is used, this must be hidden
        }

        /// <summary>
        /// Called on pointer up from movement field
        /// </summary>
        public void OnMovementUp()
        {
            // TODO:
            // hide joystick
        }

        public void OnRightDown()
        {

        }
        public void OnRightUp()
        {

        }

        public void OnLeftDown()
        {

        }
        public void OnLeftUp()
        {

        }

        /// <summary>
        /// Called on pointer down on weapon selector button
        /// </summary>
        public void OnWeaponSelectorDown()
        {
            UIController.Instance.SetActiveHUD(false);
            UIController.Instance.SetActiveWeaponSelectionMenu(true);
        }

        /// <summary>
        /// Called on pointer up from weapon selector
        /// (anywhere in weapon selector menu except weapon buttons)
        /// </summary>
        public void OnWeaponSelectorUp()
        {
            UIController.Instance.SetActiveHUD(true);
            UIController.Instance.SetActiveWeaponSelectionMenu(false);
        }

        /// <summary>
        /// Called on click on pause button
        /// </summary>
        public void OnPauseClick()
        {
            // deactivate
            if (UIController.Instance.MovementInputType == MovementInputType.Joystick)
            {
                OnMovementUp();
            }
            else if (UIController.Instance.MovementInputType == MovementInputType.Buttons)
            {
                OnLeftUp();
                OnRightUp();
            }

            if (fireButtonDown)
            {
                OnFireUp();
            }

            UIController.Instance.SetActiveHUD(false);
            UIController.Instance.SetActivePauseMenu(true);
        }

        /// <summary>
        /// Called on click on health button
        /// </summary>
        public void OnHealthClick()
        {
            // try to regenerate
            Player.Player.Instance.RegenerateHealth();
        }

        /// <summary>
        /// Called on pointer up from weapon button in weapon selection menu
        /// </summary>
        public void OnWeaponUp(int w)
        {
            UIController.Instance.SetActiveWeaponSelectionMenu(false);
            UIController.Instance.SetActiveHUD(true);
            WeaponsController.Instance.SwitchTo((WeaponIndex)w);
        }
#endregion
    }
}