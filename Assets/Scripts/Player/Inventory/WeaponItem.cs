using UnityEngine;
using SD.Weapons;

namespace SD.PlayerLogic
{
    // Represents weapon item in player's inventory
    class WeaponItem
    {
        AllWeaponsStats allStats;

        public WeaponIndex  This { get; }
        public bool         IsBought;
        public bool         IsBroken => HealthRef.Value <= 0;

        public WeaponData   Stats => allStats[This];

        public WeaponItem(WeaponIndex weapon, int health, bool isBought)
        {
            this.This = weapon;
            this.HealthRef = new RefInt(health);
            this.IsBought = isBought;

            var gameController = Object.FindObjectOfType<GameController>();
            allStats = gameController.WeaponsStats;

        }

        /// <summary>
        /// Health in percents [0,1]
        /// </summary>
        public RefInt HealthRef { get; }
    }
}
