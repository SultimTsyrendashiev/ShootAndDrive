using System.Collections;
using UnityEngine;
using SD.Player;

namespace SD.Weapons
{
    abstract class Weapon : MonoBehaviour
    {
        #region fields
        // Layers to be tested, assume that this is readonly
        protected int WeaponLayerMask;
        private const float RecoilDivisor = 30.0f;

        private WeaponState state;

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
        protected int AmmoConsumption;

        [SerializeField]
        private AudioSource audioSource;
        private Animation weaponAnimation;

        [SerializeField]
        protected AudioClip ShotSound;
        [SerializeField]
        protected AudioClip UnjamSound;
        [SerializeField]
        protected AudioClip BreakSound;
        #endregion

        public string Name { get { return weaponName; } }
        public float DamageValue { get { return damage; } }
        public AmmoType AmmoType { get { return ammoType; } }
        public float ReloadingTime { get { return reloadingTime; } }
        /// <summary>
        /// Health in percents: [0,1]
        /// </summary>
        public float Health { get { return health; } }
        /// <summary>
        /// Accuracy in percents
        /// </summary>
        public float Accuracy { get { return accuracy; } }
        public bool IsBroken { get { return health <= 0.0f; } }
        public WeaponState State { get { return state; } }

        #region initialization
        void Start()
        {
            WeaponLayerMask = LayerMask.GetMask(LayerNames.Default, LayerNames.Damageable);
        }

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
        #endregion

        #region overridable
        public abstract void PrimaryAttack();

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
        #endregion

        #region weapons
        protected void PlayAudio(AudioClip clip)
        {
            audioSource.PlayOneShot(clip);
        }

        protected void PlayPrimaryAnimation()
        {
            weaponAnimation.Play();
        }

        protected void PlayJammingAnimation()
        {
            weaponAnimation.Play();
        }

        protected void PlayUnjammingAnimation()
        {
            weaponAnimation.Play();
        }

        protected void PlayBreakingAnimation()
        {
            weaponAnimation.Play();
        }

        protected void RecoilJump()
        {
            RecoilJump(damage, reloadingTime);
        }

        protected void RecoilJump(float force, float time)
        {
            CameraShaker.Instance.Shake(force / RecoilDivisor, reloadingTime);
        }

        // Should this weapon be jammed?
        bool ToJam()
        {
            // throwables and cannon never jam
            if (AmmoType == AmmoType.CannonBalls || AmmoType == AmmoType.FireBottles || AmmoType == AmmoType.Grenades)
            {
                return false;
            }

            const float healthToJam = 0.15f; // if below this number then weapon can jam
            const float probability = 0.25f; // probability of jamming

            return health > healthToJam ? false : Random.Range(0.0f, 1.0f) < probability;
        }
        #endregion

        #region states
        void Disable()
        {
            if (state != WeaponState.Breaking && state != WeaponState.Ready)
            {
                Debug.LogWarning("Wrong weapon state");
                return;
            }

            state = WeaponState.Disabling;

            // wait for disabling
            StartCoroutine(WaitForDisabling(0.5f));
        }

        IEnumerator WaitForDisabling(float time)
        {
            yield return new WaitForSeconds(time);
            state = WeaponState.Nothing;

            // sync with player's inventory
            item.Health = health;

            gameObject.SetActive(false);
            Deactivate();
        }

        public void Enable()
        {
            if (state != WeaponState.Nothing)
            {
                Debug.LogWarning("Wrong weapon state");
                return;
            }

            state = WeaponState.Enabling;

            // sync with player's inventory
            health = item.Health;

            gameObject.SetActive(true);
            Activate();

            // wait for enabling
            StartCoroutine(Wait(0.5f, WeaponState.Ready));
        }

        public void Fire()
        {
            if (state == WeaponState.Jamming)
            {
                // if weapon is jamming, unjam it
                Unjam();
            }
            else if (state != WeaponState.Ready)
            {
                // ignore if not ready to shoot
                // Debug.LogWarning("Wrong weapon state");
                return;
            }

            state = WeaponState.Reloading;

            // process damage of shooting to the weapon
            health -= shotDmg;

            // break if not enough health
            if (health < 0.0f)
            {
                Break();
                return;
            }

            if (ToJam())
            {
                Jam();
                return;
            }

            // if not jammed, shoot
            PrimaryAttack();

            // wait for reload
            StartCoroutine(Wait(reloadingTime, WeaponState.Ready));
        }

        void Jam()
        {
            state = WeaponState.Jamming;

            // * play jamming animation (same as fire but stops in the middle)
            // * don't emit casings (if exist)
            // everything else is same as primary
            // jamming state must be processed there
            PrimaryAttack();
        }

        void Unjam()
        {
            // play animation (shaking)
            PlayUnjammingAnimation();
            PlayAudio(UnjamSound);

            // camera
            RecoilJump(10, 0.75f);

            // additional effects
            UnjamAdditional();

            // wait and reset state
            StartCoroutine(Wait(0.75f, WeaponState.Ready));
        }

        protected virtual void UnjamAdditional() { }

        void Break()
        {
            // play anim and sound
            PlayBreakingAnimation();
            PlayAudio(BreakSound);

            // then disable
            Disable();
        }

        /// <summary>
        /// Wait some time, and then set a new state
        /// </summary>
        IEnumerator Wait(float time, WeaponState newState)
        {
            yield return new WaitForSeconds(time);
            state = newState;
        }
        #endregion
    }
}