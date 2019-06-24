using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SD.Weapons;

namespace SD.Player
{
    // Player's weapons controller.
    // All weapons must be children of this object
    class WeaponsController : MonoBehaviour
    {
        WeaponsHolder   inventoryWeapons;   // weapons in player's inventory
        Dictionary<WeaponsEnum, Weapon> weapons; // actual weapons in a scene

        WeaponsEnum     currentWeapon;      // current player's weapon
        WeaponsEnum     nextWeapon;         // weapon to switch on

        AmmoHolder      inventoryAmmo;      // weapons in player's inventory

        AudioSource[]   audioSources;       // audio sources for weapons
        int             audioSourceIndex;   // last audio source  

        static WeaponsController instance;
        public static WeaponsController Instance => instance;

        void Start()
        {
            // init weapons
            inventoryWeapons = Player.Instance.Inventory.Weapons;
            inventoryAmmo = Player.Instance.Inventory.Ammo;
            Weapon[] ws = GetComponentsInChildren<Weapon>(true);
            weapons = new Dictionary<WeaponsEnum, Weapon>();

            foreach (Weapon w in ws)
            {
                // scene's weapon index is not set
                // so parse it
                WeaponsEnum index = (WeaponsEnum)Enum.Parse(typeof(WeaponsEnum), w.gameObject.name);

                // include it only if available in inventory
                // if (inventoryWeapons.IsAvailable(index))
                {
                    weapons.Add(index, w);
                    w.Init(inventoryWeapons.Get(index), inventoryAmmo);
                }
            }

            // get audio sources
            audioSources = GetComponentsInChildren<AudioSource>();
            audioSourceIndex = 0;

            Debug.Assert(audioSources.Length != 0, "No audio source for weapons");

            // for testing
            TakeOutWeapon();
        }

        public void TakeOutWeapon()
        {
            if (weapons[currentWeapon].State != WeaponState.Nothing)
            {
                return;
            }

            weapons[currentWeapon].Enable();
        }

        public void HideWeapon()
        {
            weapons[currentWeapon].ForceDisable();
        }

        public void SwitchTo(WeaponsEnum w)
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

            // wait to disable
            while (weapons[currentWeapon].State == WeaponState.Nothing)
            {
                yield return null;
            }

            // enable next
            weapons[nextWeapon].Enable();

            // set new current
            currentWeapon = nextWeapon;

            // next should not be changed
            // as it doesnt matter
            // if new next weapon is needed then current next will be ovewritten
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
    }
}
