using System;
using System.Collections.Generic;
using UnityEngine;
using SD.Weapons;

namespace SD.PlayerLogic
{
    // Represents all weapons in player's inventory
    class WeaponsHolder : IWeaponsHolder
    {
        Dictionary<WeaponIndex, WeaponItem> playerWeapons;

        public WeaponsHolder()
        {
            playerWeapons = new Dictionary<WeaponIndex, WeaponItem>();
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
            foreach (WeaponIndex a in Enum.GetValues(typeof(WeaponIndex)))
            {
                if (!playerWeapons.ContainsKey(a))
                {
                    playerWeapons.Add(a, new WeaponItem(a, 0, false, true));
                    return;
                }

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
        public void Set(WeaponIndex weapon, int health, bool isBought, bool isSelected)
        {
            if (playerWeapons.ContainsKey(weapon))
            {
                playerWeapons[weapon].IsBought = isBought;
                playerWeapons[weapon].HealthRef.Value = health;
                playerWeapons[weapon].IsSelected = isSelected;

                return;
            }

            playerWeapons.Add(weapon, new WeaponItem(weapon, health, isBought, isSelected));
        }

        public WeaponItem Get(WeaponIndex w)
        {
            return playerWeapons[w];
        }

        IWeaponItem IWeaponsHolder.Get(WeaponIndex w)
        {
            return playerWeapons[w];
        }

        /// <summary>
        /// Is this weapon available? True, if weapon is bought and not broken
        /// </summary>
        public bool IsAvailable(WeaponIndex w)
        {
            return !playerWeapons[w].IsBroken && playerWeapons[w].IsBought;
        }

        public bool IsAvailableInGame(WeaponIndex w)
        {
            return !playerWeapons[w].IsBroken && playerWeapons[w].IsBought && playerWeapons[w].IsSelected;
        }

        public List<WeaponIndex> GetAvailableWeapons()
        {
            return GetWeapons(IsAvailable);
        }

        public List<WeaponIndex> GetAvailableWeaponsInGame()
        {
            return GetWeapons(IsAvailableInGame);
        }

        public List<WeaponIndex> GetWeapons(Func<WeaponIndex, bool> selector)
        {
            var available = new List<WeaponIndex>();

            foreach (WeaponIndex w in Enum.GetValues(typeof(WeaponIndex)))
            {
                if (selector(w))
                {
                    available.Add(w);
                }
            }

            return available;
        }

        public ICollection<WeaponItem> GetAll()
        {
            return playerWeapons.Values;
        }
    }
}
