using System;
using System.Collections.Generic;
using UnityEngine;

namespace SD.UI.Shop
{
    class WeaponShop : MonoBehaviour, IMenu
    {
        [SerializeField]
        Transform           itemsContainer;

        // only one in a scene, all info about current is loaded to this component
        ShopItemWeapon      weaponShopItem;
        WeaponIndex         currentWeapon;

        IInventory inventory;
        IShop shop;

        public void Init(MenuController menuController)
        {
            weaponShopItem = itemsContainer.GetComponentInChildren<ShopItemWeapon>(true);
        }

        public void Activate()
        {
            gameObject.SetActive(true);

            shop = GameController.Instance.Shop;
            inventory = GameController.Instance.Inventory;

            SetWeaponItem(currentWeapon);
        }

        void SetWeaponItem(WeaponIndex w)
        {
            if (inventory == null || shop == null)
            {
                return;
            }

            // get current
            IWeaponItem weaponItem = inventory.Weapons.Get(w);
            IAmmoItem ammoItem = inventory.Ammo.Get(weaponItem.AmmoType);

            // set info
            weaponShopItem.SetInfo(shop, weaponItem, ammoItem);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        public void SetNext()
        {
            currentWeapon = GetNextIndex(1);
            SetWeaponItem(currentWeapon);
        }

        public void SetPrevious()
        {
            currentWeapon = GetNextIndex(-1);
            SetWeaponItem(currentWeapon);
        }

        WeaponIndex GetNextIndex(int shift)
        {
            int next = (int)currentWeapon + shift;
            int amount = Enum.GetValues(typeof(WeaponIndex)).Length;

            if (next >= 0)
            {
                if (next < amount)
                {
                    return (WeaponIndex)next;
                }
                else
                {
                    return (WeaponIndex)(amount - 1);
                }
            }
            else
            {
                return 0;
            }
        }
    }
}
