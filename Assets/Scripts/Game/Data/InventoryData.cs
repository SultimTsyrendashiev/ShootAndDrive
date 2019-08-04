using System;
using System.Collections.Generic;
using SD.Weapons;
using SD.PlayerLogic;
using UnityEngine;

namespace SD.Game.Data
{
    /// <summary>
    /// Special class for serialization
    /// </summary>
    [Serializable]
    class InventoryData
    {
        #region structs for serialization
        [Serializable]
        struct InvWeapon
        {
            public WeaponIndex  Index;
            public int          Health;
            public bool         IsBought;

            public InvWeapon(WeaponIndex index, int health, bool isBought)
            {
                Index = index;
                Health = health;
                IsBought = isBought;
            }
        }
       
        [Serializable]
        struct InvAmmo
        {
            public AmmunitionType   Type;
            public int              Amount;

            public InvAmmo(AmmunitionType type, int amount)
            {
                Type = type;
                Amount = amount;
            }
        }

        [Serializable]
        struct InvItem
        {
            public ItemType         Type;
            public int              Amount;

            public InvItem(ItemType type, int amount)
            {
                Type = type;
                Amount = amount;
            }
        }
        #endregion

        InvWeapon[]     SavedWeapons;
        InvAmmo[]       SavedAmmo;
        InvItem[]       SavedItems;
        int             SavedMoney;

        /// <summary>
        /// Load data from inventory to this class
        /// </summary>
        public void LoadFrom(PlayerInventory inventory)
        {
            // all arrays must be null
            Debug.Assert(SavedWeapons == null, "Weapons array in InventoryData must be null");
            Debug.Assert(SavedAmmo == null, "Ammo array in InventoryData must be null");
            Debug.Assert(SavedItems == null, "Items array in InventoryData must be null");

            List<InvWeapon> weapons = new List<InvWeapon>();
            List<InvAmmo> ammo = new List<InvAmmo>();
            List<InvItem> items = new List<InvItem>();

            // check each weapon
            foreach (WeaponIndex t in Enum.GetValues(typeof(WeaponIndex)))
            {
                // get from inventory
                var w = inventory.Weapons.Get(t);
                // save to the list
                weapons.Add(new InvWeapon(t, w.HealthRef.Value, w.IsBought));
            }

            foreach (AmmunitionType t in Enum.GetValues(typeof(AmmunitionType)))
            {
                // get from inventory
                var amount = inventory.Ammo.Get(t);
                // save to the list
                ammo.Add(new InvAmmo(t, amount));
            }

            foreach (ItemType t in Enum.GetValues(typeof(ItemType)))
            {
                // get from inventory
                var amount = inventory.Items.Get(t);
                // save to the list
                items.Add(new InvItem(t, amount));
            }

            // to arrays, so this class is ready for serialization
            SavedWeapons = weapons.ToArray();
            SavedAmmo = ammo.ToArray();
            SavedItems = items.ToArray();

            SavedMoney = inventory.Money;
        }

        /// <summary>
        /// Save data from this class to the inventory
        /// </summary>
        public void SaveTo(PlayerInventory inventory)
        {
            // all arrays must be not null
            Debug.Assert(SavedWeapons != null, "Weapons array in InventoryData must be not null");
            Debug.Assert(SavedAmmo != null, "Ammo array in InventoryData must be not null");
            Debug.Assert(SavedItems != null, "Items array in InventoryData must be not null");

            // load data from arrays to inventory

            foreach (var w in SavedWeapons)
            {
                inventory.Weapons.Set(w.Index, w.Health, w.IsBought);
            }

            foreach (var a in SavedAmmo)
            {
                inventory.Ammo.Set(a.Type, a.Amount);
            }

            foreach (var i in SavedItems)
            {
                inventory.Items.Set(i.Type, i.Amount);
            }

            inventory.Money = SavedMoney;
        }
    }
}
