using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SD.UI.Controls
{
    class WeaponWheelSelector : MonoBehaviour
    {
        [SerializeField]
        InputController inputController;

        [SerializeField]
        GameObject availableWeaponsObj;
        [SerializeField]
        GameObject noAvailableWeaponsObj;

        [SerializeField]
        Color defaultColor;
        [SerializeField]
        Color highlitedColor;
        [SerializeField]
        Color disabledColor;

        [SerializeField]
        GameObject wheel2;
        [SerializeField]
        GameObject wheel3;
        [SerializeField]
        GameObject wheel4;
        [SerializeField]
        GameObject wheel6;
        [SerializeField]
        GameObject wheel8;

        [SerializeField]
        Text weaponNameText;

        [SerializeField]
        Image ammoTypeImage;
        [SerializeField]
        Text ammoAmountText;

        [SerializeField]
        Color ammoColor;
        [SerializeField]
        Color ammoEmptyColor;

        [SerializeField]
        Text healthText;

        [SerializeField]
        Color greenHealthColor;
        [SerializeField]
        Color yellowHealthColor;
        [SerializeField]
        Color redHealthColor;

        IInventory inventory;

        bool isSelected;

        void OnEnable()
        {
            isSelected = false;
            UpdateWheel();
        }

        void Highlight(WeaponIndex w, bool canBeSelected)
        {
            IWeaponItem weaponItem = inventory.Weapons.Get(w);
            IAmmoItem ammoItem = inventory.Ammo.Get(weaponItem.AmmoType);

            SetName(weaponItem);
            SetHealth(weaponItem);
            SetAmmo(ammoItem);

            isSelected = canBeSelected;
        }

        void Unhighlight()
        {
            isSelected = false;
        }

        void Select(WeaponIndex w)
        {
            // if none is highlited, do nothing
            if (isSelected)
            {
                inputController.SelectWeapon(w);
                inputController.OnWeaponSelectorUp();
            }
        }

        void SetName(IWeaponItem item)
        {
            weaponNameText.enabled = item != null;

            if (item == null)
            {
                return;
            }

            string weaponName = GameController.Instance.Localization.GetValue(
                GameController.Instance.Settings.GameLanguage, item.TranslationKey);

            weaponNameText.text = weaponName;
        }

        void SetHealth(IWeaponItem item)
        {
            healthText.enabled = !item.IsAmmo;
            
            if (item.IsAmmo) 
            {
                return;
            }

            float health = (float)item.Health / item.Durability * 100;

            healthText.text = health.ToString("F1") + "%";

            const float Yellow = 15;
            const float Red = 5;

            if (health > Yellow)
            {
                healthText.color = greenHealthColor;
            }
            else if (health > Red)
            {
                healthText.color = yellowHealthColor;
            }
            else
            {
                healthText.color = redHealthColor;
            }
        }

        void SetAmmo(IAmmoItem item)
        {
            ammoTypeImage.enabled = ammoAmountText.enabled = item != null;

            if (item == null)
            {
                return;
            }

            ammoAmountText.text = item.CurrentAmount.ToString();
            ammoTypeImage.sprite = item.Icon;

            ammoAmountText.color = ammoTypeImage.color =
                item.CurrentAmount != 0 ? ammoColor : ammoEmptyColor;
        }

        void UpdateWheel()
        {
            if (inventory == null)
            {
                inventory = GameController.Instance.Inventory;
            }

            IWeaponsHolder weapons = inventory.Weapons;
            IAmmoHolder ammo = inventory.Ammo;

            List<WeaponIndex> available = weapons.GetAvailableWeaponsInGame();
            int availableAmount = available.Count;

            GameObject wheel = GetWheel(availableAmount);

            if (wheel == null)
            {
                return;
            }

            int counter = 0;

            var bs = wheel.GetComponentsInChildren<WeaponWheelButton>(true);

            foreach (WeaponWheelButton b in bs)
            {
                if (counter < availableAmount)
                {
                    IWeaponItem w = weapons.Get(available[counter]);
                    IAmmoItem a = ammo.Get(w.AmmoType);

                    b.SetColors(defaultColor, highlitedColor, disabledColor);
                    b.Set(w, a, Select, Highlight, Unhighlight);
                }
                else
                {
                    // disable buttons without weapon
                    b.Disable();
                }

                counter++;
            }
        }

        GameObject GetWheel(int weaponAmount)
        {
            wheel2.SetActive(false);
            wheel3.SetActive(false);
            wheel4.SetActive(false);
            wheel6.SetActive(false);
            wheel8.SetActive(false);

            availableWeaponsObj.SetActive(weaponAmount > 0);
            noAvailableWeaponsObj.SetActive(weaponAmount <= 0);

            if (weaponAmount <= 0)
            {
                return null;
            }

            GameObject result;
            
            if (weaponAmount <= 2)
            {
                result = wheel2;
            }
            else if (weaponAmount <= 3)
            {
                result = wheel3;
            }
            else if (weaponAmount <= 4)
            {
                result = wheel4;
            }
            else if (weaponAmount <= 6)
            {
                result = wheel6;
            }
            else
            {
                result = wheel8;
            }

            result.SetActive(true);
            return result;
        }
    }
}