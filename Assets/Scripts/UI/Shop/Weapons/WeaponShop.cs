using System;
using System.Collections.Generic;
using UnityEngine;

namespace SD.UI.Shop
{
    class WeaponShop : MonoBehaviour, IMenu
    {
        /// <summary>
        /// Prefab that contains ui weapon models and weapon item camera
        /// </summary>
        [SerializeField]
        GameObject          inventoryWorldUIPrefab;

        WeaponsWorldUI      weaponsWorldUI;

        /// <summary>
        /// Container of weapon items
        /// </summary>
        [SerializeField]
        Transform           itemsContainer;

        // only one in a scene, all info about current is loaded to this component
        ShopItemWeapon      weaponShopItem;
        WeaponIndex         currentWeapon;

        IInventory          inventory;
        IShop               shop;

        public void Init(MenuController menuController)
        {
            weaponShopItem = itemsContainer.GetComponentInChildren<ShopItemWeapon>(true);

            Debug.Assert(inventoryWorldUIPrefab.GetComponent<WeaponsWorldUI>() != null,
                "Prefab must contain 'InventoryWorldUI' script", inventoryWorldUIPrefab);

            var found = FindObjectOfType<WeaponsWorldUI>();

            if (found == null)
            {
                // create
                weaponsWorldUI = Instantiate(inventoryWorldUIPrefab).GetComponent<WeaponsWorldUI>();
            }
            else
            {
                weaponsWorldUI = found;
            }

            weaponsWorldUI.Deactivate();
        }

        public void Activate()
        {
            gameObject.SetActive(true);

            shop = GameController.Instance.Shop;
            inventory = GameController.Instance.Inventory;

            weaponsWorldUI.Activate();

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

            // get image
            var renderTexture = weaponsWorldUI.GetImage(w);

            //Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
            //RenderTexture.active = renderTexture;
            //texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            //texture2D.Apply();
            //byte[] png = texture2D.EncodeToPNG();
            //System.IO.File.WriteAllBytes("D:/UI/UI_Weapon_" + Enum.GetName(typeof(WeaponIndex), w) + ".png", png);
            //RenderTexture.active = null;
            //print("saved");

            // set info
            weaponShopItem.SetInfo(shop, weaponItem, ammoItem, renderTexture);
        }

        public void Deactivate()
        {
            weaponsWorldUI.Deactivate();
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
                    return 0;
                }
            }
            else
            {
                return (WeaponIndex)(amount - 1);
            }
        }
    }
}
