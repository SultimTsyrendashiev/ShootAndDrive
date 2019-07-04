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
        protected Transform AimTransform;
        protected Transform Casings;
        protected Transform MuzzleFlash;
        [SerializeField]
        protected bool ShowMuzzleFlash = true;

        protected float AimRadius = 1.3f;
        protected float Range = 150.0f;

        void Start()
        {
            AimTransform = CameraShaker.Instance.transform;
            Casings = FindChildByName("Casings");
            MuzzleFlash = FindChildByName("Muzzle");
        }

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
            RaycastHit hit;
            if (Physics.Raycast(from, direction, out hit, Range, WeaponLayerMask))
            {
                if (hit.collider.gameObject.layer == DamageableLayer)
                {
                    Damage dmg = Damage.CreateBulletDamage(DamageValue, direction, hit.point, hit.normal, PlayerLogic.Player.Instance.gameObject);
                    hit.collider.GetComponent<IDamageable>().ReceiveDamage(dmg);
                }
                else
                {
                    // else only generate particles

                }

                return hit.point;
            }

            return from + Range * direction;
        }

        protected override void PrimaryAttack()
        {
            // main
            Hitscan();

            // effects
            PlayPrimaryAnimation();
            PlayAudio(ShotSound);
            RecoilJump();

            // particles
            if (MuzzleFlash != null && ShowMuzzleFlash)
            {
                WeaponsParticles.Instance.EmitMuzzle(MuzzleFlash.position, MuzzleFlash.rotation);
            }
        }

        // called in animation
        public void EmitCasings()
        {
            if (Casings != null)
            {
                WeaponsParticles.Instance.EmitCasings(Casings.position, Casings.rotation, this.AmmoType, AmmoConsumption);
            }
        }

        protected void EmitTrail(Vector3 start, Vector3 end)
        {
            // TODO
        }
    }
}
