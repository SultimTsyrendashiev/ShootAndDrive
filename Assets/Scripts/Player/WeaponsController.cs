using System;
using System.Collections.Generic;
using UnityEngine;

namespace SD.Player
{
    // Player's weapons controller.
    // All weapons must be children of this object
    class WeaponsController : MonoBehaviour
    {
        WeaponsHolder weapons;

        WeaponsEnum currentWeapon;  // current player's weapon
        WeaponsEnum nextWeapon;     // weapon to switch on

        void Start()
        {
            weapons = Player.Instance.inv;
        }

        void SwitchTo(WeaponsEnum w)
        {
            if (!weapons.IsAvailable(w))
            {
                return;
            }

            nextWeapon = w;

            if ()
        }
    }
}
