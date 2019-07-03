using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.PlayerLogic
{
    // Represents weapon item in player's inventory
    class WeaponItem
    {
        private WeaponIndex weapon; // this weapon

        /// <summary>
        /// Health in percents [0,1]
        /// </summary>
        private RefInt       refHealth;

        public bool         IsBought;

        public WeaponIndex  This => weapon;
        public bool         IsBroken => refHealth.Value <= 0.0f;
        public WeaponStats  Stats => AllWeaponsStats.Instance.Get(weapon);

        public WeaponItem(WeaponIndex weapon, int health, bool isBought)
        {
            this.weapon = weapon;
            this.refHealth = new RefInt(health);
            this.IsBought = isBought;
        }

        public RefInt GetHealthRef()
        {
            return refHealth;
        }
    }
}
