using System;
using System.Collections.Generic;
using UnityEngine;

namespace SD.Weapons
{
    public class AmmoHolder
    {
        // Int as reference
        class IntWrapper
        {
            public int Value;
            public IntWrapper(int a = 0) { Value = a; }
        }

        Dictionary<AmmoType, IntWrapper> ammo;

        /// Default constructor, all values are 0
        public AmmoHolder()
        {
            ammo = new Dictionary<AmmoType, IntWrapper>();

            // add ammo types
            ammo.Add(AmmoType.Bullets, new IntWrapper());
            ammo.Add(AmmoType.BulletsHeavy, new IntWrapper());
            ammo.Add(AmmoType.BulletsPistol, new IntWrapper());
            ammo.Add(AmmoType.CannonBalls, new IntWrapper());
            ammo.Add(AmmoType.FireBottles, new IntWrapper());
            ammo.Add(AmmoType.Grenades, new IntWrapper());
            ammo.Add(AmmoType.Rockets, new IntWrapper());
            ammo.Add(AmmoType.Shells, new IntWrapper());

            // check if all types are added
            Debug.Assert(ammo.Keys.Count == Enum.GetValues(typeof(AmmoType)).Length, "Ammo holder::Not enough ammo types in dictionary");
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
