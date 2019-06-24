using UnityEngine;
using SD.Player;

namespace SD.Player
{
    class InputController
    {
        private static bool fireButtonDown;

        public static bool FireButton => fireButtonDown;

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
            // show joystick or buttons
            // if gyroscope is used, do nothing
        }

        /// <summary>
        /// Called on pointer up from movement field
        /// </summary>
        public void OnMovementUp()
        {
            // TODO:
            // hide joystick or buttons
        }

        /// <summary>
        /// Called on pointer down on weapon selector button
        /// </summary>
        public void OnWeaponSelectorDown()
        {
            SetActiveHUD(false);
            SetActiveWeaponSelectionMenu(true);
        }

        /// <summary>
        /// Called on pointer up from weapon selector
        /// (anywhere in weapon selector menu except weapon buttons)
        /// </summary>
        void OnWeaponSelectorUp()
        {
            SetActiveHUD(true);
            SetActiveWeaponSelectionMenu(false);
        }

        /// <summary>
        /// Called on click on pause button
        /// </summary>
        void OnPauseClick()
        {
            SetActiveHUD(false);
            SetActivePauseMenu(true);
        }

        /// <summary>
        /// Called on click on health button
        /// </summary>
        public void OnHealthClick()
        {
            // TODO:
            // regenerate player's health:
            // 1  check if weapon state is Ready (else return)
            // 2  hide weapon
            // 3  regenerate :
            //     if health<15 then regen without mekit
            //     if has a medkit, use it
            //     anim, sound
            // 4  take out weapon
        }

        /// <summary>
        /// Called on pointer up from weapon button in weapon selection menu
        /// </summary>
        public void OnWeaponUp(WeaponsEnum w)
        {
            SetActiveWeaponSelectionMenu(false);
            SetActiveHUD(true);
            WeaponsController.Instance.SwitchTo(w);
        }

        void SetActiveWeaponSelectionMenu(bool active)
        {
            // TODO
        }

        void SetActiveHUD(bool active)
        {
            // TODO
        }
        void SetActivePauseMenu(bool active)
        {
            // TODO
        }
    }
}