﻿using UnityEngine;
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
        protected const float   MaxAngleX = 6;
        protected const float   MaxAngleY = 3;

        /// <summary>
        /// Casings spawn transform must have this name
        /// </summary>
        const string            CasingsTransformName = "Casings";
        /// <summary>
        /// Muzzle spawn
        /// </summary>
        const string            MuzzleTransformName = "Muzzle";

        // What particles to use for muzzle flash
        protected string        MuzzleParticlesName = WeaponsController.MuzzleFlash;
        // What paarticles to use when hit background
        const string            DefaultHitParticleName = "DefaultParticles";

        [SerializeField]
        protected Transform     AimTransform;
        protected Transform     Casings;
        protected Transform     MuzzleFlash;
        [SerializeField]
        protected bool          ShowMuzzleFlash = true;

        protected const float   AimRadius = 1.9f;
        protected const float   Range = 300.0f;

        void Start()
        {
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

                    var damageable = hit.collider.GetComponent<IDamageable>();
                    Debug.Assert(damageable != null, "Each object on Damageable layer must contain IDamageable script", hit.collider);

                    damageable.ReceiveDamage(dmg);
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

        protected void EmitTrail(Vector3 start, Vector3 direction, Vector3 end)
        {
            WController.ShowLine(start, direction, end, 0.06f);
        }
    }
}
