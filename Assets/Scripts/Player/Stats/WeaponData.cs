using UnityEngine;

namespace SD.Weapons
{
    [CreateAssetMenu(menuName = "Weapon Data", order = 51)]
    class WeaponData : ScriptableObject
    {
        [SerializeField] WeaponIndex        weaponIndex;
        [SerializeField] string             weaponEditorName;
        [SerializeField] string             weaponName;
        [SerializeField] Sprite             icon;

        [SerializeField] AmmunitionType     ammoType;      // what ammo type this weapon uses

        [SerializeField] int                cost;          // cost in a shop
        [SerializeField] int                durability;    // how many shots is needed to destroy weapon

        [SerializeField] float              damage;        // in health points
        [SerializeField] float              reloadingTime; // in seconds
        [SerializeField] float              accuracy;      // accuracy in percents

        public WeaponIndex Index => weaponIndex;
        //public string EditorName => weaponEditorName;
        public string Name => weaponName;
        public Sprite Icon => icon;
        public AmmunitionType AmmoType => ammoType;
        public int Cost => cost;
        public int Durability => durability;
        public float Damage => damage;
        public float ReloadingTime => reloadingTime;
        public float Accuracy => accuracy;

        public float GetFireRate()
        {
            return (int)(60.0f / reloadingTime);
        }
    }
}