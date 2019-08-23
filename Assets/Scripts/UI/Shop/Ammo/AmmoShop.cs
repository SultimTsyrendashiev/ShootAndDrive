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

        ShopItemAmmo[] ammoShopItems;

        public void Init(MenuController menuController)
        {
            int amount = Enum.GetValues(typeof(AmmunitionType)).Length;

            var inScene = itemsContainer.GetComponentsInChildren<ShopItemAmmo>();
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
            List<AmmunitionType> availableAmmo = inventory.Ammo.GetAvailableAmmo(availableWeapons);

            // activate needed
            for (int i = 0; i < availableAmmo.Count; i++)
            {
                ammoShopItems[i].gameObject.SetActive(true);

                var ammoItem = inventory.Ammo.Get(availableAmmo[i]);
                int priceAll = shop.GetAmmoPrice(ammoItem, ammoItem.MaxAmount - ammoItem.CurrentAmount);

                ammoShopItems[i].SetInfo(shop, ammoItem);
            }

            // deactivate other
            for (int i = availableAmmo.Count; i < ammoShopItems.Length; i++)
            {
                ammoShopItems[i].gameObject.SetActive(false);
            }
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}
