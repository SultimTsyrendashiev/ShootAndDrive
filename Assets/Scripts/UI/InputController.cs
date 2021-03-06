﻿using System;
using UnityEngine;
using SD.PlayerLogic;
using SD.Weapons;

namespace SD.UI
{
    class InputController : MonoBehaviour
    {
        public static bool      FireButton { get; private set; } = false;
        public static float     MovementHorizontal { get; private set; } = 0.0f;


        public static event Action<float>       OnMovementHorizontal;

        public static event Void                OnFireButton;
        public static event Action<WeaponIndex> OnWeaponSwitch;

        public static event Void                OnPause;
        public static event Void                OnUnpause;

        public static event Void                OnMainMenuButton;
        public static event Void                OnInventoryRevert;

        public static event Void                OnSettingsApply;

        public static event Void                OnPlayButton;
        public static event Void                OnPlayWithInventoryButton;

        public static event Void                OnSettingsButton;
        public static event Void                OnInventoryButton;

        public static event Void                OnWeaponSelectionButton;
        public static event Void                OnWeaponSelectionDisableButton;

        public static event RegenerateHealth    OnHealthRegenerate;

        #region input in the editor
        [SerializeField]
        bool useEditorInput;
        [SerializeField]
        bool ignoreMovementInput;
        #endregion

        void Update()
        {
            // check back button
            //if (Input.GetKey(KeyCode.Escape))
            //{
            //    Pause();
            //}

            #region input in the editor
#if UNITY_EDITOR
            if (!useEditorInput)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                FireDown();
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                FireUp();
            }

            if (!ignoreMovementInput)
            {
                float x = Input.GetAxis("Horizontal");
                MovementHorizontal = x;
                OnMovementHorizontal?.Invoke(MovementHorizontal);
            }

            bool pressedRight = Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.UpArrow);
            bool pressedLeft = Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.DownArrow);

            if (pressedLeft || pressedRight)
            {
                var w = FindObjectOfType<WeaponsController>();

                if (w == null)
                {
                    return;
                }

                //if (!w.IsBusy())
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
            #endregion
        }


        public void UpdateMovementInput(float x)
        {
            MovementHorizontal = x;
            OnMovementHorizontal?.Invoke(MovementHorizontal);
        }

        #region event subscribers
        /// <summary>
        /// Called on pointer down on fire button
        /// </summary>
        public void FireDown()
        {
            FireButton = true;
            OnFireButton?.Invoke();
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
            OnWeaponSelectionButton?.Invoke();

            // quick fix
            FireUp();
        }

        /// <summary>
        /// Called on pointer up from weapon selector
        /// (anywhere in weapon selector menu except weapon buttons)
        /// </summary>
        public void OnWeaponSelectorUp()
        {
            OnWeaponSelectionDisableButton();
        }

        /// <summary>
        /// Called on click on pause button
        /// </summary>
        public void Pause()
        {
            OnPause?.Invoke();
        }

        /// <summary>
        /// Called on click on continue button
        /// </summary>
        public void Unpause()
        {
            OnUnpause?.Invoke();
        }

        public void ApplySettings()
        {
            OnSettingsApply?.Invoke();
        }

        public void OpenMainMenu()
        {
            OnMainMenuButton?.Invoke();
        }

        public void RevertInventory()
        {
            OnInventoryRevert?.Invoke();
        }

        public void OpenSettings()
        {
            OnSettingsButton?.Invoke();
        }

        public void OpenInventory()
        {
            OnInventoryButton?.Invoke();
        }

        public void Play()
        {
            OnPlayButton?.Invoke();
        }

        public void PlayWithInventory()
        {
            OnPlayWithInventoryButton?.Invoke();
        }

        /// <summary>
        /// Called on click on health button
        /// </summary>
        public void HealthClick()
        {
            // try to regenerate
            OnHealthRegenerate?.Invoke();
        }

        /// <summary>
        /// Called on pointer up from weapon button in weapon selection menu
        /// </summary>
        public void SelectWeapon(WeaponIndex w)
        {
            OnWeaponSwitch?.Invoke(w);
        }
#endregion
    }
}