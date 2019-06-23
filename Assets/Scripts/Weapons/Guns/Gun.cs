using System;
using System.Collections.Generic;
using UnityEngine;

namespace SD.Weapons
{
    class Gun : HitscanWeapon
    {
        protected override void Hitscan()
        {
            CheckRay(PlayerCamera.transform.position, PlayerCamera.transform.forward);
        }
    }
}
