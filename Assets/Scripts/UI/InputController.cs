using System;
using UnityEngine;
using SD.PlayerLogic;
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
        UIController            uiController;

        public static bool      FireButton { get; private set; } = false;
        public static float     MovementHorizontal { get; private set; } = 0.0f;

        public static event Void OnFireButton;
        public static event WeaponSwitch OnWeaponSwitch;

        public static event RegenerateHealth OnHealthRegenerate;

        #region input in the editor
        [SerializeField]
        bool useEditorInput;
        [SerializeField]
        bool ignoreMovementInput;

        void Start()
        {
            uiController = GetComponentInParent<UIController>();
        }

        void Update()
        {
#if UNITY_EDITOR
            if (!useEditorInput)
            {
                return;
            }

            if (!ignoreMovementInput)
            {
                float x = Input.GetAxis("Horizontal");
                MovementHorizontal = x;
            }

            bool pressedRight = Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.UpArrow);
            bool pressedLeft = Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.DownArrow);

            if (pressedLeft || pressedRight)
            {
                var w = FindObjectOfType<WeaponsController>();

                if (!w.IsBusy())
                {
                    if (w.GetCurrentWeapon(out WeaponIndex current))
                    {
                        if (w.GetNextAvailable(current, out WeaponIndex available, pressedRight))
                        {
                            SelectWeapon(available);
                        }
                    }
                }
            }
#endif
        }

        #endregion

        public void UpdateMovementInput(float x)
        {
            MovementHorizontal = x;
        }

        #region event subscribers
        /// <summary>
        /// Called on pointer down on fire button
        /// </summary>
        public void OnFireDown()
        {
            FireButton = true;

            // also call weapon controller
            OnFireButton();
        }

        /// <summary>
        /// Called on pointer up from fire button
        /// </summary>
        public void OnFireUp()
        {
            FireButton = false;
        }

        public void OnRightDown()
        {
            MovementHorizontal = 1.0f;
        }
        public void OnRightUp()
        {
            MovementHorizontal = 0.0f;
        }

        public void OnLeftDown()
        {
            MovementHorizontal = -1.0f;
        }
        public void OnLeftUp()
        {
            MovementHorizontal = 0.0f;
        }

        /// <summary>
        /// Called on pointer down on weapon selector button
        /// </summary>
        public void OnWeaponSelectorDown()
        {
            uiController.SetActiveHUD(false);
            uiController.SetActiveWeaponSelectionMenu(true);
        }

        /// <summary>
        /// Called on pointer up from weapon selector
        /// (anywhere in weapon selector menu except weapon buttons)
        /// </summary>
        public void OnWeaponSelectorUp()
        {
            uiController.SetActiveHUD(true);
            uiController.SetActiveWeaponSelectionMenu(false);
        }

        /// <summary>
        /// Called on click on pause button
        /// </summary>
        public void OnPauseClick()
        {
            // deactivate
            if (uiController.MovementInputType == MovementInputType.Joystick)
            {
                // TODO: call joystick to deactivate
                // OnMovementUp();
            }
            else if (uiController.MovementInputType == MovementInputType.Buttons)
            {
                OnLeftUp();
                OnRightUp();
            }

            if (FireButton)
            {
                OnFireUp();
            }

            uiController.SetActiveHUD(false);
            uiController.SetActivePauseMenu(true);
        }

        /// <summary>
        /// Called on click on health button
        /// </summary>
        public void OnHealthClick()
        {
            // try to regenerate
            OnHealthRegenerate();
        }

        /// <summary>
        /// Called on pointer up from weapon button in weapon selection menu
        /// </summary>
        public void SelectWeapon(WeaponIndex w)
        {
            OnWeaponSwitch(w);
        }
#endregion
    }
}