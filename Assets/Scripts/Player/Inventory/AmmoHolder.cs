using System;
using System.Collections.Generic;
using UnityEngine;
using SD.Weapons;

namespace SD.Player
{
    // Represents ammo in player's inventory
    class AmmoHolder
    {
        Dictionary<AmmoType, RefInt> ammo;

        /// Default constructor, all values are 0
        public AmmoHolder()
        {
            ammo = new Dictionary<AmmoType, RefInt>();

            foreach (AmmoType a in Enum.GetValues(typeof(AmmoType)))
            {
                ammo.Add(a, new RefInt());
            }
        }

        public int this[AmmoType type]
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

        public int Get(AmmoType type)
        {
            return ammo[type].Value;
        }

        public void Set(AmmoType type, int amount)
        {
            ammo[type].Value = amount;
        }

        public void Add(AmmoType type, int toAdd)
        {
            ammo[type].Value += toAdd;
        }
    }
}
