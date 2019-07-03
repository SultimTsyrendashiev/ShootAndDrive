using System;
using System.Collections.Generic;
using SD.Weapons;
using UnityEngine;

namespace SD.PlayerLogic
{
    /// <summary>
    /// Contains all ammo stats.
    /// For extending: add ammo in "AmmoType" enum 
    /// and use inheritance (overriding "AddAdditional" method)
    /// </summary>
    class AllAmmoStats
    {
        Dictionary<AmmunitionType, AmmoStats> ammoStats;
        static AllAmmoStats instance;

        public static AllAmmoStats Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AllAmmoStats();
                }

                return instance;
            }
        }

        private AllAmmoStats()
        {
            ammoStats = new Dictionary<AmmunitionType, AmmoStats>();

            AddAmmoStats();
            AddAdditional();

            // check if all types are added
            Debug.Assert(ammoStats.Keys.Count == Enum.GetValues(typeof(AmmunitionType)).Length, "AllAmmoStats::Not enough ammo types in dictionary");
        }

        private void AddAmmoStats()
        {
            ammoStats.Add(AmmunitionType.BulletsPistol,   new AmmoStats("Pistol Bullets", 21, 17, 1100));
            ammoStats.Add(AmmunitionType.Shells,          new AmmoStats("Shotgun Shells", 53, 10, 200));
            ammoStats.Add(AmmunitionType.Bullets,         new AmmoStats("Rifle Bullets",  51, 30, 1300));
            ammoStats.Add(AmmunitionType.BulletsHeavy,    new AmmoStats("Heavy Bullets",  63, 30, 2400));
            ammoStats.Add(AmmunitionType.Grenades,        new AmmoStats("Grenade",        200, 1, 60));
            ammoStats.Add(AmmunitionType.FireBottles,     new AmmoStats("Fire Bottle",    150, 1, 85));
            ammoStats.Add(AmmunitionType.Rockets,         new AmmoStats("Rocket",         230, 1, 120));
            ammoStats.Add(AmmunitionType.Cannonballs,     new AmmoStats("Cannonball",     350, 1, 50));
        }

        /// <summary>
        /// Add additional ammo stats, used for addons, etc
        /// </summary>
        protected virtual void AddAdditional() { }

        public AmmoStats Get(AmmunitionType a)
        {
            return ammoStats[a];
        }
    }
}
