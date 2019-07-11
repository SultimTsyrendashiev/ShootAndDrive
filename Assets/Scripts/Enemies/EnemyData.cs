using UnityEngine;

namespace SD.Enemies
{
    class EnemyData : ScriptableObject
    {
        // common
        [SerializeField] string     enemyName;
        [SerializeField] int        startHealth;
        [SerializeField] int        score;
        
        public string       Name => enemyName;
        public int          StartHealth => startHealth;
        /// <summary>
        /// How many score points will be given to player
        /// after defeating this enemy
        /// </summary>
        public int          Score => score;


        // special, if needed
        [SerializeField] bool       isDriver = false;
        [SerializeField] bool       canAttack = false;
        [SerializeField] string     projectileName;
        [SerializeField] float      timeBetweenRounds = 1.0f;
        [SerializeField] float      fireRate = 0.1f;
        [SerializeField] int        shotsAmount = 1;
        [SerializeField] string     bloodParticlesName = "Blood";

        public bool         IsDriver => isDriver;
        public bool         CanAttack => canAttack;
        /// <summary>
        /// Projectile name in object pool
        /// </summary>
        public string       ProjectileName => projectileName;
        /// <summary>
        /// Time between fire rounds
        /// </summary>
        public float        TimeBetweenRounds => timeBetweenRounds;
        /// <summary>
        /// Time between shots in seconds
        /// </summary>
        public float        FireRate => fireRate;
        /// <summary>
        /// Amount of shots for one round
        /// </summary>
        public int          ShotsAmount => shotsAmount;
        /// <summary>
        /// Blood particles name in particles pool
        /// </summary>
        public string       BloodParticlesName => bloodParticlesName;
    }
}
