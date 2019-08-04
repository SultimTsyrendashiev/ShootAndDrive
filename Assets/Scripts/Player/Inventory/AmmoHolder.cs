﻿using System;
using System.Collections.Generic;
using UnityEngine;
using SD.Weapons;

namespace SD.PlayerLogic
{
    // Represents ammo in player's inventory
    class AmmoHolder
    {
        Dictionary<AmmunitionType, RefInt> ammo;

        /// Default constructor, all values are 0
        public AmmoHolder()
        {
            ammo = new Dictionary<AmmunitionType, RefInt>();
        }

        public void Init()
        {
            foreach (AmmunitionType a in Enum.GetValues(typeof(AmmunitionType)))
            {
                ammo.Add(a, new RefInt());
            }
        }

        /// <summary>
        /// Sets default values
        /// </summary>
        public void SetDefault()
        {
            foreach (AmmunitionType a in Enum.GetValues(typeof(AmmunitionType)))
            {
                ammo[a].Value = 0;
            }
        }

        public int this[AmmunitionType type]
        {
            get
            {
                return ammo[type].Value;
            }
            set
            {
                ammo[type].Value = value;
            }
        }

        public int Get(AmmunitionType type)
        {
            return ammo[type].Value;
        }

        public void Set(AmmunitionType type, int amount)
        {
            ammo[type].Value = amount;
        }

        public void Add(AmmunitionType type, int toAdd)
        {
            ammo[type].Value += toAdd;
        }
    }
}
