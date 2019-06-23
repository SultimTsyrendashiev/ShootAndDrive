using System;
using System.Collections.Generic;
using UnityEngine;
using SD.Weapons;

namespace SD.Player
{
    // Represents player's ammo
    class AmmoHolder
    {
        Dictionary<AmmoType, RefInt> ammo;

        /// Default constructor, all values are 0
        public AmmoHolder()
        {
            ammo = new Dictionary<AmmoType, RefInt>();

            AddAmmoTypes();
            AddAdditional();

            // check if all types are added
            Debug.Assert(ammo.Keys.Count == Enum.GetValues(typeof(AmmoType)).Length, "AmmoHolder::Not enough ammo types in dictionary");
        }

        private void AddAmmoTypes()
        {
            // add ammo types
            ammo.Add(AmmoType.Bullets, new RefInt());
            ammo.Add(AmmoType.BulletsHeavy, new RefInt());
            ammo.Add(AmmoType.BulletsPistol, new RefInt());
            ammo.Add(AmmoType.CannonBalls, new RefInt());
            ammo.Add(AmmoType.FireBottles, new RefInt());
            ammo.Add(AmmoType.Grenades, new RefInt());
            ammo.Add(AmmoType.Rockets, new RefInt());
            ammo.Add(AmmoType.Shells, new RefInt());
        }

        /// <summary>
        /// Add additional ammo stats, used for addons, etc
        /// </summary>
        protected virtual void AddAdditional() { }

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
