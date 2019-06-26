using System;
using System.Collections.Generic;
using UnityEngine;

namespace SD.Weapons
{
    /// <summary>
    /// Missile based weapons.
    /// There must be a child called "MissileSpawn".
    /// </summary>
    class MissileLauncher : Weapon
    {
        [SerializeField]
        protected Missile Missile;
        [SerializeField]
        private float damageRadius;
        [SerializeField]
        private float launchSpeed;

        private Transform missileSpawn;
        
        void Start()
        {
            missileSpawn = FindChildByName("MissileSpawn");
            Debug.Assert(damageRadius > 0.0f);
        }

        protected override void PrimaryAttack()
        {
            SpawnMissile();

            PlayPrimaryAnimation();
            PlayAudio(ShotSound);
            RecoilJump();

            // WeaponsParticles.Instance.EmitMuzzle(MuzzleFlash.position, MuzzleFlash.rotation);
        }

        void SpawnMissile()
        {
            // TODO: object pool
            GameObject missileObj = Instantiate(this.Missile.gameObject, missileSpawn.position, Player.Player.Instance.transform.rotation);

            Missile missile = missileObj.GetComponent<Missile>();
            missile.Init(DamageValue, damageRadius, Player.Player.Instance.gameObject);
            missile.Launch(launchSpeed);
        }
    }
}
