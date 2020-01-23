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
        float launchSpeed;

        [SerializeField]
        bool calculateTrajectory = false;

        [SerializeField]
        bool onlyForward = false;

        Transform missileSpawn;
        Vector3 spawnStartEuler;

        protected float AutoaimRadius = 3;
        protected float AutoaimRange = 150;

        const float AutoaimThreshold = 2; // in meters

        void Start()
        {
            missileSpawn = FindChildByName("MissileSpawn");
            Debug.Assert(missileSpawn != null, "Launcher must contain child with name: " + "MissileSpawn");

            spawnStartEuler = missileSpawn.localEulerAngles;
        }

        protected override void PrimaryAttack()
        {
            // main
            SpawnMissile();
            ReduceAmmo();

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
            missileSpawn.localEulerAngles += spawnStartEuler;

            float speed = launchSpeed;

            Transform target = null;

            if (!onlyForward)
            {
                // find target
                target = Autoaim.GetTarget(missileSpawn.position + Owner.transform.forward * AutoaimThreshold, 
                    Owner.transform.forward, AutoaimRadius, AutoaimRange, AutoaimLayerMask);
            }

            if (target != null)
            {
                // target found
                Vector3 targetPos = target.position;

                if (calculateTrajectory)
                {
                    // rotate spawn transform and get missile speed for aiming
                    Autoaim.AimMissile(missileSpawn, targetPos, launchSpeed, out speed);
                }
                else
                {
                    // just rotate
                    missileSpawn.forward = targetPos - missileSpawn.position;
                }
            }
            // if cant find target, so launch at max speed
            // in default direction

            GameObject missileObj = ObjectPool.Instance.GetObject(Data.MissileName, missileSpawn.position, missileSpawn.rotation);

            Missile missile = missileObj.GetComponent<Missile>();
            missile.Set(DamageValue, Owner);
            missile.Launch(speed);
        }
    }
}
