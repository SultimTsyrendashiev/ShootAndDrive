using UnityEngine;

namespace SD.Weapons
{
    [CreateAssetMenu(menuName = "Ammo List", order = 51)]
    class AmmoList : ScriptableObject
    {
        [SerializeField]
        AmmoData[] ammoData;

        public AmmoData[] Data => ammoData;
    }
}
