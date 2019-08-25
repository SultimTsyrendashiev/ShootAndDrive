using System;
using System.Collections.Generic;
using UnityEngine;
using SD.Weapons;

namespace SD.PlayerLogic
{
    // Represents ammo in player's inventory
    class AmmoHolder : IAmmoHolder
    {
        Dictionary<AmmunitionType, AmmoItem> ammo;

        /// Default constructor, all values are 0
        public AmmoHolder()
        {
            ammo = new Dictionary<AmmunitionType, AmmoItem>();
        }

        public void Init()
        {
            SetDefault();
        }

        /// <summary>
        /// Sets default values
        /// </summary>
        public void SetDefault()
        {
            foreach (AmmunitionType a in Enum.GetValues(typeof(AmmunitionType)))
            {
                ammo.Add(a, new AmmoItem(a, 0));
            }
        }

        public void Set(AmmunitionType type, int amount)
        {
            int max = ammo[type].MaxAmount;
            ammo[type].CurrentAmount = Mathf.Clamp(amount, 0, max); 
        }

        public void Add(AmmunitionType type, int toAdd)
        {
            int max = ammo[type].MaxAmount;
            int newAmount = ammo[type].CurrentAmount + toAdd;

            ammo[type].CurrentAmount = Mathf.Clamp(newAmount, 0, max);
        }

        IAmmoItem IAmmoHolder.Get(AmmunitionType type)
        {
            return ammo[type];
        }

        public List<AmmunitionType> GetAmmo(Func<AmmunitionType, bool> selector)
        {
            var available = new List<AmmunitionType>();

            foreach (AmmunitionType a in Enum.GetValues(typeof(AmmunitionType)))
            {
                if (selector(a))
                {
                    available.Add(a);
                }
            }

            return available;
        }

        public List<AmmunitionType> GetAvailableAmmo()
        {
            return GetAmmo((AmmunitionType type) => ammo[type].CurrentAmount > 0);
        }

        public List<AmmunitionType> GetAvailableAmmo(List<WeaponIndex> weapons)
        {
            return GetAmmo((AmmunitionType type) => ammo[type].CurrentAmount > 0 && IsNecessary(type, weapons));
        }

        public List<AmmunitionType> GetNecessaryAmmo(List<WeaponIndex> weapons)
        {
            return GetAmmo((AmmunitionType type) => IsNecessary(type, weapons));
        }

        /// <summary>
        /// Is this ammo type necessary for at least one weapon from the list?
        /// </summary>
        bool IsNecessary(AmmunitionType type, List<WeaponIndex> weapons)
        {
            var weaponsStats = GameController.Instance.WeaponsStats;

            foreach (var w in weapons)
            {
                if (weaponsStats[w].AmmoType == type)
                {
                    return true;
                }
            }

            return false;
        }

        public ICollection<AmmoItem> GetAll()
        {
            return ammo.Values;
        }
    }
}
