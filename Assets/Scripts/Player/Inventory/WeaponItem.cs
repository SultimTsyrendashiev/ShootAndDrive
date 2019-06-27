using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.Player
{
    // Represents weapon item in player's inventory
    class WeaponItem
    {
        private WeaponIndex weapon; // this weapon

        /// <summary>
        /// Health in percents [0,1]
        /// </summary>
        public float        Health;
        public bool         IsBought;

        public WeaponIndex  This { get { return weapon; } }
        public bool         IsBroken { get { return Health <= 0.0f; } }
        public WeaponStats  Stats { get { return AllWeaponsStats.Instance.Get(weapon); } }

        public WeaponItem(WeaponIndex weapon, float health, bool isBought)
        {
            this.weapon = weapon;
            this.Health = health;
            this.IsBought = isBought;
        }
    }
}
