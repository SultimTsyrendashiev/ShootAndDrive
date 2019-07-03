﻿using System;
using System.Collections.Generic;
using UnityEngine;
using SD.Weapons;

namespace SD.PlayerLogic
{
    /// <summary>
    /// Player's inventory.
    /// Holds all information about player's items.
    /// This class is singleton.
    /// </summary>
    class PlayerInventory : MonoBehaviour
    {
        public WeaponsHolder    Weapons;
        public AmmoHolder       Ammo;
        public ItemsHolder      Items;

        static PlayerInventory instance;
        public static PlayerInventory Instance => instance;

        void Awake()
        {
            if (instance != null)
            {
                return;
            }

            instance = this;

            Weapons = new WeaponsHolder();
            Ammo = new AmmoHolder();
            Items = new ItemsHolder();

            DontDestroyOnLoad(gameObject);
        }

        void OnDestroy()
        {
            // save data
            Save();
        }

        #region saving / loading
        /// <summary>
        /// Save inventory
        /// </summary>
        public void Save()
        {
            foreach (WeaponIndex w in Enum.GetValues(typeof(WeaponIndex)))
            {
                SaveWeapon(w);
            }

            foreach (AmmunitionType a in Enum.GetValues(typeof(AmmunitionType)))
            {
                SaveAmmo(a);
            }

            foreach (ItemType i in Enum.GetValues(typeof(ItemType)))
            {
                SaveItem(i);
            }
        }
        /// <summary>
        /// Load inventory to this class
        /// </summary>
        public void Load()
        {
            Weapons.Clear();

            foreach (WeaponIndex w in Enum.GetValues(typeof(WeaponIndex)))
            {
                LoadWeapon(w);
            }

            foreach (AmmunitionType a in Enum.GetValues(typeof(AmmunitionType)))
            {
                LoadAmmo(a);
            }

            foreach (ItemType i in Enum.GetValues(typeof(ItemType)))
            {
                LoadItem(i);
            }
        }

        void SaveWeapon(WeaponIndex w)
        {
            WeaponItem weapon = Weapons.Get(w);

            int bought = weapon.IsBought ? 1 : 0;
            PlayerPrefs.SetInt(GetNameB(w), bought);

            int health = weapon.GetHealthRef().Value;
            PlayerPrefs.SetInt(GetNameH(w), health);
        }

        void LoadWeapon(WeaponIndex w)
        {
            int bought = PlayerPrefs.GetInt(GetNameB(w), 0);
            int health = PlayerPrefs.GetInt(GetNameH(w), 0);

            Weapons.Add(w, health, bought == 1);
        }

        void SaveAmmo(AmmunitionType a)
        {
            int amount = Ammo.Get(a);
            PlayerPrefs.SetInt(GetAmmoName(a), amount);
        }
        
        void LoadAmmo(AmmunitionType a)
        {
            int amount = PlayerPrefs.GetInt(GetAmmoName(a), 0);
            Ammo.Set(a, amount);
        }

        void SaveItem(ItemType a)
        {
            int amount = Items.Get(a);
            PlayerPrefs.SetInt(GetItemName(a), amount);
        }

        void LoadItem(ItemType a)
        {
            int amount = PlayerPrefs.GetInt(GetItemName(a), 0);
            Items.Set(a, amount);
        }
        #endregion

        #region names
        string GetNameB(WeaponIndex w)
        {
            return AllWeaponsStats.Instance.Get(w).Name + "Bought";
        }

        string GetNameH(WeaponIndex w)
        {
            return AllWeaponsStats.Instance.Get(w).Name + "Health";
        }

        string GetAmmoName(AmmunitionType a)
        {
            return a.ToString();
        }
        string GetItemName(ItemType a)
        {
            return a.ToString();
        }
        #endregion

        public List<WeaponIndex> GetAvailableWeapons()
        {
            List<WeaponIndex> available = new List<WeaponIndex>();

            foreach (WeaponIndex w in Enum.GetValues(typeof(WeaponIndex)))
            {
                if (Weapons.IsAvailable(w))
                {
                    available.Add(w);
                }
            }

            return available;
        }

        #region cheats
        /// <summary>
        /// Give all to player
        /// </summary>
        public void GiveAll()
        {
            GiveAllWeapons();
            GiveAllAmmo();
        }

        /// <summary>
        /// Give all weapons to player
        /// </summary>
        public void GiveAllWeapons()
        {
            foreach (WeaponIndex w in Enum.GetValues(typeof(WeaponIndex)))
            {
                // TODO: remove, 1 is for test
                Weapons.SetHealth(w, AllWeaponsStats.Instance.Get(w).Durability);
                Weapons.SetBought(w, true);
            }
        }

        /// <summary>
        /// Give all ammo to player
        /// </summary>
        public void GiveAllAmmo()
        {
            foreach (AmmunitionType a in Enum.GetValues(typeof(AmmunitionType)))
            {
                Ammo.Set(a, AllAmmoStats.Instance.Get(a).MaxAmount);
            }
        }
        #endregion
    }
}
