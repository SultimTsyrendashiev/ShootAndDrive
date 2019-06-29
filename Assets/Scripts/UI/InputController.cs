using System;
using UnityEngine;
using SD.Player;
using SD.Weapons;
using UnityEngine.EventSystems;

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
        private static bool fireButtonDown = false;
        private static float movementHorizontal = 0.0f;

        public static bool FireButton => fireButtonDown;
        public static float MovementHorizontal => movementHorizontal;

        #region input in the editor
#if UNITY_EDITOR
        [SerializeField]
        bool useEditorInput;
        [SerializeField]
        bool ignoreMovementInput;
        int currentWeaponIndex = 0;

        void Update()
        {
            if (!useEditorInput)
            {
                return;
            }

            if (!ignoreMovementInput)
            {
                float x = Input.GetAxis("Horizontal");
                movementHorizontal = x;
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

                    SelectWeapon((WeaponIndex)currentWeaponIndex);
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

                    SelectWeapon((WeaponIndex)currentWeaponIndex);
                }
            }
        }
#endif
        #endregion

        public void UpdateMovementInput(float x)
        {
            movementHorizontal = x;
        }

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

        public void OnRightDown()
        {
            movementHorizontal = 1.0f;
        }
        public void OnRightUp()
        {
            movementHorizontal = 0.0f;
        }

        public void OnLeftDown()
        {
            movementHorizontal = -1.0f;
        }
        public void OnLeftUp()
        {
            movementHorizontal = 0.0f;
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
                // TODO: call joystick to deactivate
                // OnMovementUp();
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
        public void SelectWeapon(WeaponIndex w)
        {
            WeaponsController.Instance.SwitchTo(w);
        }
#endregion
    }
}