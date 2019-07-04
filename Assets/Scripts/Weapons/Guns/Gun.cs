﻿using UnityEngine;

namespace SD.Weapons
{
    class Gun : HitscanWeapon
    {
        const float AccuracyMultiplier = 4;

        protected override void Hitscan()
        {
            // autoaim
            Autoaim.Aim(AimTransform.position, AimTransform.forward, AimRadius, out Vector3 aimedDir, Range, AutoaimLayerMask);

            // process accuracy
            float rangex = 2 * Accuracy * AccuracyMultiplier;
            float rangey = Accuracy * AccuracyMultiplier;

            Vector3 distortedDir =
                Quaternion.AngleAxis(Random.Range(-rangex, rangex), AimTransform.up)
                * Quaternion.AngleAxis(Random.Range(-rangey, rangey), AimTransform.right)
                * aimedDir;

            Vector3 end = CheckRay(AimTransform.position, distortedDir);

            // emit trail, if muzzle flash transform exists
            if (MuzzleFlash != null)
            {
                EmitTrail(MuzzleFlash.position, end);
            }
        }
    }
}
