﻿using UnityEngine;
using UnityEngine.UI;

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

        [SerializeField]
        Text currentAmmoText;
        [SerializeField]
        Text distanceText;
        [SerializeField]
        Text killsAmountText;

        const float MaxHealthImageWidth = 160;
        [SerializeField]
        Image healthImage;
        [SerializeField]
        Image vehicleHealthImage;

        void Start()
        {
            SetActiveHUD(true);
            SetActivePauseMenu(false);
            SetActiveWeaponSelectionMenu(false);

            // sign to events
            Weapons.Weapon.OnAmmoChange += SetAmmoAmount;
            PlayerLogic.Player.Instance.OnHealthChange += SetHealth;
            PlayerLogic.PlayerVehicle.OnDistanceChange += SetDistance;
            PlayerLogic.PlayerVehicle.OnVehicleHealthChange += SetVehicleHealth;
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

        public void SetAmmoAmount(int amount)
        {
            currentAmmoText.text = amount >= 0 ? amount.ToString() : "";
        }

        public void SetDistance(float meters)
        {
            distanceText.text = meters.ToString("N1");
        }

        public void SetKillsAmount(int amount)
        {
            killsAmountText.text = amount.ToString();
        }

        /// <summary>
        /// Set health in HUD
        /// </summary>
        /// <param name="health">health in [0..100]</param>
        public void SetHealth(float health)
        {
            Vector2 d = healthImage.rectTransform.sizeDelta;
            d.x = health / 100 * MaxHealthImageWidth;

            healthImage.rectTransform.sizeDelta = d;
        }

        /// <summary>
        /// Set health in HUD
        /// </summary>
        /// <param name="health">health in [0..100]</param>
        public void SetVehicleHealth(float health)
        {
            Vector2 d = vehicleHealthImage.rectTransform.sizeDelta;
            d.x = health / 100 * MaxHealthImageWidth;

            vehicleHealthImage.rectTransform.sizeDelta = d;
        }
    }
}