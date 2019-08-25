using System;
using System.Collections.Generic;
using UnityEngine;
using SD.Weapons;

namespace SD.PlayerLogic
{
    /// <summary>
    /// Player's inventory.
    /// Holds all information about player's items.
    /// This class is singleton.
    /// </summary>
    class PlayerInventory : IInventory
    {
        public WeaponsHolder Weapons { get; private set; }
        public AmmoHolder Ammo { get; private set; }
        public ItemsHolder Items { get; private set; }

        public event PlayerBalanceChange OnBalanceChange;

        int money;
        public int Money
        {
            get
            {
                return money;
            }
            set
            {
                int oldBalance = money;
                money = value;

                try
                {
                    OnBalanceChange(oldBalance, money);
                }
                catch { }
            }
        }

        IWeaponsHolder IInventory.Weapons => Weapons;
        IAmmoHolder IInventory.Ammo => Ammo;

        /// <summary>
        /// Sets default values
        /// </summary>
        public PlayerInventory()
        {
            Weapons = new WeaponsHolder();
            Ammo = new AmmoHolder();
            Items = new ItemsHolder();
        }

        public void Init()
        {
            Weapons.Init();
            Ammo.Init();
            Items.Init();
            Money = 0;
        }

        public void SetDefault()
        {
            Weapons.SetDefault();
            Ammo.SetDefault();
            Items.SetDefault();
            Money = 1000;
        }

        #region cheats
        /// <summary>
        /// Give all to player
        /// </summary>
        public void GiveAll()
        {
            // only in editor
#if !UNITY_EDITOR
            return;
#endif

            GiveAllWeapons();
            GiveAllAmmo();
        }

        /// <summary>
        /// Give all weapons to player
        /// </summary>
        public void GiveAllWeapons()
        {
            foreach (WeaponIndex w in Enum.GetValues(typeof(WeaponIndex)))
            {
                Weapons.Set(w, GameController.Instance.WeaponsStats[w].Durability, true, true);
            }
        }

        /// <summary>
        /// Give all ammo to player
        /// </summary>
        public void GiveAllAmmo()
        {
            foreach (AmmunitionType a in Enum.GetValues(typeof(AmmunitionType)))
            {
                Ammo.Set(a, GameController.Instance.AmmoStats[a].MaxAmount);
            }
        }
        #endregion
    }
}
