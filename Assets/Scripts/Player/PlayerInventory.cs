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
        public int Money { get; set; }

        /// <summary>
        /// Sets default values
        /// </summary>
        public PlayerInventory()
        {
            Weapons = new WeaponsHolder();
            Ammo = new AmmoHolder();
            Items = new ItemsHolder();

            // init
            Weapons.Init();
            Ammo.Init();
            Items.Init();
            Money = 0;
        }

        /// <summary>
        /// Save inventory
        /// </summary>
        public void Save()
        {
            Game.Data.DataSystem.SaveInventory(this);
        }
        /// <summary>
        /// Load inventory to this class
        /// </summary>
        public void Load()
        {
            Game.Data.DataSystem.LoadInventory(this);
        }

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
                Weapons.Set(w, stats[w].Durability, true);
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

        public void SetDefault()
        {
            Weapons.SetDefault();
            Ammo.SetDefault();
            Items.SetDefault();
        }
        #endregion
    }
}
