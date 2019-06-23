using System;
using System.Collections.Generic;
using UnityEngine;
using SD.Weapons;

namespace SD.Player
{
    // Represents special items in player's inventory
    class ItemsHolder
    {
        Dictionary<ItemType, RefInt> playerItems;

        public ItemsHolder()
        {
            playerItems = new Dictionary<ItemType, RefInt>();

            AddAmmoTypes();
            AddAdditional();

            // check if all types are added
            Debug.Assert(playerItems.Keys.Count == Enum.GetValues(typeof(AmmoType)).Length, "ItemHolder::Not enough item types in dictionary");
        }

        private void AddAmmoTypes()
        {
            // add ammo types
            playerItems.Add(ItemType.Medkit, new RefInt());
        }

        /// <summary>
        /// Add additional ammo stats, used for addons, etc
        /// </summary>
        protected virtual void AddAdditional() { }

        public int Get(ItemType type)
        {
            return playerItems[type].Value;
        }

        public void Set(ItemType type, int amount)
        {
            playerItems[type].Value = amount;
        }

        public void Add(ItemType type, int toAdd)
        {
            playerItems[type].Value += toAdd;
        }
    }
}
