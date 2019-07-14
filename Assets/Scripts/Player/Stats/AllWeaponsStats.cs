using System;
using System.Collections.Generic;
using UnityEngine;

namespace SD.Weapons
{
    /// <summary>
    /// Contains all weapons stats.
    /// This component can be attached only to game cotroller.
    /// This class is component as there must be a reference to weapons list object.
    /// For extending: add weapon in "WeaponIndex" enum 
    /// and add weapon stats to this list.
    /// </summary>
    [RequireComponent(typeof(GameController))]
    class AllWeaponsStats : MonoBehaviour
    {
        [SerializeField]
        WeaponsList weaponsList;

        Dictionary<WeaponIndex, WeaponData> weapons;

        public void Init()
        {
            weapons = new Dictionary<WeaponIndex, WeaponData>();
            AddWeapons();

            // check if all types are added
            Debug.Assert(weapons.Keys.Count == Enum.GetValues(typeof(WeaponIndex)).Length, "Not enough weapons types in dictionary", this);
        }

        /// <summary>
        /// Add main weapons stats
        /// </summary>
        void AddWeapons()
        {
            var weaponsData = weaponsList.Data;

            foreach (var w in weaponsData)
            {
                w.TestCalculateEnum();
                weapons.Add(w.Index, w);
            }
        }

        public WeaponData this[WeaponIndex weapon]
        {
            get
            {
                return weapons[weapon];
            }
        }

        public WeaponData Get(WeaponIndex weapon)
        {
            return this[weapon];
        }

        public static bool CanJam(AmmunitionType a)
        {
            return !(a == AmmunitionType.Cannonballs || a == AmmunitionType.FireBottles || a == AmmunitionType.Grenades);
        }

        public static bool CanBreak(AmmunitionType a)
        {
            return !(a == AmmunitionType.Cannonballs || a == AmmunitionType.FireBottles || a == AmmunitionType.Grenades);
        }
    }
}
