using UnityEngine;

namespace SD.Enemies
{
    [CreateAssetMenu(menuName = "Enemy Data", order = 51)]
    class EnemyData : ScriptableObject
    {
        // common
        [SerializeField] string     enemyName;
        [SerializeField] int        startHealth;
        [SerializeField] int        score;
        
        public string               Name => enemyName;
        public int                  StartHealth => startHealth;
        /// <summary>
        /// How many score points will be given to player
        /// after defeating this enemy
        /// </summary>
        public int                  Score => score;


        // special, if needed
        [SerializeField] bool       isDriver = false;

        [SerializeField] bool       canAttack = false;
        [SerializeField] string     projectileName;
        [SerializeField] int        projectileDamage = 7;
        [SerializeField] float      projectileSpeed = 5.0f;

        [SerializeField] Vector2    timeBetweenRounds = new Vector2(1.0f, 1.0f);
        [SerializeField] float      fireRate = 0.1f;
        [SerializeField] int        shotsAmount = 1;

        [SerializeField] float      maxAttackDistance;
        [SerializeField] float      minAttackDistance;

        [SerializeField] string     bloodParticlesName = "Blood";
        [SerializeField] string     corpseName;

        [SerializeField] AudioClip  attackSound;
        [SerializeField] AudioClip  deathSound;
        [SerializeField] AudioClip  woundSound;

        public bool                 IsDriver => isDriver;
        public bool                 CanAttack => canAttack;
        /// <summary>
        /// Projectile name in object pool
        /// </summary>
        public string               ProjectileName => projectileName;
        /// <summary>
        /// Time between fire rounds
        /// </summary>
        public Vector2              TimeBetweenRounds => timeBetweenRounds;
        /// <summary>
        /// Time between shots in seconds
        /// </summary>
        public float                FireRate => fireRate;
        /// <summary>
        /// Amount of shots for one round
        /// </summary>
        public int                  ShotsAmount => shotsAmount;
        /// <summary>
        /// Blood particles name in particles pool
        /// </summary>
        public string               BloodParticlesName => bloodParticlesName;

        public string               CorpseName => corpseName;

        public float                ProjectileSpeed => projectileSpeed;

        public int                  ProjectileDamage => projectileDamage;

        public float                MaxAttackDistance => maxAttackDistance;
        public float                MinAttackDistance => minAttackDistance;

        public AudioClip            AttackSound => attackSound;
        public AudioClip            DeathSound => deathSound;
        public AudioClip            WoundSound => woundSound;
    }
}
