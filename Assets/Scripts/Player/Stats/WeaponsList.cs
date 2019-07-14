using UnityEngine;

namespace SD.Weapons
{
    [CreateAssetMenu(menuName = "Weapons List", order = 51)]
    class WeaponsList : ScriptableObject
    {
        [SerializeField]
        WeaponData[] weaponsData;

        public WeaponData[] Data => weaponsData;
    }
}
