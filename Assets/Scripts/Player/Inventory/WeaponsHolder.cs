using System;
using System.Collections.Generic;
using UnityEngine;
using SD.Utils.Collections;

namespace SD.PlayerLogic
{
    // Represents all weapons in player's inventory
    class WeaponsHolder : IWeaponsHolder
    {
        Dictionary<WeaponIndex, WeaponItem> playerWeapons;

        // TODO: make variable
        const int MaxSelectedWeaponAmount = 8;
        UniqueQueue<WeaponIndex> selectedWeapons;

        public WeaponsHolder()
        {
            playerWeapons = new Dictionary<WeaponIndex, WeaponItem>();
            selectedWeapons = new UniqueQueue<WeaponIndex>(MaxSelectedWeaponAmount);
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
                    playerWeapons.Add(a, new WeaponItem(this, a, 0, false));
                }
                else
                {
                    playerWeapons[a].HealthRef.Value = 0;
                    playerWeapons[a].IsBought = false;
                }
            }

            selectedWeapons.Clear();
        }

        public void Clear()
        {
            playerWeapons.Clear();
            selectedWeapons.Clear();
        }

        /// <summary>
        /// Sets perameters of the weapon.
        /// If there is no given weapon, it will be added.
        /// Otherwise, data will be rewritten.
        /// </summary>
        public void Set(WeaponIndex weapon, int health, bool isBought, bool isSelected)
        {
            if (isSelected)
            {
                selectedWeapons.Push(weapon);
            }
            else
            {
                selectedWeapons.Remove(weapon);
            }

            if (playerWeapons.ContainsKey(weapon))
            {
                playerWeapons[weapon].IsBought = isBought;
                playerWeapons[weapon].HealthRef.Value = health;
            }
            else
            {
                playerWeapons.Add(weapon, new WeaponItem(this, weapon, health, isBought));
            }
        }

        public WeaponItem Get(WeaponIndex w)
        {
            WeaponItem item = playerWeapons[w];

            // check weapon's health, if it's out of bounds, clamp it
            item.Health = Mathf.Clamp(item.Health, 0, item.Durability);

            // normalize other values
            if (item.Health == 0 && !item.IsAmmo)
            {
                item.IsBought = false;
                selectedWeapons.Remove(w);
            }

            return item;
        }

        IWeaponItem IWeaponsHolder.Get(WeaponIndex w)
        {
            return Get(w);
        }

        /// <summary>
        /// Is this weapon available? True, if weapon is bought and not broken.
        /// Weapon is not broken if health > 0 or weapon is ammo
        /// </summary>
        public bool IsAvailable(WeaponIndex w)
        {
            return !playerWeapons[w].IsBroken && playerWeapons[w].IsBought;
        }

        public bool IsAvailableInGame(WeaponIndex w)
        {
            //return playerWeapons[w].IsBought;

            return !playerWeapons[w].IsBroken
                && playerWeapons[w].IsBought
                && playerWeapons[w].IsSelected;
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

        public bool ContainsAtLeastOne()
        {
            foreach (IWeaponItem w in playerWeapons.Values)
            {
                if (w.IsBought)
                {
                    return true;
                }
            }

            return false;
        }

        public void Select(WeaponIndex index)
        {
            selectedWeapons.Push(index);
        }

        public void Deselect(WeaponIndex index)
        {
            selectedWeapons.Remove(index);
        }

        public bool IsSelected(WeaponIndex index)
        {
            WeaponItem item = playerWeapons[index];
            return selectedWeapons.Contains(index) && !(item.Health == 0 && !item.IsAmmo);
        }
    }
}
