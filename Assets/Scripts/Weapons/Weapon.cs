using UnityEngine;
using SD.Player;

namespace SD.Weapons
{
    abstract class Weapon : MonoBehaviour
    {
        #region fields
        // Layers to be tested
        protected readonly int WeaponLayerMask = LayerMask.GetMask(LayerNames.Default, LayerNames.Damageable);
        private const float RecoilDivisor = 50.0f;

        // Items in player's inventory
        private WeaponItem item;
        private AmmoHolder ammo;

        private string weaponName;
        private float damage;
        private AmmoType ammoType;
        private float reloadingTime;
        private float accuracy;
        private float health;
        private float shotDmg;      // damage to the weapon, for 1 shot

        [SerializeField]
        private AudioSource audioSource;
        private Animation weaponAnimation;

        protected int AmmoConsumption;
        #endregion

        public string Name { get { return weaponName; } }
        public float DamageValue { get { return damage; } }
        public AmmoType AmmoType { get { return ammoType; } }
        public float ReloadingTime { get { return reloadingTime; } }
        public float Accuracy { get { return accuracy; } }
        public bool IsBroken { get { return health <= 0.0f; } }

        public abstract void PrimaryAttack();

        /// <summary>
        /// Init fields from player's items
        /// </summary>
        public void Init(WeaponItem playerItem, AmmoHolder playerAmmo)
        {
            ammo = playerAmmo;
            item = playerItem;

            weaponName = item.Stats.Name;
            damage = item.Stats.Damage;
            ammoType = item.Stats.AmmoType;
            reloadingTime = item.Stats.ReloadingTime;
            accuracy = item.Stats.Accuracy;

            shotDmg = 1.0f / (float)item.Stats.Durability;
            health = item.Health;

            weaponAnimation = GetComponentInChildren<Animation>();
        }

        void OnDisable()
        {
            // sync with player's inventory
            item.Health = health;
            Deactivate();
        }

        void OnEnable()
        {
            // sync with player's inventory
            health = item.Health;
            Activate();
        }

        /// <summary>
        /// Called on weapon disable
        /// </summary>
        protected virtual void Deactivate() { }

        /// <summary>
        /// Called on weapon enable
        /// </summary>
        protected virtual void Activate() { }

        protected void ReduceAmmo()
        {
            ammo.Add(ammoType, -AmmoConsumption);
        }

        protected void PlayAudio(AudioClip clip)
        {
            audioSource.PlayOneShot(clip);
        }

        protected void PlayPrimaryAnimation()
        {
            weaponAnimation.Play();
        }

        protected void RecoilJump()
        {
            CameraShaker.Instance.Shake(damage / RecoilDivisor, reloadingTime);
        }
    }
}