using UnityEngine;

namespace SD.Weapons
{
    class Gun : HitscanWeapon
    {
        protected override void Hitscan()
        {
            // autoaim
            Autoaim.Aim(AimTransform.position, AimTransform.forward, AimRadius, out Vector3 aimedDir, Range, AutoaimLayerMask);

            // process accuracy
            float rangex = (1 - Accuracy) * MaxAngleX;
            float rangey = (1 - Accuracy) * MaxAngleY;

            Vector3 distortedDir = HealthInt >= 3 ?
                Quaternion.AngleAxis(Random.Range(-rangex, rangex), AimTransform.up)
                * Quaternion.AngleAxis(Random.Range(-rangey, rangey), AimTransform.right)
                * aimedDir :
                aimedDir;

            Vector3 end = CheckRay(AimTransform.position, distortedDir);

            // emit trail, if muzzle flash transform exists
            if (MuzzleFlash != null)
            {
                EmitTrail(MuzzleFlash.position, distortedDir, end);
            }
        }
    }
}
