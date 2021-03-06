﻿//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using SD.PlayerLogic;

//namespace SD.Weapons
//{
//    class WeaponsParticles : MonoBehaviour
//    {
//        [SerializeField]
//        private ParticleSystem casingsPistol;
//        [SerializeField]
//        private ParticleSystem casingsBullet;
//        [SerializeField]
//        private ParticleSystem casingsHeavyPart;
//        [SerializeField]
//        private ParticleSystem casingsShell;
//        [SerializeField]
//        private ParticleSystem casingsGrenade;
//        [SerializeField]
//        private ParticleSystem muzzleFlash;

//        private static WeaponsParticles instance;
//        public static WeaponsParticles Instance { get { return instance; } }

//        private void Start()
//        {
//            Debug.Assert(instance == null, "Several weapons particles constrollers");

//            instance = this;

//            SetSimulationSpace(casingsPistol);
//            SetSimulationSpace(casingsBullet);
//            SetSimulationSpace(casingsHeavyPart);
//            SetSimulationSpace(casingsShell);
//            SetSimulationSpace(casingsGrenade);
//            SetSimulationSpace(muzzleFlash);
//        }

//        /// <summary>
//        /// Set player's simulation space for particle system
//        /// </summary>
//        private void SetSimulationSpace(ParticleSystem system)
//        {
//            Debug.Assert(system != null);

//            var main = system.main;
//            main.simulationSpace = ParticleSystemSimulationSpace.Custom;
//            main.customSimulationSpace = Player.Instance.transform;
//        }

//        public void EmitCasings(Vector3 position, Quaternion rotation, AmmunitionType type, int amount = 1)
//        {
//            // particle system to emit
//            ParticleSystem system = null;

//            if (type == AmmunitionType.BulletsHeavy)                
//            {
//                casingsHeavyPart.transform.position = position;
//                casingsHeavyPart.transform.rotation = rotation;
//                casingsHeavyPart.Emit(amount);

//                system = casingsBullet;
//            }
//            else if (type == AmmunitionType.Bullets)
//            {
//                system = casingsBullet;
//            }
//            else if (type == AmmunitionType.BulletsPistol)
//            {
//                system = casingsPistol;
//            }
//            else if (type == AmmunitionType.Shells)
//            {
//                system = casingsShell;
//            }
//            else if (type == AmmunitionType.Grenades)
//            {
//                system = casingsGrenade;
//            }

//            if (system != null)
//            {
//                system.transform.position = position;
//                system.transform.rotation = rotation;
//                system.Emit(amount);
//            }
//        }

//        public void EmitMuzzle(Vector3 position, Quaternion rotation)
//        {
//            muzzleFlash.transform.position = position;
//            muzzleFlash.transform.rotation = rotation;
//            muzzleFlash.Play();
//        }
//    }
//}
