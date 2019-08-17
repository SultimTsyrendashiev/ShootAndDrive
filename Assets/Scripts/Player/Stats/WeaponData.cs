using UnityEngine;

namespace SD.Weapons
{
    [CreateAssetMenu(menuName = "Weapon Data", order = 51)]
    class WeaponData : ScriptableObject
    {
        // common
        [SerializeField] WeaponIndex        weaponIndex;
        [SerializeField] string             weaponEditorName;
        [SerializeField] string             translationKey;
        [SerializeField] Sprite             icon;

        [SerializeField] AudioClip          shotSound;
        [SerializeField] AudioClip          unjamSound;
        [SerializeField] AudioClip          breakSound;


        [SerializeField] AmmunitionType     ammoType;           // what ammo type this weapon uses
        [SerializeField] int                ammoConsumption;

        [SerializeField] int                cost;               // cost in a shop
        [SerializeField] int                durability;         // how many shots is needed to destroy weapon

        [SerializeField] bool               canBeDamaged;       // should health of this weapon reduce?
        [SerializeField] bool               isAmmo;             // is this weapon is ammo too? F.e grenades, fire bottles

        [SerializeField] float              percentageForJam = 0.17f;   // if (health/durabiltity) below this number, 
                                                                        // weapon can jam
        [SerializeField] float              jamProbability = 0.03f;

        [SerializeField] float              damage;             // in health points
        [SerializeField] float              reloadingTime;      // in seconds

        // for hitscan
        [SerializeField] float              accuracy;           // accuracy in [0..1]

        // for missiles
        [SerializeField] string             missileName;
        
        void Awake()
        {
            translationKey = "Inventory.Weapons.Names." + Index.ToString();
        }

        public WeaponIndex      Index => weaponIndex;
        public string           EditorName => weaponEditorName;
        public string           TranslationKey => translationKey;
        public Sprite           Icon => icon;
        public AmmunitionType   AmmoType => ammoType;
        public int              AmmoConsumption => ammoConsumption;
        public int              Cost => cost;
        public int              Durability => durability;
        public float            PercentageForJam => percentageForJam;
        /// <summary>
        /// Probability of jamming
        /// </summary>
        public float            JamProbability => jamProbability; 
        public float            Damage => damage;
        public float            ReloadingTime => reloadingTime;
        /// <summary>
        /// Accuracy in [0..1].
        /// '1' means perfect accuracy.
        /// </summary>
        public float            Accuracy => accuracy;
        public string           MissileName => missileName;

        public AudioClip        ShotSound => shotSound;
        public AudioClip        UnjamSound => unjamSound; 
        public AudioClip        BreakSound => breakSound;

        /// <summary>
        /// Get fire rate in rounds per minute
        /// </summary>
        public int RoundsPerMinute => (int)(60.0f / reloadingTime);

        public bool IsAmmo => isAmmo;
        public bool CanBeDamaged => canBeDamaged;
    }
}