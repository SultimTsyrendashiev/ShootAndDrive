using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SD.Player;
using SD.UI;

namespace SD.Weapons
{
    // Player's weapons controller.
    // All weapons must be children of this object
    class WeaponsController : MonoBehaviour
    {
        WeaponsHolder       inventoryWeapons;   // weapons in player's inventory
        Dictionary<WeaponIndex, Weapon> weapons; // actual weapons in a scene

        WeaponIndex         currentWeapon;      // current player's weapon
        WeaponIndex         nextWeapon;         // weapon to switch on

        AmmoHolder          inventoryAmmo;      // weapons in player's inventory

        AudioSource[]       audioSources;       // audio sources for weapons
        int                 audioSourceIndex;   // last audio source  

        Animation           commonAnimation;
        const string        animHide    = "WeaponHide";
        const string        animTakeOut = "WeaponTake";

        static WeaponsController instance;
        public static WeaponsController Instance => instance;

        void Start()
        {
            instance = this;

            // init weapons
            inventoryWeapons = Player.Player.Instance.Inventory.Weapons;
            inventoryAmmo = Player.Player.Instance.Inventory.Ammo;
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
                    w.Init(inventoryWeapons.Get(index), inventoryAmmo);
                }

                // hide
                w.gameObject.SetActive(false);
            }

            // get audio sources
            audioSources = GetComponentsInChildren<AudioSource>();
            audioSourceIndex = 0;
            Debug.Assert(audioSources.Length != 0, "No audio source for weapons");

            commonAnimation = GetComponent<Animation>();
            Debug.Assert(commonAnimation != null);

            // for testing
            TakeOutWeapon();
        }

        public void Fire()
        {
            Weapon current = weapons[currentWeapon];

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
            while (InputController.FireButton)
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

        public void TakeOutWeapon()
        {
            if (weapons[currentWeapon].State != WeaponState.Nothing)
            {
                return;
            }

            weapons[currentWeapon].Enable();
            commonAnimation.Play(animTakeOut);
        }

        /// <summary>
        /// Expected that there will be waiting for state Nothing
        /// after calling this function
        /// </summary>
        public void HideWeapon()
        {
            weapons[currentWeapon].ForceDisable();
            commonAnimation.Play(animHide);
        }

        public void SwitchTo(WeaponIndex w)
        {
            // check if bought or not broken
            if (!inventoryWeapons.IsAvailable(w))
            {
                return;
            }

            nextWeapon = w;

            StartCoroutine(WaitForSwitch());
        }

        /// <summary>
        /// Wait current weapon to finish
        /// and then enable new
        /// </summary>
        IEnumerator WaitForSwitch()
        {
            // disable current
            weapons[currentWeapon].ForceDisable();
            commonAnimation.Play(animHide);

            // wait to disable
            yield return new WaitForSeconds(weapons[currentWeapon].TakingOutTime);

            // enable next
            weapons[nextWeapon].Enable();
            commonAnimation.Play(animTakeOut);

            // set new current
            currentWeapon = nextWeapon;
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


        public WeaponState GetCurrentWeaponState()
        {
            // TODO: check if there is no weapons
            return weapons[currentWeapon].State;
        }
    }
}
