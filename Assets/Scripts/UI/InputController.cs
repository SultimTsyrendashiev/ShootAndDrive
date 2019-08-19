using System;
using UnityEngine;
using SD.PlayerLogic;
using SD.Weapons;

namespace SD.UI
{
    class InputController : MonoBehaviour
    {
        [SerializeField]
        HUDController            hudController;


        public static bool      FireButton { get; private set; } = false;
        public static float     MovementHorizontal { get; private set; } = 0.0f;


        public static event FloatChange         OnMovementHorizontal;

        public static event Void                OnFireButton;
        public static event WeaponSwitch        OnWeaponSwitch;
        public static event Void                OnPause;
        public static event Void                OnUnpause;
        public static event Void                OnMainMenuButton;
        public static event Void                OnPlayButton;
        public static event Void                OnPlayWithInventoryButton;
        public static event Void                OnSettingsButton;
        public static event Void                OnInventoryButton;

        public static event RegenerateHealth    OnHealthRegenerate;

        #region input in the editor
        [SerializeField]
        bool useEditorInput;
        [SerializeField]
        bool ignoreMovementInput;

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
                OnMovementHorizontal(MovementHorizontal);
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
            OnMovementHorizontal(MovementHorizontal);
        }

        #region event subscribers
        /// <summary>
        /// Called on pointer down on fire button
        /// </summary>
        public void FireDown()
        {
            FireButton = true;
            OnFireButton();
        }

        /// <summary>
        /// Called on pointer up from fire button
        /// </summary>
        public void FireUp()
        {
            FireButton = false;
        }

        /// <summary>
        /// Called on pointer down on weapon selector button
        /// </summary>
        public void OnWeaponSelectorDown()
        {
            hudController.SetActiveHUD(false);
            hudController.SetActiveWeaponSelectionMenu(true);
        }

        /// <summary>
        /// Called on pointer up from weapon selector
        /// (anywhere in weapon selector menu except weapon buttons)
        /// </summary>
        public void OnWeaponSelectorUp()
        {
            hudController.SetActiveHUD(true);
            hudController.SetActiveWeaponSelectionMenu(false);
        }

        /// <summary>
        /// Called on click on pause button
        /// </summary>
        public void Pause()
        {
            OnPause();
        }

        /// <summary>
        /// Called on click on continue button
        /// </summary>
        public void Unpause()
        {
            OnUnpause();
        }

        public void OpenMainMenu()
        {
            OnMainMenuButton();
        }

        public void OpenSettings()
        {
            OnSettingsButton();
        }

        public void OpenInventory()
        {
            OnInventoryButton();
        }

        public void Play()
        {
            OnPlayButton();
        }

        public void PlayWithInventory()
        {
            OnPlayWithInventoryButton();
        }

        /// <summary>
        /// Called on click on health button
        /// </summary>
        public void HealthClick()
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