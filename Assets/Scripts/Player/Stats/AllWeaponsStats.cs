using System;
using System.Collections.Generic;
using UnityEngine;
using SD.Weapons;

namespace SD.Player
{
    /// <summary>
    /// Contains all weapons stats.
    /// For extending: add weapon in "WeaponIndex" enum 
    /// and use inheritance (overriding "AddAdditional" method)
    /// </summary>
    class AllWeaponsStats
    {
        Dictionary<WeaponIndex, WeaponStats> weapons;
        static AllWeaponsStats instance;

        public static AllWeaponsStats Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AllWeaponsStats();
                }

                return instance;
            }
        }

        private AllWeaponsStats()
        {
            weapons = new Dictionary<WeaponIndex, WeaponStats>();

            AddWeapons();
            AddAdditional();

            // check if all types are added
            Debug.Assert(weapons.Keys.Count == Enum.GetValues(typeof(WeaponIndex)).Length, "AllWeaponsStats::Not enough weapons types in dictionary");
        }

        /// <summary>
        /// Add main weapons stats
        /// </summary>
        private void AddWeapons()
        {
            weapons.Add(WeaponIndex.Pistol,             new WeaponStats("Pistol", AmmoType.BulletsPistol, 
                1500, 1500, 10, 0.2f, 0.2f));
            weapons.Add(WeaponIndex.PistolHeavy,        new WeaponStats("Heavy Pistol", AmmoType.BulletsPistol, 
                3200, 750, 24, 0.4f, 0.2f));
            weapons.Add(WeaponIndex.PistolRevolver,     new WeaponStats("Revolver", AmmoType.BulletsPistol, 
                5000, 650, 20, 0.6f, 0.1f));

            weapons.Add(WeaponIndex.SmgMicro,           new WeaponStats("Micro SMG", AmmoType.BulletsPistol, 
                4000, 2000, 10, 0.06f, 0.4f));
            weapons.Add(WeaponIndex.SmgTactic,          new WeaponStats("Tactic SMG", AmmoType.BulletsPistol, 
                10000, 5500, 5, 0.1f, 0.1f));
            weapons.Add(WeaponIndex.SmgHeavy,           new WeaponStats("Heavy SMG", AmmoType.BulletsPistol, 
                7000, 4000, 15, 0.1f, 0.3f));
            weapons.Add(WeaponIndex.SmgClassic,         new WeaponStats("Classic SMG", AmmoType.BulletsPistol, 
                11000, 4500, 8, 0.08f, 0.2f));

            weapons.Add(WeaponIndex.Shotgun,            new WeaponStats("Shotgun", AmmoType.Shells, 
                4500, 600, 7, 0.75f, 0.3f));
            weapons.Add(WeaponIndex.ShotgunAuto,        new WeaponStats("Auto Shotgun", AmmoType.Shells, 
                8000, 500, 7, 0.5f, 0.6f));
            weapons.Add(WeaponIndex.ShotgunDouble,      new WeaponStats("Double Barrel Shotgun", AmmoType.Shells, 
                9000, 2200, 10, 1.0f, 0.8f));

            weapons.Add(WeaponIndex.RifleAssault,       new WeaponStats("Assault Rifle", AmmoType.Bullets, 
                13000, 4200, 20, 0.08f, 0.2f));
            weapons.Add(WeaponIndex.RifleCarbine,       new WeaponStats("Carbine Rifle", AmmoType.Bullets, 
                14000, 2800, 25, 0.1f, 0.2f));

            weapons.Add(WeaponIndex.HMachineGun,        new WeaponStats("Machine Gun", AmmoType.BulletsHeavy, 
                21000, 4500, 40, 0.14f, 0.4f));
            weapons.Add(WeaponIndex.HMinigun,           new WeaponStats("Minigun", AmmoType.BulletsHeavy, 
                35000, 16000, 20, 0.04f, 0.4f));

            weapons.Add(WeaponIndex.TGrenade,           new WeaponStats("Grenade", AmmoType.Grenades, 
                200, int.MaxValue, 50, 1.0f, 0));
            weapons.Add(WeaponIndex.TFireBottle,        new WeaponStats("Fire Bottle", AmmoType.FireBottles, 
                150, int.MaxValue, 50, 1.0f, 0));

            weapons.Add(WeaponIndex.LauncherRocket,     new WeaponStats("Rocket Launcher", AmmoType.Rockets, 
                28000, 1200, 100, 1.25f, 0));
            weapons.Add(WeaponIndex.LauncherGrenade,    new WeaponStats("Grenade Launcher", AmmoType.Rockets, 
                30000, 2800, 80, 1f, 0));
            weapons.Add(WeaponIndex.LauncherCannon,     new WeaponStats("Cannon", AmmoType.Cannonballs, 
                60000, 3000, 500, 2f, 0));
        }

        /// <summary>
        /// Add additional weapons stats, used for addons, etc
        /// </summary>
        protected virtual void AddAdditional() { }

        public WeaponStats Get(WeaponIndex weapon)
        {
            return weapons[weapon];
        }

        public bool CanJam(AmmoType a)
        {
            return !(a == AmmoType.Cannonballs || a == AmmoType.FireBottles || a == AmmoType.Grenades);
        }
    }
}
