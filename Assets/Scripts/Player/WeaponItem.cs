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
        float health;   // here, as it's changing over time,
                        // WeaponStats holds stats that are constant for each weapon

        WeaponsEnum weapon;  // current weapon
    }
}
