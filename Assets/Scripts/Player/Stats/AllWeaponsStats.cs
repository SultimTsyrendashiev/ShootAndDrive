using System;
using System.Collections.Generic;
using UnityEngine;
using SD.Weapons;

namespace SD.PlayerLogic
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
            weapons.Add(WeaponIndex.Pistol,             new WeaponStats("Pistol", AmmunitionType.BulletsPistol, 
                1500, 1500, 10, 0.2f, 0.2f));
            weapons.Add(WeaponIndex.PistolHeavy,        new WeaponStats("Heavy Pistol", AmmunitionType.BulletsPistol, 
                3200, 750, 24, 0.4f, 0.2f));
            weapons.Add(WeaponIndex.PistolRevolver,     new WeaponStats("Revolver", AmmunitionType.BulletsPistol, 
                5000, 650, 20, 0.6f, 0.1f));

            weapons.Add(WeaponIndex.SmgMicro,           new WeaponStats("Micro SMG", AmmunitionType.BulletsPistol, 
                4000, 2000, 10, 0.06f, 0.4f));
            weapons.Add(WeaponIndex.SmgTactic,          new WeaponStats("Tactic SMG", AmmunitionType.BulletsPistol, 
                10000, 5500, 5, 0.1f, 0.1f));
            weapons.Add(WeaponIndex.SmgHeavy,           new WeaponStats("Heavy SMG", AmmunitionType.BulletsPistol, 
                7000, 4000, 15, 0.1f, 0.3f));
            weapons.Add(WeaponIndex.SmgClassic,         new WeaponStats("Classic SMG", AmmunitionType.BulletsPistol, 
                11000, 4500, 8, 0.08f, 0.2f));

            weapons.Add(WeaponIndex.Shotgun,            new WeaponStats("Shotgun", AmmunitionType.Shells, 
                4500, 600, 7, 0.75f, 0.42f));
            weapons.Add(WeaponIndex.ShotgunAuto,        new WeaponStats("Auto Shotgun", AmmunitionType.Shells, 
                8000, 500, 7, 0.5f, 0.55f));
            weapons.Add(WeaponIndex.ShotgunDouble,      new WeaponStats("Double Barrel Shotgun", AmmunitionType.Shells, 
                9000, 2200, 10, 1.0f, 0.7f));

            weapons.Add(WeaponIndex.RifleAssault,       new WeaponStats("Assault Rifle", AmmunitionType.Bullets, 
                13000, 4200, 20, 0.08f, 0.2f));
            weapons.Add(WeaponIndex.RifleCarbine,       new WeaponStats("Carbine Rifle", AmmunitionType.Bullets, 
                14000, 2800, 25, 0.1f, 0.2f));

            weapons.Add(WeaponIndex.HMachineGun,        new WeaponStats("Machine Gun", AmmunitionType.BulletsHeavy, 
                21000, 4500, 40, 0.14f, 0.4f));
            weapons.Add(WeaponIndex.HMinigun,           new WeaponStats("Minigun", AmmunitionType.BulletsHeavy, 
                35000, 16000, 20, 0.04f, 0.4f));

            weapons.Add(WeaponIndex.TGrenade,           new WeaponStats("Grenade", AmmunitionType.Grenades, 
                200, int.MaxValue, 50, 1.0f, 0));
            weapons.Add(WeaponIndex.TFireBottle,        new WeaponStats("Fire Bottle", AmmunitionType.FireBottles, 
                150, int.MaxValue, 50, 1.0f, 0));

            weapons.Add(WeaponIndex.LauncherRocket,     new WeaponStats("Rocket Launcher", AmmunitionType.Rockets, 
                28000, 1200, 100, 1.25f, 0));
            weapons.Add(WeaponIndex.LauncherGrenade,    new WeaponStats("Grenade Launcher", AmmunitionType.Rockets, 
                30000, 2800, 80, 1f, 0));
            weapons.Add(WeaponIndex.LauncherCannon,     new WeaponStats("Cannon", AmmunitionType.Cannonballs, 
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

        public bool CanJam(AmmunitionType a)
        {
            return !(a == AmmunitionType.Cannonballs || a == AmmunitionType.FireBottles || a == AmmunitionType.Grenades);
        }

        public bool CanBreak(AmmunitionType a)
        {
            return !(a == AmmunitionType.Cannonballs || a == AmmunitionType.FireBottles || a == AmmunitionType.Grenades);
        }
    }
}
