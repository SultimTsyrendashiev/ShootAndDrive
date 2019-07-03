using System.Collections;
using UnityEngine;
using SD.PlayerLogic;
using System.Collections.Generic;

namespace SD.Weapons
{
    delegate void WeaponBreak(WeaponIndex brokenWeapon); 

    abstract class Weapon : MonoBehaviour
    {
        #region fields
        #region readonly
        protected int AutoaimLayerMask; // Mask to find autoaim targets
        protected int WeaponLayerMask;  // Mask to be tested

        protected int DamageableLayer; // Layer with damageable objects

        private string AnimShootName;
        private string AnimJamName;
        private string AnimUnjamName;
        private string AnimBreakName;

        const float RecoilJumpMultiplier = 0.25f;

        const float HealthToJam = 0.15f;    // if below this number then weapon can jam
        #endregion


        /// <summary>
        /// Common event, called on waepon breaking
        /// </summary>
        public static event WeaponBreak OnWeaponBreak;

        // Items in player's inventory
        private WeaponItem item;
        private AmmoHolder ammo;
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
        public WeaponIndex WeaponIndex { get; private set; }
        public string Name { get; private set; }
        public float DamageValue { get; private set; }
        public AmmunitionType AmmoType { get; private set; }
        public float ReloadingTime { get; private set; }
        /// <summary>
        /// Health in percents: [0,1]
        /// </summary>
        public float Health => (float)refHealth.Value / durability;
        /// <summary>
        /// Accuracy in percents
        /// </summary>
        public float Accuracy { get; private set; }
        public bool IsBroken => refHealth.Value <= 0;
        public WeaponState State { get; private set; }
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
            Name = item.Stats.Name;
            WeaponIndex = item.This;

            DamageValue = item.Stats.Damage;
            AmmoType = item.Stats.AmmoType;
            ReloadingTime = item.Stats.ReloadingTime;
            Accuracy = item.Stats.Accuracy;

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
            AutoaimLayerMask = LayerMask.GetMask(LayerNames.AutoaimTargets);
            WeaponLayerMask = LayerMask.GetMask(LayerNames.Default, LayerNames.Damageable);
            DamageableLayer = LayerMask.NameToLayer(LayerNames.Damageable);

            // set hand animation
            handPivot = GetComponentInChildren<HandPivot>(true);
            if (handPivot != null)
            {
                string handAnimName = "H" + tempName;
                handPivot.Init(handAnimName);
            }

            // set state
            State = WeaponState.Nothing;
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
            ammo.Add(AmmoType, -AmmoConsumption);
        }
        #endregion

        #region weapons
        protected void PlayAudio(AudioClip clip)
        {
            WeaponsController.Instance.PlaySound(clip);
        }

        protected void PlayPrimaryAnimation()
        {
            if (State != WeaponState.Jamming)
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
            RecoilJump(DamageValue, ReloadingTime);
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
            if (State != WeaponState.Breaking && State != WeaponState.Ready)
            {
                Debug.LogWarning("Wrong weapon state");
                return;
            }

            State = WeaponState.Disabling;

            // wait for disabling
            StartCoroutine(WaitForDisabling(HidingTime));
        }

        IEnumerator WaitForDisabling(float time)
        {
            yield return new WaitForSeconds(time);
            State = WeaponState.Nothing;

            gameObject.SetActive(false);
            Deactivate();
        }

        /// <summary>
        /// Force weapon to disable.
        /// Note: when weapon is disabled, its state is Nothing.
        /// </summary>
        public bool ForceDisable()
        {
            switch (State)
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
            while (State != WeaponState.Ready)
            {
                yield return null;
            }

            Disable();
        }

        public void Enable()
        {
            if (State != WeaponState.Nothing)
            {
                Debug.LogWarning("Wrong weapon state");
                return;
            }

            State = WeaponState.Enabling;

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
            if (ammo[AmmoType] < AmmoConsumption)
            {
                return;
            }

            if (State != WeaponState.Ready)
            {
                // ignore if not ready to shoot
                // Debug.LogWarning("Wrong weapon state");
                return;
            }

            State = WeaponState.Reloading;

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
            StartCoroutine(Wait(ReloadingTime, nextState));
        }

        void Jam()
        {
            State = WeaponState.Jamming;
        }

        public void Unjam()
        {
            if (State != WeaponState.ReadyForUnjam)
            {
                return;
            }

            State = WeaponState.Unjamming;

            // play animation (shaking)
            PlayUnjammingAnimation();
            PlayAudio(UnjamSound);

            // camera
            RecoilJump(-20, ReloadingTime);

            // additional effects
            UnjamAdditional();

            // wait and reset state
            StartCoroutine(Wait(ReloadingTime, WeaponState.Ready));
        }

        protected virtual void UnjamAdditional() { }

        void Break()
        {
            State = WeaponState.Breaking;

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
            OnWeaponBreak(WeaponIndex);

            // then disable (there will be waited for disabling)
            Disable();
        }

        /// <summary>
        /// Wait some time, and then set a new state
        /// </summary>
        IEnumerator Wait(float time, WeaponState newState)
        {
            yield return new WaitForSeconds(time);
            State = newState;
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