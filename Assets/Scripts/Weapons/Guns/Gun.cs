using System;
using System.Collections.Generic;
using UnityEngine;

namespace SD.Weapons
{
    class Gun : HitscanWeapon
    {
        public override void PrimaryAttack()
        {
            CheckRay(PlayerCamera.transform.position, PlayerCamera.transform.forward);

            PlayPrimaryAnimation();
            PlayAudio(ShotSound);

            Ammo.Add(this.AmmoType, -AmmoConsumption);
        }
    }
}
