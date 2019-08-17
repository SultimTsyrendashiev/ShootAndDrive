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
        Dictionary<AmmunitionType, AmmoData> ammoStats;

        public AllAmmoStats(AmmoData[] data)
        {
            ammoStats = new Dictionary<AmmunitionType, AmmoData>();
            AddAmmoStats(data);

            // check if all types are added
            Debug.Assert(ammoStats.Keys.Count == Enum.GetValues(typeof(AmmunitionType)).Length, "AllAmmoStats::Not enough ammo types in dictionary");
        }

        void AddAmmoStats(AmmoData[] data)
        {
            foreach (var a in data)
            {
                ammoStats.Add(a.Type, a);
            }
        }

        public AmmoData this[AmmunitionType a]
        {
            get
            {
                return ammoStats[a];
            }
        }
    }
}
