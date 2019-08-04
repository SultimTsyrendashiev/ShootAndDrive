using System;
using System.Collections.Generic;
using UnityEngine;
using SD.Weapons;

namespace SD.PlayerLogic
{
    // Represents all weapons in player's inventory
    class WeaponsHolder
    {
        Dictionary<WeaponIndex, WeaponItem> playerWeapons;

        public WeaponsHolder()
        {
            playerWeapons = new Dictionary<WeaponIndex, WeaponItem>();
        }

        public void Init()
        {
            foreach (WeaponIndex a in Enum.GetValues(typeof(WeaponIndex)))
            {
                playerWeapons.Add(a, new WeaponItem(a, 0, false));
            }
        }

        /// <summary>
        /// Sets default values
        /// </summary>
        public void SetDefault()
        {
            foreach (WeaponIndex a in Enum.GetValues(typeof(WeaponIndex)))
            {
                playerWeapons[a].HealthRef.Value = 0;
                playerWeapons[a].IsBought = false;
            }
        }

        public void Clear()
        {
            playerWeapons.Clear();
        }

        /// <summary>
        /// Sets perameters of the weapon.
        /// If there is no given weapon, it will be added.
        /// Otherwise, data will be rewritten.
        /// </summary>
        public void Set(WeaponIndex weapon, int health, bool isBought)
        {
            if (playerWeapons.ContainsKey(weapon))
            {
                playerWeapons[weapon].IsBought = isBought;
                playerWeapons[weapon].HealthRef.Value = health;

                return;
            }

            playerWeapons.Add(weapon, new WeaponItem(weapon, health, isBought));
        }

        public WeaponItem Get(WeaponIndex w)
        {
            return playerWeapons[w];
        }

        /// <summary>
        /// Is this weapon available? (Weapon is bought and not broken)
        /// </summary>
        public bool IsAvailable(WeaponIndex w)
        {
            return !playerWeapons[w].IsBroken && playerWeapons[w].IsBought;
        }
    }
}
