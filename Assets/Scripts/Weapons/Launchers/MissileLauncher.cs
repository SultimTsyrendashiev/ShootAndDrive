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
        protected GameObject Missile;

        private Transform missileSpawn;
        
        void Start()
        {
            missileSpawn = transform.Find("MissileSpawn");
        }

        public override void PrimaryAttack()
        {
            SpawnMissile();
            ReduceAmmo();

            PlayPrimaryAnimation();
            PlayAudio(ShotSound);
            RecoilJump();

            // WeaponsParticles.Instance.EmitMuzzle(MuzzleFlash.position, MuzzleFlash.rotation);
        }

        void SpawnMissile()
        {
            // TODO: object pool
            Instantiate(Missile, missileSpawn.position, missileSpawn.rotation);
        }
    }
}
