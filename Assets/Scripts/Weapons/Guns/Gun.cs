using UnityEngine;

namespace SD.Weapons
{
    class Gun : HitscanWeapon
    {
        const float AccuracyMultiplier = 3;

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

            CheckRay(AimTransform.position, distortedDir);
        }
    }
}
