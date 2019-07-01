using System.Collections;
using UnityEngine;
using SD.Player;
using System.Collections.Generic;

namespace SD.Weapons
{
    delegate void WeaponBreak(WeaponIndex brokenWeapon); 

    abstract class Weapon : MonoBehaviour
    {
        #region fields
        #region readonly
        protected int WeaponLayerMask; // Mask to be tested
        protected int DamageableLayer; // Layer with damageable objects

        private string AnimShootName;
        private string AnimJamName;
        private string AnimUnjamName;
        private string AnimBreakName;

        const float RecoilJumpMultiplier = 0.25f;

        const float HealthToJam = 0.15f;    // if below this number then weapon can jam
        #endregion

        private WeaponState state;

        /// <summary>
        /// Common event, called on waepon breaking
        /// </summary>
        public static event WeaponBreak OnWeaponBreak;

        // Items in player's inventory
        private WeaponItem item;
        private AmmoHolder ammo;

        private WeaponIndex weaponIndex;
        private string weaponName;
        private float damage;
        private AmmoType ammoType;
        private float reloadingTime;
        private float accuracy;
        private int durability;

        // weapon's health must be synced with inventory
        // so use reference to that int
        private RefInt refHealth;

        public float TakingOutTime = 0.2f;
        public float HidingTime = 0.2f;

        [SerializeField]
        protected int AmmoConsumption = 1;
        [SerializeField]
        public float JamProbability = 0.03f; // probability of jamming

        private Animation weaponAnimation;
        private HandPivot handPivot;    // script for attaching hand

        [SerializeField]
        protected AudioClip ShotSound;
        [SerializeField]
        protected AudioClip UnjamSound;
        [SerializeField]
        protected AudioClip BreakSound;
        #endregion

        #region properties
        public WeaponIndex WeaponIndex => weaponIndex;
        public string Name => weaponName;
        public float DamageValue => damage;
        public AmmoType AmmoType => ammoType;
        public float ReloadingTime => reloadingTime;
        /// <summary>
        /// Health in percents: [0,1]
        /// </summary>
        public float Health => (float)refHealth.Value / durability;
        /// <summary>
        /// Accuracy in percents
        /// </summary>
        public float Accuracy => accuracy;
        public bool IsBroken => refHealth.Value <= 0;
        public WeaponState State => state;
        #endregion

        #region initialization
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

            durability = item.Stats.Durability;
            refHealth = item.GetHealthRef();

            // get anim
            weaponAnimation = GetComponentInChildren<Animation>(true);

            string tempName = weaponAnimation.clip.name;
            tempName = tempName.Remove(0, 1);

            AnimShootName = "S" + tempName;
            AnimJamName = "J" + tempName;
            AnimUnjamName = "U" + tempName;
            AnimBreakName = "B" + tempName;

            // layers
            WeaponLayerMask = ~((1 << LayerMask.GetMask(LayerNames.Default)) | (1 << LayerMask.GetMask(LayerNames.Damageable)));
            DamageableLayer = LayerMask.NameToLayer(LayerNames.Damageable);

            // set hand animation
            handPivot = GetComponentInChildren<HandPivot>(true);
            if (handPivot != null)
            {
                string handAnimName = "H" + tempName;
                handPivot.Init(handAnimName);
            }

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
            if (state != WeaponState.Jamming)
            {
                PlayShootingAnimation();
            }
            else
            {
                PlayJammingAnimation();
            }
        }

        void PlayShootingAnimation()
        {
            // reset to start
            if (weaponAnimation.isPlaying)
            {
                foreach (AnimationState state in weaponAnimation)
                {
                    state.time = 0;
                }
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
            if (force == 0)
            {
                return;
            }

            float sign = Mathf.Sign(force);

            CameraShaker.Instance.Shake(sign * RecoilJumpMultiplier * Mathf.Log(Mathf.Abs(force)), time);
        }

        /// <summary>
        /// Should this weapon jam?
        /// </summary>
        bool ToJam()
        {
            // throwables and cannon never jam
            if (!AllWeaponsStats.Instance.CanJam(AmmoType))
            {
                return false;
            }

            return refHealth.Value > HealthToJam ? false : Random.Range(0.0f, 1.0f) < JamProbability;
        }

        /// <summary>
        /// Shortcut for 'AllWeaponsStats.Instance.CanBreak'
        /// </summary>
        bool CanBreak()
        {
            return AllWeaponsStats.Instance.CanBreak(AmmoType);
        }

        /// <summary>
        /// Should this weapon break?
        /// </summary>
        bool ToBreak()
        {
            if (!CanBreak())
            {
                return false;
            }

            return refHealth.Value <= 0;
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

            gameObject.SetActive(false);
            Deactivate();
        }

        /// <summary>
        /// Force weapon to disable.
        /// Note: when weapon is disabled, its state is Nothing.
        /// </summary>
        public bool ForceDisable()
        {
            switch (state)
            {
                case WeaponState.Nothing:
                    return true;
                case WeaponState.Breaking:
                    return false;
                case WeaponState.Disabling:
                    return false;
                case WeaponState.Ready:
                    Disable();
                    return false;
                default:
                    StartCoroutine(WaitForReady());
                    return false;
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

            gameObject.SetActive(true);
            Activate();

            // pose hand
            if (handPivot != null)
            {
                handPivot.PoseHand();
            }
            else
            {
                // if doesn't exist, dont render it
                HandsController.Instance.RenderRightHand = false;
            }

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

            // process shooting damage to the weapon
            if (CanBreak())
            {
                refHealth.Value--;
            }
            
            // break if not enough health
            if (ToBreak())
            {
                Break();
                return;
            }

            WeaponState nextState = WeaponState.Ready;

            // check for jamming
            if (ToJam())
            {
                Jam();
                nextState = WeaponState.ReadyForUnjam;
            }

            // - shoot
            // - if weapon must jam, needed animation will be played
            //   everything else is same as primary

            PrimaryAttack();
            ReduceAmmo();

            // wait for reload
            StartCoroutine(Wait(reloadingTime, nextState));
        }

        void Jam()
        {
            state = WeaponState.Jamming;
        }

        public void Unjam()
        {
            if (state != WeaponState.ReadyForUnjam)
            {
                return;
            }

            state = WeaponState.Unjamming;

            // play animation (shaking)
            PlayUnjammingAnimation();
            PlayAudio(UnjamSound);

            // camera
            RecoilJump(-20, reloadingTime);

            // additional effects
            UnjamAdditional();

            // wait and reset state
            StartCoroutine(Wait(reloadingTime, WeaponState.Ready));
        }

        protected virtual void UnjamAdditional() { }

        void Break()
        {
            state = WeaponState.Breaking;

            // play anim and sound
            PlayBreakingAnimation();
            PlayAudio(BreakSound);

            StartCoroutine(WaitForBreak());
        }

        IEnumerator WaitForBreak()
        {
            // wait for breaking time without disabling
            yield return new WaitForSeconds(weaponAnimation[AnimBreakName].length);

            // call event
            OnWeaponBreak(weaponIndex);

            // then disable (there will be waited for disabling)
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