using System;
using System.Collections.Generic;
using UnityEngine;

namespace SD.UI.Shop
{
    class AmmoShop : MonoBehaviour, IMenu
    {
        [SerializeField]
        GameObject ammoItemPrefab;

        [SerializeField]
        Transform itemsContainer;

        [SerializeField]
        GameObject noAvailableAmmoText;

        ShopItemAmmo[] ammoShopItems;

        public void Init(MenuController menuController)
        {
            int amount = Enum.GetValues(typeof(AmmunitionType)).Length;

            var inScene = itemsContainer.GetComponentsInChildren<ShopItemAmmo>(true);
            int inSceneAmount = inScene.Length;

            if (inSceneAmount < amount)
            {
                int toCreate = amount - inSceneAmount;

                // create new
                for (int i = 0; i < toCreate; i++)
                {
                    Instantiate(ammoItemPrefab, itemsContainer);
                }
            }
            else
            {
                int toDelete = amount - inSceneAmount;

                // delete unnecessary
                for (int i = 0; i < toDelete; i++)
                {
                    Destroy(inScene[i].gameObject);
                }
            }

            ammoShopItems = itemsContainer.GetComponentsInChildren<ShopItemAmmo>(true);
        }

        public void Activate()
        {
            gameObject.SetActive(true);

            IShop shop = GameController.Instance.Shop;
            IInventory inventory = GameController.Instance.Inventory;

            List<WeaponIndex> availableWeapons = inventory.Weapons.GetAvailableWeapons();
            List<AmmunitionType> availableAmmo = inventory.Ammo.GetNecessaryAmmo(availableWeapons);

            // activate needed
            for (int i = 0; i < availableAmmo.Count; i++)
            {
                ammoShopItems[i].gameObject.SetActive(true);

                ammoShopItems[i].SetInfo(shop, inventory.Ammo.Get(availableAmmo[i]));
            }

            // deactivate other
            for (int i = availableAmmo.Count; i < ammoShopItems.Length; i++)
            {
                ammoShopItems[i].gameObject.SetActive(false);
            }

            noAvailableAmmoText.SetActive(availableAmmo.Count == 0);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}
