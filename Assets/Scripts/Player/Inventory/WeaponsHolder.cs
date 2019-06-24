using System;
using System.Collections.Generic;
using UnityEngine;
using SD.Weapons;

namespace SD.Player
{
    // Represents all weapons in player's inventory
    class WeaponsHolder
    {
        [SerializeField]
        Dictionary<WeaponsEnum, WeaponItem> playerWeapons;

        public WeaponsHolder()
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
                Debug.LogWarning("Weapon is already added");
            }

            playerWeapons.Add(weapon, new WeaponItem(weapon, health, isBought));
        }

        public WeaponItem Get(WeaponsEnum w)
        {
            return playerWeapons[w];
        }

        public void SetHealth(WeaponsEnum w, float health)
        {
            playerWeapons[w].Health = health;
        }

        public void SetBought(WeaponsEnum w, bool isBought)
        {
            playerWeapons[w].IsBought = isBought;
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
