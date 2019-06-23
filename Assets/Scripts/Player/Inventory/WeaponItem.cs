using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.Player
{
    // Represents weapon in player's inventory
    class WeaponItem
    {
        WeaponsEnum weapon; // current weapon

        // here, as it's changing over time,
        // WeaponStats holds stats that are constant for each weapon
        public float Health { get; set; }
        public WeaponStats Stats { get { return AllWeaponsStats.Instance.Get(weapon); } }
        public bool IsBroken { get { return Health <= 0.0f; } }
    }
}
