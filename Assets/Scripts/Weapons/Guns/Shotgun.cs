using UnityEngine;

namespace SD.Weapons
{
    class Shotgun : HitscanWeapon
    {
        const float AccuracyMultiplier = 18;

        [SerializeField]
        private int pelletCount = 7;
        [SerializeField]
        private AudioClip reloadSound;

        protected override void Hitscan()
        {                
            // autoaim
            Vector3 aimedDir;
            Autoaim.Aim(AimTransform.position, AimTransform.forward, AimRadius, out aimedDir, Range, WeaponLayerMask);

            float rangey = 0.3f * Accuracy;

            float deltax = 2 * Accuracy / pelletCount;
            float basex = -Accuracy;

            rangey *= AccuracyMultiplier;
            deltax *= AccuracyMultiplier;
            basex *= AccuracyMultiplier;

            for (int i = 0; i < pelletCount; i++)
            {
                // process accuracy
                float rangexl = basex + deltax * i;
                float rangexr = basex + deltax * (i + 1);

                Vector3 distortedDir =
                    Quaternion.AngleAxis(Random.Range(rangexl, rangexr), AimTransform.up)
                    * Quaternion.AngleAxis(Random.Range(-rangey, rangey), AimTransform.right)
                    * aimedDir;

                print(distortedDir);

                CheckRay(AimTransform.position, distortedDir);
            }
        }

        public void PlayAdditionalSound()
        {
            PlayAudio(reloadSound);
        }
    }
}
