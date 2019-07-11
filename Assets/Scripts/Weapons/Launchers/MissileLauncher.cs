using System;
using System.Collections.Generic;
using UnityEngine;
using SD.ObjectPooling;
using SD.PlayerLogic;

namespace SD.Weapons
{
    /// <summary>
    /// Missile based weapons.
    /// There must be a child called "MissileSpawn".
    /// </summary>
    class MissileLauncher : Weapon
    {
        [SerializeField]
        protected string MissileName;
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
            // main
            SpawnMissile();

            // effects
            PlayPrimaryAnimation();
            PlayAudio(ShotSound);
            RecoilJump();

            // particles
            // WeaponsParticles.Instance.EmitMuzzle(MuzzleFlash.position, MuzzleFlash.rotation);
        }

        void SpawnMissile()
        {
            GameObject missileObj = ObjectPool.Instance.GetObject(MissileName, missileSpawn.position, Owner.transform.rotation);

            Missile missile = missileObj.GetComponent<Missile>();
            missile.Set(DamageValue, damageRadius, Owner);
            missile.Launch(launchSpeed);
        }
    }
}
