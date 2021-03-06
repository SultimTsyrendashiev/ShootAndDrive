﻿using System;
using System.Collections.Generic;
using UnityEngine;
using SD.Weapons;

namespace SD.PlayerLogic
{
    // Represents special items in player's inventory
    class ItemsHolder
    {
        Dictionary<ItemType, RefInt> playerItems;

        public ItemsHolder()
        {
            playerItems = new Dictionary<ItemType, RefInt>();
        }

        public void Init()
        {
            foreach (ItemType a in Enum.GetValues(typeof(ItemType)))
            {
                playerItems.Add(a, new RefInt());
            }
        }

        /// <summary>
        /// Sets default values
        /// </summary>
        public void SetDefault()
        {
            foreach (ItemType a in Enum.GetValues(typeof(ItemType)))
            {
                playerItems[a].Value = 0;
            }
        }

        public int this[ItemType type]
        {
            get
            {
                return playerItems[type].Value;
            }
            set
            {
                playerItems[type].Value = value;
            }
        }

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
