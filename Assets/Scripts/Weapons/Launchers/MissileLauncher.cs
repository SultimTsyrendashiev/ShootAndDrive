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

        protected float AutoaimRadius = 3;
        protected float AutoaimRange = 150;

        void Start()
        {
            missileSpawn = FindChildByName("MissileSpawn");
            Debug.Assert(damageRadius > 0.0f, "Radius > 0", this);
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
            // reset spawn transform's rotation,
            // set max speed
            missileSpawn.rotation = Owner.transform.rotation;
            float speed = launchSpeed;

            // find target
            Transform target = Autoaim.GetTarget(missileSpawn.position, Owner.transform.forward, AutoaimRadius, AutoaimRange, AutoaimLayerMask);
            Vector3 targetPos;

            if (target != null)
            {
                // target found
                targetPos = target.position;

                // rotate spawn transform to aim,

                Autoaim.AimMissile(missileSpawn, targetPos, launchSpeed, out speed);

            }
            // if cant find target, so launch at max speed
            // in default direction

            GameObject missileObj = ObjectPool.Instance.GetObject(MissileName, missileSpawn.position, missileSpawn.rotation);

            Missile missile = missileObj.GetComponent<Missile>();
            missile.Set(DamageValue, damageRadius, Owner);
            missile.Launch(speed);
        }
    }
}
