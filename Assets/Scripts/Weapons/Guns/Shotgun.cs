using System;
using System.Collections.Generic;
using UnityEngine;

namespace SD.Weapons
{
    class Shotgun : HitscanWeapon
    {
        [SerializeField]
        private int pelletCount = 7;
        [SerializeField]
        private AudioClip reloadSound;

        protected override void Hitscan()
        {
            for (int i = 0; i < pelletCount; i++)
            {
                CheckRay(PlayerCamera.transform.position, PlayerCamera.transform.forward);
            }
        }

        public void PlayAdditionalSound()
        {
            PlayAudio(reloadSound);
        }
    }
}
