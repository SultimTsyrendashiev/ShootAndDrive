using System.Collections;
using UnityEngine;

namespace SD.Weapons
{
    /// <summary>
    /// There is a small delay before hitscan
    /// </summary>
    class Revolver : Gun
    {
        [SerializeField]
        float triggerTime = 0.083f;

        protected override void PrimaryAttack()
        {
            // start animation
            PlayPrimaryAnimation();
            // and wait for delay
            StartCoroutine(WaitForTrigger());
        }

        IEnumerator WaitForTrigger()
        {
            yield return new WaitForSeconds(triggerTime);
            
            // same as base.PrimaryAttack(), but without animation

            // main
            Hitscan();
            ReduceAmmo();

            // effects
            PlayAudio(ShotSound);
            RecoilJump();

            // particles
            if (MuzzleFlash != null && ShowMuzzleFlash)
            {
                ParticlesPool.Instance.Play(MuzzleParticlesName, MuzzleFlash.position, MuzzleFlash.rotation);
            }
        }
    }
}
