using System;
using System.Collections.Generic;
using UnityEngine;

namespace SD.UI.Shop
{
    class WeaponEquip : MonoBehaviour, IMenu
    {
        // TODO
        int maxWeaponAmount = 8;

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
        // actually, unique queue
        LinkedList<WeaponIndex> selectedWeapons;
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
                items[j].Set(inventory.Weapons.Get(index), Select, Remove);
                items[j].SetColors(defaultButtonColor, selectedButtonColor);
                j++;
            }

            selectedWeapons = new LinkedList<WeaponIndex>();
        }

        public void Activate()
        {
            gameObject.SetActive(true);
            UpdateList();
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        void UpdateList()
        {
            // list must be refilled
            selectedWeapons.Clear();
         
            var available = inventory.Weapons.GetAvailableWeapons();

            availableWeaponsObject.SetActive(available.Count != 0);
            noAvailableWeaponsObject.SetActive(available.Count == 0);

            if (available.Count == 0)
            {
                return;
            }

            foreach (var i in items)
            {
                i.gameObject.SetActive(false);
                i.SetSelection(false);
            }
            
            // enable only available
            foreach (WeaponIndex index in available)
            {
                items[(int)index].gameObject.SetActive(true);
            }

            var selected = inventory.Weapons.GetAvailableWeaponsInGame();

            foreach (WeaponIndex index in selected)
            {
                // add to list and check
                Select(index);
            }

            // mark weapons
            foreach (WeaponIndex index in selectedWeapons)
            {
                items[(int)index].SetSelection(true);
            }
        }

        // TODO: remove from here. Max amount must be checked in weapons holder
        void Select(WeaponIndex index)
        {
            if (!selectedWeapons.Contains(index))
            {
                selectedWeapons.AddLast(index);
                inventory.Weapons.Get(index).IsSelected = true;
            }
            else
            {
                // remove and add to the end of list
                selectedWeapons.Remove(index);
                selectedWeapons.AddLast(index);
            }

            while (selectedWeapons.Count > maxWeaponAmount)
            {
                // remove beginning
                Remove(selectedWeapons.First.Value);
            }
        }

        void Remove(WeaponIndex index)
        {
            // using linked list as queue doesn't have remove method
            selectedWeapons.Remove(index);
            inventory.Weapons.Get(index).IsSelected = false;
        }
    }
}