using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SD.PlayerLogic;
// TODO: use events
using SD.UI;

namespace SD.Weapons
{
    /// <summary>
    /// Player's weapons controller.
    /// All weapons must be children of this object
    /// </summary>
    class WeaponsController : MonoBehaviour
    {
        bool                playerIsActive;

        WeaponsHolder       inventoryWeapons;   // weapons in player's inventory
        Dictionary<WeaponIndex, Weapon> weapons; // actual weapons in a scene

        Maybe<WeaponIndex>  currentWeapon;      // current player's weapon (if exist)
        WeaponIndex         nextWeapon;         // weapon to switch on

        AmmoHolder          inventoryAmmo;      // weapons in player's inventory

        AudioSource[]       audioSources;       // audio sources for weapons
        int                 audioSourceIndex;   // last audio source  

        bool                isSwitching;
        bool                canSwitchToAnotherNext;

        Animation           commonAnimation;
        const string        animHide    = "WeaponHide";
        const string        animTakeOut = "WeaponTake";

        /// <summary>
        /// Player which holds this weapons
        /// </summary>
        public Player CurrentPlayer { get; private set; }
        public AllWeaponsStats Stats { get; private set; }
        public static WeaponsController Instance { get; private set; }
     
        #region init
        void Awake()
        {
            Instance = this;
        }

        public void Init(Player player)
        {
            playerIsActive = true;
            CurrentPlayer = player;

            // init weapons
            inventoryWeapons = CurrentPlayer.Inventory.Weapons;
            inventoryAmmo = CurrentPlayer.Inventory.Ammo;

            Weapon[] ws = GetComponentsInChildren<Weapon>(true);
            weapons = new Dictionary<WeaponIndex, Weapon>();

            foreach (Weapon w in ws)
            {
                // scene's weapon index is not set
                // so parse it
                WeaponIndex index = (WeaponIndex)Enum.Parse(typeof(WeaponIndex), w.gameObject.name);

                // include it only if available in inventory
                // if (inventoryWeapons.IsAvailable(index))
                {
                    weapons.Add(index, w);
                    w.Init(this, inventoryWeapons.Get(index), inventoryAmmo);
                }

                // hide
                w.gameObject.SetActive(false);
            }

            // get audio sources
            audioSources = GetComponentsInChildren<AudioSource>();
            audioSourceIndex = 0;
            Debug.Assert(audioSources.Length != 0, "No audio source for weapons");

            // animation
            commonAnimation = GetComponent<Animation>();
            Debug.Assert(commonAnimation != null);

            // states
            isSwitching = false;
            canSwitchToAnotherNext = false;
            currentWeapon = new Maybe<WeaponIndex>();

            // events
            Weapon.OnWeaponBreak += ProcessWeaponBreak;
            CurrentPlayer.OnPlayerStateChange += ProcessPlayerStateChange;

            // set parameters for weapons particles
            InitParticles();
        }
        #endregion

        private void ProcessPlayerStateChange(PlayerState state)
        {
            switch(state)
            {
                case PlayerState.Dead:
                case PlayerState.Regenerating:
                    HideWeapon();
                    playerIsActive = false;
                    break;
                case PlayerState.Ready:
                    playerIsActive = true;
                    TakeOutWeapon();
                    break;
            }
        }

        void OnDestroy()
        {
            // unsign from events to enable GC
            Weapon.OnWeaponBreak -= ProcessWeaponBreak;
            CurrentPlayer.OnPlayerStateChange -= ProcessPlayerStateChange;
        }

        public void Fire()
        {
            if (!playerIsActive)
            {
                return;
            }

            if (!currentWeapon.Exist)
            {
                return;
            }

            Weapon current = weapons[currentWeapon.Value];

            // if weapon in these states
            // fire button event must be ignored
            if (current.State == WeaponState.Breaking
                || current.State == WeaponState.Jamming
                || current.State == WeaponState.Disabling 
                || current.State == WeaponState.Nothing)
            {
                return;
            }

            // There is no special button for unajamming,
            // so fire button is used.
            // But player must retap on it.
            if (current.State == WeaponState.ReadyForUnjam && current.State != WeaponState.Unjamming)
            {
                // if weapon is jamming, unjam it
                current.Unjam();
                return;
            }

            StartCoroutine(WaitForFire(current));
        }

        IEnumerator WaitForFire(Weapon w)
        {
            // while player is holding fire button
            while (InputController.FireButton && playerIsActive)
            {
                // if ready, then shoot
                if (w.State == WeaponState.Ready)
                {
                    w.Fire();
                    yield return new WaitForSeconds(w.ReloadingTime);
                }
                else if (w.State == WeaponState.Unjamming 
                        || w.State == WeaponState.Reloading
                        || w.State == WeaponState.Enabling)
                {
                    // in this states, player holds button
                    // and wants weapon to shoot
                    // but he must wait until state Ready
                    yield return null;
                }
                else
                {
                    yield break;
                }
            }
        }

        /// <summary>
        /// Take out current weapon.
        /// Note: only current
        /// </summary>
        public void TakeOutWeapon()
        {
            if (!playerIsActive)
            {
                return;
            }

            // ignore if current weapon will change
            if (isSwitching)
            {
                return;
            }

            // if not hidden
            if (currentWeapon.Exist)
            {
                return;
            }

            // check if available in inventorys
            if (!inventoryWeapons.IsAvailable(currentWeapon.Value))
            {
                return;
            }

            // if not hidden (according to its state)
            if (weapons[currentWeapon.Value].State != WeaponState.Nothing)
            {
                return;
            }

            weapons[currentWeapon.Value].Enable();
            commonAnimation.Play(animTakeOut);

            // now there is current weapon
            // note: it's ok not to wait 
            //       for actual weapon enabling (i.e. state == Ready)
            currentWeapon.Exist = true;
        }

        /// <summary>
        /// Expected that there will be waiting for '!IsBusy()'
        /// after calling this function, if there will be action after hiding
        /// </summary>
        public void HideWeapon()
        {
            // ignore if current weapon will change
            if (isSwitching)
            {
                return;
            }

            // if already hidden
            if (!currentWeapon.Exist)
            {
                return;
            }

            // if can be hidden
            if (weapons[currentWeapon.Value].State != WeaponState.Ready)
            {
                return;
            }

            weapons[currentWeapon.Value].ForceDisable();
            commonAnimation.Play(animHide);

            // there is no current weapon anymore
            // note: it's ok not to wait 
            //       for actual weapon hiding (i.e. state == Nothing)
            currentWeapon.Exist = false;
        }

        public void SwitchTo(WeaponIndex w)
        {
            if (!playerIsActive)
            {
                return;
            }

            // if there is current weapon,
            // don't switch to same weapon
            if (currentWeapon.Exist && currentWeapon.Value == w)
            {
                return;
            }

            // if next is already processing
            if (nextWeapon == w)
            {
                return;
            }

            // check if bought and not broken
            if (!inventoryWeapons.IsAvailable(w))
            {
                return;
            }

            // if currently switching to one weapon
            // but player changed his mind 
            // and now wants another weapon;
            // it's possible if old desired weapon
            // is not appeared yet
            if (isSwitching && canSwitchToAnotherNext)
            {
                // just reassign next weapon
                // and don't call WaitForSwitch()
                nextWeapon = w;
                return;
            }
            else if (isSwitching && !canSwitchToAnotherNext)
            {
                // old desired weapon is already appeared
                // so start new coroutine
                StartCoroutine(WaitForNewNext(w));

                return;
            }

            // note: it's impossible 
            // if 'isSwitching' = 0
            // and 'canSwitchToAnotherNext' = 1

            // default
            nextWeapon = w;
            StartCoroutine(WaitForSwitch());
        }

        /// <summary>
        /// Wait current weapon to finish
        /// and then enable new
        /// </summary>
        IEnumerator WaitForSwitch()
        {
            isSwitching = true;
            canSwitchToAnotherNext = true;

            // if there is current weapon
            // (else just enable next)
            if (currentWeapon.Exist)
            {
                // disable current
                // if state is not Nothing then wait for disabling state
                if (!weapons[currentWeapon.Value].ForceDisable())
                {
                    // wait for disabling state
                    while (weapons[currentWeapon.Value].State != WeaponState.Disabling)
                    {
                        yield return null;
                    }

                    // weapon is now disabling, so play animation
                    commonAnimation.Play(animHide);

                    // wait for hiding
                    yield return new WaitForSeconds(weapons[currentWeapon.Value].HidingTime);
                }
            }

            // if player changed his mind and wants another
            // now he can't switch to it;
            // he must wait for previous
            canSwitchToAnotherNext = false;

            // enable next
            weapons[nextWeapon].Enable();
            commonAnimation.Play(animTakeOut);

            // set new current
            currentWeapon.Exist = true;
            currentWeapon.Value = nextWeapon;

            isSwitching = false;
        }

        IEnumerator WaitForNewNext(WeaponIndex newNext)
        {
            // wait for old switching
            while (isSwitching)
            {
                yield return null;
            }

            // now it's safe to reassign 'nextWeapon'
            nextWeapon = newNext;
            StartCoroutine(WaitForSwitch());
        }
        
        /// <summary>
        /// Called on weapon break event
        /// </summary>
        void ProcessWeaponBreak(WeaponIndex brokenWeapon)
        {
            // if there is no current weapon, then ignore
            if (!currentWeapon.Exist)
            {
                return;
            }

            // if not current, then ignore
            if (currentWeapon.Value != brokenWeapon)
            {
                Debug.Log("Incorrect broken weapon", this);
                return;
            }

            // if already switching to another weapon
            if (isSwitching)
            {
                return;
            }

            // try to switch to next available
            if (GetNextAvailable(brokenWeapon, out WeaponIndex available))
            {
                // if found
                SwitchTo(available);
                return;
            }

            // else, just disable broken
            weapons[brokenWeapon].ForceDisable();
            commonAnimation.Play(animHide);

            // there is no current weapon anymore
            // note: it's ok not to wait 
            //       for actual weapon hiding (i.e. state == Nothing)
            currentWeapon.Exist = false;
        }

        public bool GetCurrentWeaponState(out WeaponState result)
        {
            result = weapons[currentWeapon.Value].State;
            return currentWeapon.Exist;
        }

        public bool IsBusy()
        {
            return isSwitching 
                // if there is weapon, but it's not ready
                || (currentWeapon.Exist && weapons[currentWeapon.Value].State != WeaponState.Ready)
                // if there is no weapon, but previous wasn't really hidden yet
                || (!currentWeapon.Exist && weapons[currentWeapon.Value].State == WeaponState.Nothing);
        }

        public bool GetNextAvailable(WeaponIndex weapon, out WeaponIndex availableWeapon, bool shiftToRight = true)
        {
            // try to switch to next available
            int allWeaponsCount = Enum.GetValues(typeof(WeaponIndex)).Length;

            int current = (int)weapon;
            int available = current;

            do
            {
                if (shiftToRight)
                {
                    available = available + 1 >= allWeaponsCount ? 0 : available + 1;
                }
                else
                {
                    available = available - 1 < 0 ? allWeaponsCount - 1 : available - 1;
                }

                if (inventoryWeapons.IsAvailable((WeaponIndex)available))
                {
                    // if found
                    availableWeapon = (WeaponIndex)available;
                    return true;
                }

            } while (available != current);

            availableWeapon = weapon;
            return false;
        }

        public bool GetCurrentWeapon(out WeaponIndex weapon)
        {
            weapon = currentWeapon.Value;
            return currentWeapon.Exist;
        }

        #region weapons effects
        const string CasingsPistol = "CasingsPistol";
        const string CasingsRifle = "CasingsRifle";
        const string CasingsHeavyPart = "CasingsHeavyPart";
        const string CasingsShells = "CasingsShells";
        const string CasingsGrenade = "CasingsGrenade";
        const string MuzzleFlash = "MuzzleFlash";

        /// <summary>
        /// Set new simulation space for weapon particles
        /// </summary>
        void InitParticles()
        {
            string[] particles = { CasingsPistol, CasingsRifle, CasingsHeavyPart,
                CasingsShells, CasingsGrenade, MuzzleFlash };

            foreach (string s in particles)
            {
                var main = ParticlesPool.Instance.GetParticleSystem(s).main;
                main.simulationSpace = ParticleSystemSimulationSpace.Custom;
                main.customSimulationSpace = transform;
            }
        }

        public void EmitCasings(Vector3 position, Quaternion rotation, AmmunitionType type, int amount = 1)
        {
            // particle system to emit
            string system = null;

            switch (type)
            {
                case AmmunitionType.BulletsHeavy:
                    ParticlesPool.Instance.Emit(CasingsHeavyPart, position, rotation, amount);
                    system = CasingsRifle;
                    break;

                case AmmunitionType.Bullets:
                    system = CasingsRifle;
                    break;

                case AmmunitionType.BulletsPistol:
                    system = CasingsPistol;
                    break;

                case AmmunitionType.Shells:
                    system = CasingsShells;
                    break;

                case AmmunitionType.Grenades:
                    system = CasingsGrenade;
                    break;
            }

            if (system != null)
            {
                ParticlesPool.Instance.Emit(system, position, rotation, amount);
            }
        }

        public void PlaySound(AudioClip clip)
        {
            if (audioSourceIndex >= audioSources.Length)
            {
                audioSourceIndex = 0;
            }

            audioSources[audioSourceIndex].PlayOneShot(clip);
            audioSourceIndex++;
        }
        #endregion
    }
}
