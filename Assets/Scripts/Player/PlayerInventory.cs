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
        public WeaponsHolder    Weapons { get; private set; }
        public AmmoHolder       Ammo { get; private set; }
        public ItemsHolder      Items { get; private set; }

        PlayerStats playerStats;
        public PlayerStats      PlayerStats { get => playerStats; set => playerStats = value; }


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

                OnBalanceChange?.Invoke(oldBalance, money);
            }
        }

        public event PlayerBalanceChange OnBalanceChange;

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

            SignToStatsEvents();
        }

        public void Init()
        {
            Weapons.Init();
            Ammo.Init();
            Items.Init();
            money = 0;
        }

        public void InitMoney(int amount)
        {
            money = amount;
        }

        public void SetDefault()
        {
            Weapons.SetDefault();
            Ammo.SetDefault();
            Items.SetDefault();
            money = 2500;

            Weapons.Set(WeaponIndex.PistolRevolver, 4, true, true);
            Ammo.Set(AmmunitionType.BulletsPistol, 4);

#if UNITY_EDITOR
            //GiveAll();
#endif
        }

#region player's stats handlers
        ~PlayerInventory()
        {
            // unsign
            Weapon.OnShot -= IncreaseShotAmount;
            GameController.OnScoreSet -= SetScore;

            GameController.Instance.Shop.OnAmmoBuy -= ShopAmmoBuy;
            GameController.Instance.Shop.OnWeaponBuy -= ShopWeaponBuy;
            GameController.Instance.Shop.OnWeaponRepair -= ShopWeaponRepair;

            OnBalanceChange -= IncreaseSpentCash;
        }

        /// <summary>
        /// Sign to events to change player's stats
        /// </summary>
        void SignToStatsEvents()
        {
            Weapon.OnShot += IncreaseShotAmount;
            GameController.OnScoreSet += SetScore;

            GameController.Instance.Shop.OnAmmoBuy += ShopAmmoBuy;
            GameController.Instance.Shop.OnWeaponBuy += ShopWeaponBuy;
            GameController.Instance.Shop.OnWeaponRepair += ShopWeaponRepair;

            OnBalanceChange += IncreaseSpentCash;
        }

        void IncreaseSpentCash(int oldBalance, int newBalance)
        {
            int diff = newBalance - oldBalance;

            // if was decreased
            if (diff < 0)
            {
                playerStats.Cash_TotalSpent += -diff;
            }
        }

        void ShopWeaponRepair(WeaponIndex index, int price)
        {
            playerStats.Cash_SpentOnRepairs += price;
            playerStats.Cash_SpentOnWeapons += price;
        }

        void ShopWeaponBuy(WeaponIndex index, int price)
        {
            playerStats.Cash_SpentOnWeaponBuys += price;
            playerStats.Cash_SpentOnWeapons += price;
        }

        void ShopAmmoBuy(AmmunitionType index, int price)
        {
            playerStats.Cash_SpentOnAmmo += price;
        }

        void SetScore(GameScore score, int prevBestScore)
        {
            if (playerStats.Score_Best < score.ActualScorePoints)
            {
                playerStats.Score_Best = score.ActualScorePoints;
            }

            playerStats.Player_TotalDeaths++;

            playerStats.Cash_TotalEarned += score.Money;

            playerStats.Combat_TotalKills += score.KillsAmount;
            playerStats.Combat_TotalVehiclesDestroyed += score.DestroyedVehiclesAmount;

            playerStats.Score_TotalEarned += score.ActualScorePoints;

            playerStats.Vehicle_OverallTravelledDistance += score.TravelledDistance;
        }

        void IncreaseShotAmount()
        {
            playerStats.Combat_TotalShots++;
        }
#endregion

#region cheats
        /// <summary>
        /// Give all to player
        /// </summary>
        public void GiveAll()
        {
            // only in editor
#if UNITY_EDITOR
            GiveAllWeapons();
            GiveAllAmmo();
#endif
        }

        /// <summary>
        /// Give all weapons to player
        /// </summary>
        public void GiveAllWeapons()
        {
#if UNITY_EDITOR
            foreach (WeaponIndex w in Enum.GetValues(typeof(WeaponIndex)))
            {
                Weapons.Set(w, GameController.Instance.WeaponsStats[w].Durability, true, true);
            }
#endif
        }

        /// <summary>
        /// Give all ammo to player
        /// </summary>
        public void GiveAllAmmo()
        {
#if UNITY_EDITOR
            foreach (AmmunitionType a in Enum.GetValues(typeof(AmmunitionType)))
            {
                Ammo.Set(a, GameController.Instance.AmmoStats[a].MaxAmount);
            }
#endif
        }
#endregion
    }
}
