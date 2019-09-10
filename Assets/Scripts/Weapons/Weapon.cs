using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SD.PlayerLogic;
using Random = UnityEngine.Random;

namespace SD.Weapons
{
    abstract class Weapon : MonoBehaviour
    {
        #region fields
        #region readonly
        public const float             TakingOutTime   = 0.2f;
        public const float             HidingTime      = 0.2f;
        #endregion

        #region events
        /// <summary>
        /// Common event, called on waepon breaking
        /// </summary>
        public static event Action<WeaponIndex> OnWeaponBreak;
        
        /// <summary>
        /// Called on ammo change.
        /// Shows current weapon's ammo amount
        /// </summary>
        public static event Action<int>         OnAmmoChange;

        /// <summary>
        /// Calles when weapon finish shooting
        /// </summary>
        public static event Action<WeaponIndex> OnShootFinish;

        public static event Action<WeaponIndex> OnAmmoRunOut;

        public static event Action              OnShot;

        public static event Action<WeaponState, WeaponState> OnStateChange;
        #endregion

        // Items in player's inventory
        WeaponItem              item;
        IAmmoHolder             ammo;

        // weapon's health must be synced with inventory
        // so use reference to that int
        RefInt                  refHealth;

        #region animation fields
        Animation               weaponAnimation;
        string                  AnimShootName;
        string                  AnimJamName;
        string                  AnimUnjamName;
        string                  AnimBreakName;

        HandPivot               handPivot;    // script for attaching hand
        #endregion

        protected float         RecoilJumpMultiplier = 0.25f;

        #region layers
        protected int AutoaimLayerMask { get; private set; }   // Mask to find autoaim targets
        protected int WeaponLayerMask { get; private set; }    // Mask to be tested
        protected int DamageableLayer { get; private set; }    // Layer with damageable objects
        #endregion

        WeaponState state;
        public WeaponState      State
        {
            get
            {
                return state;
            }
            private set
            {
                WeaponState prev = state;
                state = value;

                OnStateChange?.Invoke(prev, state);
            }
        }

        /// <summary>
        /// Was weapon jammed before disable?
        /// </summary>
        bool                    wasJammed;

        /// <summary>
        /// Current owner of this weapon
        /// </summary>
        public GameObject               Owner { get; private set; }
        protected WeaponsController     WController { get; private set; }
        #endregion

        #region weapon item properties
        public WeaponData       Data { get; private set; }

        public WeaponIndex      WeaponIndex { get; private set; }
        public float            DamageValue { get; private set; }
        public AmmunitionType   AmmoType { get; private set; }
        public int              AmmoConsumption { get; private set; }
        public float            ReloadingTime { get; private set; }
        /// <summary>
        /// Health in [0..1]
        /// </summary>
        public float            Health => (float)refHealth.Value / Durability;
        public float            PercentageForJam { get; private set; }
        public float            JamProbability { get; private set; }
        /// <summary>
        /// Accuracy in [0..1].
        /// '1' means perfect accuracy.
        /// </summary>
        public float            Accuracy { get; private set; }
        public bool             IsBroken => refHealth.Value <= 0;
        public int              Durability { get; private set; }

        public AudioClip        ShotSound { get; private set; }
        public AudioClip        UnjamSound { get; private set; }
        public AudioClip        BreakSound { get; private set; }

        #endregion

        #region initialization
        /// <summary>
        /// Init fields from player's items
        /// </summary>
        public void Init(WeaponsController controller, WeaponItem playerItem, IAmmoHolder playerAmmo)
        {
            Debug.Assert(WController == null, "Several initializations of the same weapon", controller);


            WController = controller;
            Owner = controller.CurrentPlayer.gameObject;


            ammo = playerAmmo;
            item = playerItem;


            // load info
            Data                = item.Stats;

            WeaponIndex         = item.This;

            DamageValue         = Data.Damage;
            AmmoType            = Data.AmmoType;
            AmmoConsumption     = Data.AmmoConsumption;
            ReloadingTime       = Data.ReloadingTime;
            Accuracy            = Data.Accuracy;

            Durability          = Data.Durability;
            refHealth           = item.HealthRef;
            JamProbability      = Data.JamProbability;

            PercentageForJam    = Data.PercentageForJam;

            ShotSound           = Data.ShotSound;
            BreakSound          = Data.BreakSound;
            UnjamSound          = Data.UnjamSound;


            // layers
            AutoaimLayerMask = LayerMask.GetMask(LayerNames.AutoaimTargets);
            WeaponLayerMask = LayerMask.GetMask(LayerNames.Default, LayerNames.Damageable);
            DamageableLayer = LayerMask.NameToLayer(LayerNames.Damageable);


            // get anim
            weaponAnimation = GetComponentInChildren<Animation>(true);

            string tempName = weaponAnimation.clip.name;
            tempName = tempName.Remove(0, 1);

            AnimShootName = "S" + tempName;
            AnimJamName = "J" + tempName;
            AnimUnjamName = "U" + tempName;
            AnimBreakName = "B" + tempName;


            // set hand animation
            handPivot = GetComponentInChildren<HandPivot>(true);
            if (handPivot != null)
            {
                string handAnimName = "H" + tempName;
                handPivot.Init(handAnimName);
            }

            // set state
            State = WeaponState.Nothing;
            wasJammed = false;

            InitWeapon();
        }
        #endregion

        #region overridable
        protected abstract void PrimaryAttack();

        /// <summary>
        /// Called on init
        /// </summary>
        protected virtual void InitWeapon() { }

        /// <summary>
        /// Called on weapon disable
        /// </summary>
        protected virtual void Deactivate() { }

        /// <summary>
        /// Called on weapon enable
        /// </summary>
        protected virtual void Activate() { }
        #endregion

        #region weapons
        protected void PlayAudio(AudioClip clip)
        {
            WController.PlaySound(clip);
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

        protected void ReduceAmmo()
        {
            int newAmount = ammo.Get(AmmoType).CurrentAmount - AmmoConsumption;

            ammo.Get(AmmoType).CurrentAmount = newAmount;
            OnAmmoChange?.Invoke(newAmount);

            // if run out of ammo, send event
            if (newAmount == 0)
            {
                OnAmmoRunOut?.Invoke(WeaponIndex);
            }
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
            if (!AllWeaponsStats.CanJam(AmmoType))
            {
                return false;
            }

            if (refHealth.Value >= 3)
            {
                float healthPercentage = (float)refHealth.Value / Durability;

                return healthPercentage > PercentageForJam ? false : Random.Range(0.0f, 1.0f) < JamProbability;
            }
            else if (refHealth.Value == 2)
            {
                return Random.Range(0.0f, 1.0f) > 0.3f;
            }
            else
            {
                // last shot always jam
                return true;
            }
        }

        /// <summary>
        /// Shortcut for 'AllWeaponsStats.Instance.CanBreak'
        /// </summary>
        bool CanBreak()
        {
            return AllWeaponsStats.CanBreak(AmmoType);
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
            if (State != WeaponState.Breaking 
                && State != WeaponState.Ready
                && State != WeaponState.ReadyForUnjam)
            {
                Debug.LogWarning("Wrong weapon state");
                return;
            }

            // mark, it will be used when weapon will be enabled
            wasJammed = State == WeaponState.ReadyForUnjam;

            State = WeaponState.Disabling;

            // disable current ammo amount
            OnAmmoChange?.Invoke(-1);

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

                case WeaponState.ReadyForUnjam:
                    Disable();
                    return false;

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
        /// Must be called when weapons' controller is reinitting
        /// </summary>
        public void Reinit()
        {
            wasJammed = State == WeaponState.ReadyForUnjam;
            State = WeaponState.Nothing;

            OnAmmoChange?.Invoke(-1);

            gameObject.SetActive(false);
            Deactivate();
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

        HandsController handsController;

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

            // this weapon is activating,
            // show its ammo
            OnAmmoChange?.Invoke(ammo.Get(AmmoType).CurrentAmount);

            // pose hand
            if (handPivot != null)
            {
                //handPivot.PoseHand();
            }
            else
            {
                if (handsController == null)
                {
                    handsController = FindObjectOfType<HandsController>();
                }

                if (handsController != null)
                {
                    // if doesn't exist, dont render it
                    handsController.RenderRightHand = false;
                }
            }

            // wait for enabling
            if (!wasJammed)
            {
                StartCoroutine(Wait(TakingOutTime, WeaponState.Ready));
            }
            else
            {
                StartCoroutine(Wait(TakingOutTime, WeaponState.ReadyForUnjam));
            }
        }

        public void Fire()
        {
            if (ammo.Get(AmmoType).CurrentAmount < AmmoConsumption)
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
            if (Data.CanBeDamaged)
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

            OnShot?.Invoke();

            // wait for reload
            StartCoroutine(WaitForShoot(nextState));
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
            yield return new WaitForSeconds(weaponAnimation[AnimBreakName].length * 2);

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

        /// <summary>
        /// Wait reloading time, then switch to next state
        /// and call OnShootFinish event, if next state is 'Ready'
        /// </summary>
        /// <returns></returns>
        IEnumerator WaitForShoot(WeaponState nextState)
        {
            yield return new WaitForSeconds(ReloadingTime);
            State = nextState;

            if (nextState == WeaponState.Ready)
            {
                OnShootFinish?.Invoke(WeaponIndex);
            }
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