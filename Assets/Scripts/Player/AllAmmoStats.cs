using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.Player
{
    /// <summary>
    /// Contains all ammo stats.
    /// For extending: add ammo in "AmmoType" enum 
    /// and use inheritance (overriding "AddAdditional" method)
    /// </summary>
    class AllAmmoStats
    {
        Dictionary<AmmoType, AmmoStats> ammoStats;
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
            ammoStats = new Dictionary<AmmoType, AmmoStats>();

            AddAmmoStats();
            AddAdditional();
        }

        private void AddAmmoStats()
        {
            ammoStats.Add(AmmoType.BulletsPistol,   new AmmoStats("Pistol Bullets", 21, 17));
            ammoStats.Add(AmmoType.Shells,          new AmmoStats("Shotgun Shells", 53, 10));
            ammoStats.Add(AmmoType.Bullets,         new AmmoStats("Rifle Bullets",  51, 30));
            ammoStats.Add(AmmoType.BulletsHeavy,    new AmmoStats("Heavy Bullets",  63, 30));
            ammoStats.Add(AmmoType.Grenades,        new AmmoStats("Grenade",        200, 1));
            ammoStats.Add(AmmoType.FireBottles,     new AmmoStats("Fire Bottle",    150, 1));
            ammoStats.Add(AmmoType.Rockets,         new AmmoStats("Rocket",         230, 1));
            ammoStats.Add(AmmoType.CannonBalls,     new AmmoStats("Cannonball",     350, 1));
        }

        /// <summary>
        /// Add additional ammo stats, used for addons, etc
        /// </summary>
        protected virtual void AddAdditional() { }
    }
}
