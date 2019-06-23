using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SD.Player;

namespace SD.Weapons
{
    abstract class HitscanWeapon : Weapon
    {
        protected readonly int DamageableLayer = LayerMask.NameToLayer(LayerNames.Damageable);

        [SerializeField]
        protected AudioClip ShotSound;
        protected Camera PlayerCamera;

        protected float AimRadius = 2.0f;
        protected float Range = 150.0f;

        void Start()
        {
            PlayerCamera = Player.Player.Instance.MainCamera;
        }

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
    }
}
