﻿using UnityEngine;

namespace SD.Weapons
{
    class Shotgun : HitscanWeapon
    {
        [SerializeField]
        private int pelletCount = 7;
        [SerializeField]
        private AudioClip reloadSound;

        protected override void InitWeapon()
        {
            MuzzleParticlesName = WeaponsController.MuzzleFlashShotgun;
            RecoilJumpMultiplier *= 2;
        }

        protected override void Hitscan()
        {                
            // autoaim
            Vector3 aimedDir;
            Autoaim.Aim(AimTransform.position, AimTransform.forward, AimRadius, out aimedDir, Range, AutoaimLayerMask);

            float rangey = 1 - Accuracy;
            rangey *= MaxAngleY;

            float deltax = 2 * (1 - Accuracy) / pelletCount;
            float basex = -(1 - Accuracy);

            if (HealthInt >= 3)
            {
                deltax *= MaxAngleX;
                basex *= MaxAngleX;
            }
            else
            {
                deltax *= MaxAngleX / 3.0f;
                basex *= MaxAngleX / 3.0f;
            }

            for (int i = 0; i < pelletCount; i++)
            {
                // process accuracy
                float rangexl = basex + deltax * i;
                float rangexr = basex + deltax * (i + 1);

                Vector3 distortedDir =
                    Quaternion.AngleAxis(Random.Range(rangexl, rangexr), AimTransform.up)
                    * Quaternion.AngleAxis(Random.Range(-rangey, rangey), AimTransform.right)
                    * aimedDir;

                CheckRay(AimTransform.position, distortedDir);

                //Vector3 end = CheckRay(AimTransform.position, distortedDir);

                //// emit trail, if muzzle flash transform exists
                //if (MuzzleFlash != null)
                //{
                //    EmitTrail(MuzzleFlash.position, end);
                //}
            }
        }

        public void PlayAdditionalSound()
        {
            PlayAudio(reloadSound);
        }
    }
}
