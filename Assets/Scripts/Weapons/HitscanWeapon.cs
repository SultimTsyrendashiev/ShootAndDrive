using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SD.Player;

namespace SD.Weapons
{
    /// <summary>
    /// Hitscan weapons.
    /// There should be children called "Casings" and "Muzzle" to emit particles.
    /// </summary>
    abstract class HitscanWeapon : Weapon
    {
        // Assume that this is readonly
        protected int DamageableLayer;

        protected Camera PlayerCamera;
        protected Transform Casings;
        protected Transform MuzzleFlash;

        protected float AimRadius = 2.0f;
        protected float Range = 150.0f;

        void Start()
        {
            DamageableLayer = LayerMask.NameToLayer(LayerNames.Damageable);
            PlayerCamera = Player.Player.Instance.MainCamera;
            Casings = transform.Find("Casings");
            MuzzleFlash = transform.Find("Muzzle");
        }

        protected abstract void Hitscan();

        protected void CheckRay(Vector3 from, Vector3 direction)
        {
            Vector3 aimedDir;
            Autoaim.Aim(from, direction, AimRadius, out aimedDir, WeaponLayerMask);

            // process accuracy

            RaycastHit hit;
            if (Physics.Raycast(from, aimedDir, out hit, Range, WeaponLayerMask))
            {
                if (hit.collider.gameObject.layer == DamageableLayer)
                {
                    Damage dmg = new Damage(DamageValue, DamageType.Bullet, from, hit.point, hit.normal, Player.Player.Instance.gameObject);
                    hit.collider.GetComponent<IDamageable>().ReceiveDamage(dmg);
                }
                else
                {
                    // else only generate particles

                }
            }
        }

        protected override void PrimaryAttack()
        {
            // main
            Hitscan();
            ReduceAmmo();

            // animation
            if (State != WeaponState.Jamming)
            {
                PlayPrimaryAnimation();
            }
            else
            {
                PlayJammingAnimation();
            }
            
            // sound
            PlayAudio(ShotSound);

            // camera
            RecoilJump();

            // particles
            if (MuzzleFlash != null)
            {
                WeaponsParticles.Instance.EmitMuzzle(MuzzleFlash.position, MuzzleFlash.rotation);
            }

            if (Casings != null && State != WeaponState.Jamming)
            {
                WeaponsParticles.Instance.EmitCasings(Casings.position, Casings.rotation, this.AmmoType, AmmoConsumption);
            }
        }

        protected override void UnjamAdditional()
        {
            WeaponsParticles.Instance.EmitCasings(Casings.position, Casings.rotation, this.AmmoType, AmmoConsumption);
        }
    }
}
