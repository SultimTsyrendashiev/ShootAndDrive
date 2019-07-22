using UnityEngine;
using SD.PlayerLogic;

namespace SD.Weapons
{
    /// <summary>
    /// Hitscan weapons.
    /// There should be children called "Casings" and "Muzzle" to emit particles.
    /// </summary>
    abstract class HitscanWeapon : Weapon
    {
        /// <summary>
        /// Max not accurate angle
        /// </summary>
        protected const float MaxAngleX = 6;
        protected const float MaxAngleY = 3;

        /// <summary>
        /// Casings spawn transform must have this name
        /// </summary>
        const string CasingsTransformName = "Casings";
        /// <summary>
        /// Muzzle spawn
        /// </summary>
        const string MuzzleTransformName = "Muzzle";

        // What particles to use for muzzle flash
        const string MuzzleParticlesName = "MuzzleFlash";
        // What paarticles to use when hit background
        const string DefaultHitParticleName = "DefaultParticles";

        protected Transform AimTransform;
        protected Transform Casings;
        protected Transform MuzzleFlash;
        [SerializeField]
        protected bool ShowMuzzleFlash = true;

        protected const float AimRadius = 1.9f;
        protected const float Range = 300.0f;

        void Start()
        {
            AimTransform = CameraShaker.Instance.transform;
            Casings = FindChildByName(CasingsTransformName);
            MuzzleFlash = FindChildByName(MuzzleTransformName);
        }

        /// <summary>
        /// Override this method for special hitscan.
        /// 'CheckRay' should be used
        /// </summary>
        protected abstract void Hitscan();

        /// <summary>
        /// Checks ray
        /// </summary>
        /// <returns>
        /// hit point if exists; 
        /// else point on the end of this ray
        /// </returns>
        protected Vector3 CheckRay(Vector3 from, Vector3 direction)
        {
            if (Physics.Raycast(from, direction, out RaycastHit hit, Range, WeaponLayerMask))
            {
                if (hit.collider.gameObject.layer == DamageableLayer)
                {
                    Damage dmg = Damage.CreateBulletDamage(DamageValue, direction, hit.point, hit.normal, Owner);
                    hit.collider.GetComponent<IDamageable>().ReceiveDamage(dmg);
                }
                else
                {
                    // else only generate particles
                    ParticlesPool.Instance.Play(DefaultHitParticleName, hit.point, Quaternion.LookRotation(hit.normal));
                }

                return hit.point;
            }

            return from + Range * direction;
        }

        protected override void PrimaryAttack()
        {
            // main
            Hitscan();
            ReduceAmmo();

            // effects
            PlayPrimaryAnimation();
            PlayAudio(ShotSound);
            RecoilJump();

            // particles
            if (MuzzleFlash != null && ShowMuzzleFlash)
            {
                ParticlesPool.Instance.Play(MuzzleParticlesName, MuzzleFlash.position, MuzzleFlash.rotation);
            }
        }

        // called in animation
        public void EmitCasings()
        {
            if (Casings != null)
            {
                WController.EmitCasings(Casings.position, Casings.rotation, AmmoType, AmmoConsumption);
            }
        }

        protected void EmitTrail(Vector3 start, Vector3 end)
        {
            // TODO
        }
    }
}
