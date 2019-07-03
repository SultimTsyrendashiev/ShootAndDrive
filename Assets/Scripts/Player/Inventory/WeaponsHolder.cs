using System;
using System.Collections.Generic;
using UnityEngine;
using SD.Weapons;

namespace SD.PlayerLogic
{
    // Represents all weapons in player's inventory
    class WeaponsHolder
    {
        [SerializeField]
        Dictionary<WeaponIndex, WeaponItem> playerWeapons;

        public WeaponsHolder()
        {
            playerWeapons = new Dictionary<WeaponIndex, WeaponItem>();
        }

        public void Clear()
        {
            playerWeapons.Clear();
        }

        public void Add(WeaponIndex weapon, int health, bool isBought)
        {
            if (playerWeapons.ContainsKey(weapon))
            {
                Debug.LogWarning("Weapon is already added");
            }

            playerWeapons.Add(weapon, new WeaponItem(weapon, health, isBought));
        }

        public WeaponItem Get(WeaponIndex w)
        {
            return playerWeapons[w];
        }

        public void SetHealth(WeaponIndex w, int health)
        {
            playerWeapons[w].GetHealthRef().Value = health;
        }

        public void SetBought(WeaponIndex w, bool isBought)
        {
            playerWeapons[w].IsBought = isBought;
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
