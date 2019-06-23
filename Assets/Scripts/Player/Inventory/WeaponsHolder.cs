using System;
using System.Collections.Generic;
using UnityEngine;
using SD.Weapons;

namespace SD.Player
{
    // Represents all weapons in player's inventory
    class WeaponsHolder
    {
        Dictionary<WeaponsEnum, WeaponItem> playerWeapons;

        private WeaponsHolder()
        {
            playerWeapons = new Dictionary<WeaponsEnum, WeaponItem>();
        }

        public void Clear()
        {
            playerWeapons.Clear();
        }

        public void Add(WeaponsEnum weapon, float health, bool isBought)
        {
            if (playerWeapons.ContainsKey(weapon))
            {
                Debug.LogWarning("Weapon is already contained");
            }

            // todo
        }

        public WeaponItem Get(WeaponsEnum weapon)
        {
            return playerWeapons[weapon];
        }

        /// <summary>
        /// Is this weapon available? (Weapon is bought and not broken)
        /// </summary>
        public bool IsAvailable(WeaponsEnum w)
        {
            return !playerWeapons[w].IsBroken && playerWeapons[w].IsBought;
        }
    }
}
