using UnityEngine;
using System.Collections;

namespace SD.Enemies
{
    [RequireComponent(typeof(Collider))]
    class VehiclePassenger : MonoBehaviour, IDamageable, IAttackable
    {
        enum PassengerState
        {
            Nothing,
            Active,
            Dead,
            Damaging
        }

        // how long to wait for damage
        const float TimeForDamage = 1.0f;

        [SerializeField]
        EnemyData                   data;

        [SerializeField]
        Transform                   projectileSpawn;

        [SerializeField]
        Collider                    autoaimTarget;

        [SerializeField]
        AudioSource                 audioSource;

        Collider                    damageable;
        // Current vehicle of this passenger
        EnemyVehicle                vehicle;
        // Current target of this passenger
        IEnemyTarget                target;

        [SerializeField]
        // Animation for this passenger
        EnemyPassengerAnimation     passengerAnimation;

        [SerializeField]
        ParticleSystem              muzzleFlash;

        // When to end damaging
        float                       damageEndTime;

        // When to start attacking
        float                       attackTime;
        float                       minAttackDistanceSqr;
        float                       maxAttackDistanceSqr;
        bool                        isAttacking;

        PassengerState              State { get; set; }
        public float                Health { get; private set; }

        /// <summary>
        /// Called on death, sends info about this enemy
        /// </summary>
        public event PassengerDeath OnPassengerDeath;

        #region init
        /// <summary>
        /// Called from vehicle class to init
        /// </summary>
        public void Init(EnemyVehicle vehicle)
        {
            this.vehicle = vehicle;
            this.target = null;
            damageable = GetComponent<Collider>();
            State = PassengerState.Nothing;

            Debug.Assert(passengerAnimation != null, "Passenger animation is not set", this);
            passengerAnimation.Init(this);

            minAttackDistanceSqr = data.MinAttackDistance * data.MinAttackDistance;
            maxAttackDistanceSqr = data.MaxAttackDistance * data.MaxAttackDistance;
        }

        /// <summary>
        /// Called on respawn
        /// </summary>
        public void Reinit()
        {
            Health = data.StartHealth;
            State = PassengerState.Active;

            if (autoaimTarget != null)
            {
                autoaimTarget.enabled = true;
            }

            // reset animation
            passengerAnimation.gameObject.SetActive(true);
            passengerAnimation.Reset();

            // reset collider
            damageable.enabled = true;

            StartAttacking(false);
        }

        /// <summary>
        /// Called on vehicle disable
        /// (when its state turns to 'Nothing')
        /// </summary>
        public void Deactivate()
        {
            StopAttacking();

            State = PassengerState.Nothing;
        }
        #endregion

        void Update()
        {
            // if time for damage passed, but state still is damaging,
            // then return to normal state
            if (Time.time > damageEndTime && State == PassengerState.Damaging)
            {
                damageEndTime = 0;
                State = PassengerState.Active;
            }

            if (Time.time > attackTime && CanAttack())
            {
                StartCoroutine(Attack());
            }
        }

        #region health
        public void ReceiveDamage(Damage damage)
        {
            if (State == PassengerState.Nothing)
            {
                return;
            }

            // always play blood particle system
            ParticlesPool.Instance.Play(data.BloodParticlesName,
                damage.Type == DamageType.Bullet ? damage.Point : transform.position, Quaternion.LookRotation(
                damage.Type == DamageType.Bullet ? damage.Normal : damage.Point - transform.position));

            if (State == PassengerState.Dead)
            {
                return;
            }

            // stop attacking
            if (isAttacking)
            {
                StopAttacking();
            }

            State = PassengerState.Damaging;
            Health -= damage.CalculateDamageValue(transform.position);

            if (Health > 0)
            {
                passengerAnimation.Damage();
                damageEndTime = Time.time + TimeForDamage;
            }
            else // death
            {
                Die(damage.Initiator);
            }
        }

        /// <summary>
        /// Must be called only from 'ReceiveDamage'
        /// </summary>
        void Die(GameObject deathInitiator)
        {
            Health = 0;
            State = PassengerState.Dead;

            // disable autoaim target
            if (autoaimTarget != null)
            {
                autoaimTarget.enabled = false;
            }

            if (string.IsNullOrEmpty(data.CorpseName))
            {
                // play animation if there is no corpse
                passengerAnimation.Die();
            }
            else
            {
                // hide animated passenger
                passengerAnimation.gameObject.SetActive(false);
                // deactivate damageable
                damageable.enabled = false;

                // get corpse from object pool
                var co = ObjectPool.Instance.GetObject(data.CorpseName, transform.position, transform.rotation);

                var c = co.GetComponent<Corpse>();
                Debug.Assert(c != null, "Corpse object must contain corpse component", co);

                Vector3 av = Random.onUnitSphere * Random.Range(10, 40);
                c.Launch(vehicle.VehicleRigidbody.velocity, av);
            }

            OnPassengerDeath(data, autoaimTarget.transform, deathInitiator);
        }

        /// <summary>
        /// Kill this passenger
        /// </summary>
        public void Kill(GameObject initiator)
        {
            if (Health <= 0)
            {
                return;
            }

            Damage fatalDamage = Damage.CreateBulletDamage(Health,
                    transform.forward, transform.position, transform.forward, initiator);

            ReceiveDamage(fatalDamage);
        }
        #endregion

        #region attack
        /// <summary>
        /// Set target for this passenger.
        /// If target is null, passenger will stop attacking
        /// </summary>
        public void SetTarget(IEnemyTarget target)
        {
            this.target = target;

            // if forced to stop
            if (target == null && isAttacking)
            {
                StopAttacking();
                return;
            }

            // start if object is enabled and ready,
            // try to start attack
            if (State == PassengerState.Active && !isAttacking)
            {
                StartAttacking(true);
            }
        }

        bool CanAttack()
        {
            if (isAttacking)
            {
                return false;
            }

            if (!data.CanAttack || data.ShotsAmount == 0)
            {
                return false;
            }

            // must be active
            if (State != PassengerState.Active || target == null)
            {
                return false;
            }

            Vector3 fromTarget = transform.position - target.Target.position;
            float sqrDist = fromTarget.sqrMagnitude;

            // if out
            if (sqrDist > maxAttackDistanceSqr ||
                sqrDist < minAttackDistanceSqr)
            {
                return false;
            }

            // don't attack, if behind
            if (Vector3.Dot(fromTarget, target.Target.forward) < 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Start attacking, set next attack time
        /// </summary>
        IEnumerator Attack()
        {
            isAttacking = true;

            int         shotsAmount = data.ShotsAmount;
            float       fireRate = data.FireRate;

            // play animation
            passengerAnimation.Attack();

            for (int i = 0; i < shotsAmount; i++)
            {
                // always check for state and target
                if (State != PassengerState.Active || target == null || !isAttacking)
                {
                    yield break;
                }

                LaunchMissile(i);
                muzzleFlash?.Play();

                PlayAttackSound();

                yield return new WaitForSeconds(fireRate);
            }

            isAttacking = false;
            StartAttacking(false);
        }

        void StartAttacking(bool instant)
        {
            if (!instant)
            {
                // set for next time
                attackTime = Time.time + Random.Range(data.TimeBetweenRounds[0], data.TimeBetweenRounds[1]);
            }
            else
            {
                attackTime = Time.time;
            }
        }

        void LaunchMissile(int index)
        {
            Vector3 direction = AimToTarget(index);

            var missileObj = ObjectPool.Instance.GetObject( data.ProjectileName, projectileSpawn.position, direction);
            var missile = missileObj.GetComponent<Weapons.Missile>();
            missile.Set(data.ProjectileDamage, vehicle.gameObject);
            missile.Launch(data.ProjectileSpeed);
        }

        void PlayAttackSound()
        {
            audioSource?.PlayOneShot(data.AttackSound);
        }

        void StopAttacking()
        {
            isAttacking = false;
            StopAllCoroutines();
        }

        Vector3 AimToTarget(int shotIndex)
        {
            if (target == null)
            {
                return transform.forward;
            }

            Vector3 direction = target.Target.position - projectileSpawn.position;
            direction.x = 0;

            direction.Normalize();

            const float maxAngle = 0.3f;
            int amount = data.ShotsAmount; // >= 1

            if (amount == 1)
            {
                return direction;
            }

            float delta = 1.0f / (amount - 1);
            // [0..1]
            float anglex = shotIndex * delta;
            // [-1..1]
            anglex = -1 + anglex * 2;

            anglex *= maxAngle;

            // half of shots must be straight to target
            if (shotIndex < data.ShotsAmount / 2)
            {
                // apply angle
                direction = Quaternion.AngleAxis(anglex, transform.up)
                    * direction;
            }
            else
            {
                direction = Quaternion.AngleAxis(Random.Range(-0.5f, 2.0f), transform.right)
                    * direction;
            }

            return direction;
        }
        #endregion
    }
}
