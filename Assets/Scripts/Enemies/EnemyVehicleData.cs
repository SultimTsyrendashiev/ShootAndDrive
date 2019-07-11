﻿using UnityEngine;

namespace SD.Enemies
{
    class EnemyVehicleData : ScriptableObject
    {
        [SerializeField] string     vehicleCommonName;
        [SerializeField] string     vehicleMark;
        [SerializeField] string     vehicleModel;
        [SerializeField] string     vehicleType;
        [SerializeField] float      speed;
        [SerializeField] float      brakingTime = 2.0f;
        [SerializeField] int        startHealth;
        [SerializeField] int        score;
        [SerializeField] string     explosionName = "Explosion";
        [SerializeField] string     hitParticlesName = "Sparks";

        public string       CommonName => vehicleCommonName;
        public string       Mark => vehicleMark;
        public string       Model => vehicleModel;
        public string       Type => vehicleType;
        public float        BrakingTime => brakingTime;
        public float        Speed => speed;
        public int          StartHealth => startHealth;
        /// <summary>
        /// How many score points will be given to player
        /// after destroying this vehicle
        /// </summary>
        public int          Score => score;
        /// <summary>
        /// What particle system in particles pool to use
        /// when this vehicle will be destroyed
        /// </summary>
        public string       ExplosionName => explosionName;
        public string       HitParticlesName => hitParticlesName;
    }
}