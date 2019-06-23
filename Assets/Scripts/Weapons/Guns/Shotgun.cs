using System;
using System.Collections.Generic;
using UnityEngine;

namespace SD.Weapons
{
    class Shotgun : HitscanWeapon
    {
        [SerializeField]
        private int PelletCount = 7;

        protected override void Hitscan()
        {
            for (int i = 0; i < PelletCount; i++)
            {
                CheckRay(PlayerCamera.transform.position, PlayerCamera.transform.forward);
            }
        }
    }
}
