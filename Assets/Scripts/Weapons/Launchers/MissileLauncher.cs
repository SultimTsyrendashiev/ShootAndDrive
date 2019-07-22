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
        private float launchSpeed;

        [SerializeField]
        private bool needsAutoaim = false;

        private Transform missileSpawn;

        protected float AutoaimRadius = 3;
        protected float AutoaimRange = 150;

        void Start()
        {
            missileSpawn = FindChildByName("MissileSpawn");
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
            missileSpawn.localEulerAngles += new Vector3(-30,0,0);

            float speed = launchSpeed;

            // find target
            Transform target = Autoaim.GetTarget(missileSpawn.position, Owner.transform.forward, AutoaimRadius, AutoaimRange, AutoaimLayerMask);
            Vector3 targetPos;

            if (target != null)
            {
                // target found
                targetPos = target.position;

                if (needsAutoaim)
                {
                    // rotate spawn transform to aim,
                    // and get speed
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
