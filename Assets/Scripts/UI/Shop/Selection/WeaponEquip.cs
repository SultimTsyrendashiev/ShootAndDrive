using System;
using System.Collections.Generic;
using UnityEngine;

namespace SD.UI.Shop
{
    class WeaponEquip : MonoBehaviour, IMenu
    {
        [SerializeField]
        Transform content;
        [SerializeField]
        GameObject itemPrefab;

        [SerializeField]
        GameObject availableWeaponsObject;
        [SerializeField]
        GameObject noAvailableWeaponsObject;

        [SerializeField]
        Color defaultButtonColor;
        [SerializeField]
        Color selectedButtonColor;

        WeaponEquipItem[] items;
        IInventory inventory;

        public void Init(MenuController menuController)
        {
            var inScene = GetComponentsInChildren<WeaponEquipItem>(true);
            int curAmount = inScene.Length;

            var weaponIndices = Enum.GetValues(typeof(WeaponIndex));
            int amount = weaponIndices.Length;

            for (int i = curAmount; i < amount; i++)
            {
                Instantiate(itemPrefab, content);
            }

            if (inventory == null)
            {
                inventory = GameController.Instance.Inventory;
            }

            items = GetComponentsInChildren<WeaponEquipItem>(true);

            int j = 0;
            foreach(WeaponIndex index in weaponIndices)
            {
                items[j++].Init(inventory.Weapons.Get(index), 
                    inventory.Weapons.Select, inventory.Weapons.Deselect,
                    defaultButtonColor, selectedButtonColor);
            }
        }

        public void Activate()
        {
            UpdateList();
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        void UpdateList()
        {
            var available = inventory.Weapons.GetAvailableWeapons();

            availableWeaponsObject.SetActive(available.Count != 0);
            noAvailableWeaponsObject.SetActive(available.Count == 0);

            if (available.Count == 0)
            {
                return;
            }

            // disable all
            foreach (WeaponEquipItem i in items)
            {
                i.gameObject.SetActive(false);
            }
            
            // enable only available
            foreach (WeaponIndex index in available)
            {
                items[(int)index].gameObject.SetActive(true);
            }
        }
    }
}