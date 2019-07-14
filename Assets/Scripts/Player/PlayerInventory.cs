using System;
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
    class PlayerInventory
    {
        public WeaponsHolder Weapons { get; private set; }
        public AmmoHolder Ammo { get; private set; }
        public ItemsHolder Items { get; private set; }

        public PlayerInventory()
        {
            Weapons = new WeaponsHolder();
            Ammo = new AmmoHolder();
            Items = new ItemsHolder();
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
            PlayerPrefs.SetInt(GetNameB(weapon), bought);

            int health = weapon.HealthRef.Value;
            PlayerPrefs.SetInt(GetNameH(weapon), health);
        }

        void LoadWeapon(WeaponIndex w)
        {
            WeaponItem weapon = Weapons.Get(w);

            int bought = PlayerPrefs.GetInt(GetNameB(weapon), 0);
            int health = PlayerPrefs.GetInt(GetNameH(weapon), 0);

            Weapons.Set(w, health, bought == 1);
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
        string GetNameB(WeaponItem w)
        {
            return w.Stats.Name + "Bought";
        }

        string GetNameH(WeaponItem w)
        {
            return w.Stats.Name + "Health";
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
            //            // only in editor
            //#if !UNITY_EDITOR
            //            return;
            //#endif

            var stats = UnityEngine.Object.FindObjectOfType<GameController>().WeaponsStats;

            foreach (WeaponIndex w in Enum.GetValues(typeof(WeaponIndex)))
            {
                // TODO: remove, 1 is for test
                Weapons.SetHealth(w, stats[w].Durability);
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
