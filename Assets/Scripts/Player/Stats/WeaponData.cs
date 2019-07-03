using UnityEngine;
using SD.Weapons;

namespace SD.PlayerLogic
{
    // TODO: Use this instead of WeaponStats
    [CreateAssetMenu(menuName = "Weapon Data", order = 51)]
    class WeaponData : ScriptableObject
    {
        [SerializeField] string         weaponName;
        [SerializeField] Sprite         pistolIcon;

        [SerializeField] AmmunitionType       ammoType;      // what ammo type this weapon uses

        [SerializeField] int            cost;          // cost in a shop
        [SerializeField] int            durability;    // how many shots is needed to destroy weapon

        [SerializeField] float          damage;        // in health points
        [SerializeField] float          reloadingTime; // in seconds
        [SerializeField] float          accuracy;      // accuracy in percents
    }
}