using System.Collections;
using UnityEngine;
using SD.Player;
using System.Collections.Generic;

namespace SD.Weapons
{
    abstract class Weapon : MonoBehaviour
    {
        #region fields
        #region readonly
        protected int WeaponLayerMask; // Layers to be tested

        private string AnimShootName;
        private string AnimJamName;
        private string AnimUnjamName;
        private string AnimBreakName;

        private const float RecoilDivisor = 10.0f;

        const float HealthToJam = 0.15f;    // if below this number then weapon can jam
        const float JamProbability = 0.02f; // probability of jamming
        #endregion

        private WeaponState state;

        // Items in player's inventory
        private WeaponItem item;
        private AmmoHolder ammo;

        private WeaponsEnum weaponIndex;
        private string weaponName;
        private float damage;
        private AmmoType ammoType;
        private float reloadingTime;
        private float accuracy;
        private float health;
        private float shotDmg;      // damage to the weapon, for 1 shot

        public float TakingOutTime = 0.2f;
        public float HidingTime = 0.2f;

        [SerializeField]
        protected int AmmoConsumption = 1;

        private Animation weaponAnimation;

        [SerializeField]
        protected AudioClip ShotSound;
        [SerializeField]
        protected AudioClip UnjamSound;
        [SerializeField]
        protected AudioClip BreakSound;
        #endregion

        #region properties
        public WeaponsEnum WeaponIndex => weaponIndex;
        public string Name => weaponName;
        public float DamageValue => damage;
        public AmmoType AmmoType => ammoType;
        public float ReloadingTime => reloadingTime;
        /// <summary>
        /// Health in percents: [0,1]
        /// </summary>
        public float Health => health;
        /// <summary>
        /// Accuracy in percents
        /// </summary>
        public float Accuracy => accuracy;
        public bool IsBroken => health <= 0.0f;
        public WeaponState State => state;
        #endregion

        #region initialization
        void Start()
        {
            WeaponLayerMask = LayerMask.GetMask(LayerNames.Default, LayerNames.Damageable);
            state = WeaponState.Nothing;
        }

        /// <summary>
        /// Init fields from player's items
        /// </summary>
        public void Init(WeaponItem playerItem, AmmoHolder playerAmmo)
        {
            ammo = playerAmmo;
            item = playerItem;

            // load info
            weaponName = item.Stats.Name;
            weaponIndex = item.This;

            damage = item.Stats.Damage;
            ammoType = item.Stats.AmmoType;
            reloadingTime = item.Stats.ReloadingTime;
            accuracy = item.Stats.Accuracy;

            shotDmg = 1.0f / (float)item.Stats.Durability;
            health = item.Health;

            // get anim
            weaponAnimation = GetComponentInChildren<Animation>(true);

            string tempName = weaponAnimation.clip.name;
            tempName = tempName.Remove(0, 1);

            AnimShootName = "S" + tempName;
            AnimJamName = "J" + tempName;
            AnimUnjamName = "U" + tempName;
            AnimBreakName = "B" + tempName;

            // set state
            state = WeaponState.Nothing;
        }
        #endregion

        #region overridable
        protected abstract void PrimaryAttack();

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
            WeaponsController.Instance.PlaySound(clip);
        }

        protected void PlayPrimaryAnimation()
        {
            if (weaponAnimation.isPlaying)
            {
                foreach (AnimationState state in weaponAnimation)
                {
                    state.time = 0;
                }
            }

            if (State == WeaponState.Jamming)
            {
                PlayJammingAnimation();
                return;
            }

            weaponAnimation.Play();
        }

        void PlayJammingAnimation()
        {
            weaponAnimation.Play(AnimJamName);
        }

        void PlayUnjammingAnimation()
        {
            weaponAnimation.Play(AnimUnjamName);
        }

        void PlayBreakingAnimation()
        {
            weaponAnimation.Play(AnimBreakName);
        }

        protected void RecoilJump()
        {
            RecoilJump(damage, reloadingTime);
        }

        protected void RecoilJump(float force, float time)
        {
            CameraShaker.Instance.Shake(force / RecoilDivisor, time);
        }

        // Should this weapon be jammed?
        bool ToJam()
        {
            // throwables and cannon never jam
            if (AmmoType == AmmoType.Cannonballs || AmmoType == AmmoType.FireBottles || AmmoType == AmmoType.Grenades)
            {
                return false;
            }

            return health > HealthToJam ? false : Random.Range(0.0f, 1.0f) < JamProbability;
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
            StartCoroutine(WaitForDisabling(HidingTime));
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

        /// <summary>
        /// Force weapon to disable.
        /// When weapon is disabled, its state is Nothing.
        /// </summary>
        public void ForceDisable()
        {
            switch (state)
            {
                case WeaponState.Nothing:
                    return;
                case WeaponState.Breaking:
                    return;
                case WeaponState.Disabling:
                    return;
                case WeaponState.Ready:
                    Disable();
                    return;
                default:
                    StartCoroutine(WaitForReady());
                    return;
            }
        }

        /// <summary>
        /// Wait for Ready state and then disable
        /// </summary>
        IEnumerator WaitForReady()
        {
            while (state != WeaponState.Ready)
            {
                yield return null;
            }

            Disable();
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
            StartCoroutine(Wait(TakingOutTime, WeaponState.Ready));
        }

        public void Fire()
        {
            if (ammo[ammoType] < AmmoConsumption)
            {
                return;
            }

            if (state != WeaponState.Ready)
            {
                // ignore if not ready to shoot
                // Debug.LogWarning("Wrong weapon state");
                return;
            }

            state = WeaponState.Reloading;

            // throwables never breaks
            if (AmmoType == AmmoType.FireBottles || AmmoType == AmmoType.Grenades)
            {
                // process shooting damage to the weapon
                health -= shotDmg;
            }

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
            ReduceAmmo();

            // wait for reload
            StartCoroutine(Wait(reloadingTime, WeaponState.Ready));
        }

        void Jam()
        {
            state = WeaponState.Jamming;

            // jamming animation will be played
            // everything else is same as primary
            PrimaryAttack();
        }

        public void Unjam()
        {
            if (state != WeaponState.Jamming)
            {
                return;
            }

            state = WeaponState.Unjamming;

            // play animation (shaking)
            PlayUnjammingAnimation();
            PlayAudio(UnjamSound);

            // camera
            RecoilJump(10, 0.75f);

            // additional effects
            UnjamAdditional();

            // wait and reset state
            StartCoroutine(Wait(reloadingTime, WeaponState.Ready));
        }

        protected virtual void UnjamAdditional() { }

        void Break()
        {
            // play anim and sound
            PlayBreakingAnimation();
            PlayAudio(BreakSound);

            StartCoroutine(WaitForBreak());
        }

        IEnumerator WaitForBreak()
        {
            // wait for reloading time without disbling
            yield return new WaitForSeconds(reloadingTime - HidingTime);

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

        protected Transform FindChildByName(string name)
        {
            Stack<Transform> ts = new Stack<Transform>();
            ts.Push(transform);

            while (ts.Count > 0)
            {
                Transform t = ts.Pop();

                if (t.name == name)
                {
                    return t;
                }

                foreach (Transform c in t)
                {
                    ts.Push(c);
                }
            }

            return null;
        }
    }
}