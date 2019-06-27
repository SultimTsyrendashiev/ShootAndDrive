﻿using UnityEngine;
using SD.Weapons;

namespace SD.Player
{
    // Use this instead of WeaponStats?
    class WeaponData : ScriptableObject
    {
        [SerializeField] string          weaponName;
        [SerializeField] AmmoType        ammoType;      // what ammo type this weapon uses

        [SerializeField] int             cost;          // cost in a shop
        [SerializeField] int             durability;    // how many shots is needed to destroy weapon

        [SerializeField] float           damage;        // in health points
        [SerializeField] float           reloadingTime; // in seconds
        [SerializeField] float           accuracy;      // accuracy in percents
    }
}