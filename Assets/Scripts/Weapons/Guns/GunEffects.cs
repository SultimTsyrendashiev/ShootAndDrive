﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SD.PlayerLogic;

namespace SD.Weapons
{
    /// <summary>
    /// Special script for transfering info from animation events
    /// </summary>
    public class GunEffects : MonoBehaviour
    {
        HitscanWeapon currentWeapon;

        void Start()
        {
            currentWeapon = GetComponentInParent<HitscanWeapon>();
            Debug.Assert(currentWeapon != null, this);
        }

        public void EmitCasings()
        {
            currentWeapon.EmitCasings();
        }

        public void PlayAdditionalSound()
        {
            if (currentWeapon is Shotgun)
            {
                ((Shotgun)currentWeapon).PlayAdditionalSound();
            }
        }
    }
}
