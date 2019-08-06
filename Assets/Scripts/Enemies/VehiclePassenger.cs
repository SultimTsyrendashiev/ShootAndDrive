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
        // when can attack
        const float AttackDistance = 75.0f;
        const float AttackDistanceSqr = AttackDistance * AttackDistance;

        [SerializeField]
        EnemyData               data;

        [SerializeField]
        Transform               projectileSpawn;

        [SerializeField]
        Collider                autoaimTarget;

        Coroutine               attackCoroutine;

        Collider                damageable;
        // Current vehicle of this passenger
        EnemyVehicle            vehicle;
        // Current target of this passenger
        IEnemyTarget            target;

        [SerializeField]
        // Animation for this passenger
        EnemyPassengerAnimation passengerAnimation;
        
        // When to end damaging
        float                   damageEndTime;

        PassengerState          State { get; set; }
        public float            Health { get; private set; }

        /// <summary>
        /// Called on death, sends info about this enemy
        /// </summary>
        public event PassengerDied OnPassengerDeath;

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

            TryToStartAttack();
        }

        /// <summary>
        /// Called on vehicle disable
        /// (when its state turns to 'Nothing')
        /// </summary>
        public void Deactivate()
        {
            StopAllCoroutines();
            attackCoroutine = null;

            State = PassengerState.Nothing;
        }
        #endregion

        void Update()
        {
            // if time for damage passed and was damaging
            if (damageEndTime <= 0 && State == PassengerState.Damaging)
            {
                // return to normal state
                damageEndTime = 0;
                State = PassengerState.Active;
            }
        }

        #region health
        public void ReceiveDamage(Damage damage)
        {
            if (State == PassengerState.Nothing)
            {
                Debug.Log("Wrong damageable state", this);
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
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }

            State = PassengerState.Damaging;
            Health -= damage.CalculateDamageValue(transform.position);
            
            if (Health > 0)
            {
                passengerAnimation.Damage();
                damageEndTime += TimeForDamage;
            }
            else // death
            {
                Die();
            }
        }

        /// <summary>
        /// Must be called only from 'ReceiveDamage'
        /// </summary>
        void Die()
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

            OnPassengerDeath(data);
        }

        /// <summary>
        /// Kill this passenger
        /// </summary>
        public void Kill()
        {
            if (Health <= 0)
            {
                return;
            }

            Damage fatalDamage = Damage.CreateBulletDamage(Health,
                    transform.forward, transform.position, transform.forward, gameObject);

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

            if (target == null && attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
            }

            // start if object is enabled and ready,
            // try to start attack
            if (State == PassengerState.Active)
            {
                TryToStartAttack();
            }
        }

        bool TryToStartAttack()
        {
            if (!data.CanAttack || data.ShotsAmount == 0)
            {
                return false;
            }

            if (target == null)
            {
                return false;
            }

            // if already attacking
            if (attackCoroutine != null)
            {
                return false;
            }

            // this coroutine will be disabled when
            // passenger will receive damage
            attackCoroutine = StartCoroutine(WaitForAttack());

            return true;
        }

        IEnumerator WaitForAttack()
        {
            Vector2     timeBetweenRounds = data.TimeBetweenRounds;
            int         shotsAmount = data.ShotsAmount;
            string      projectileName = data.ProjectileName;
            float       fireRate = data.FireRate;

            while (isActiveAndEnabled)
            {
                yield return new WaitForSeconds(Random.Range(timeBetweenRounds[0], timeBetweenRounds[1]));

                // must be active
                if (State != PassengerState.Active || target == null)
                {
                    attackCoroutine = null;
                    yield break;
                }

                Vector3 fromTarget = transform.position - target.Target.position;

                // don't attack if target is far away
                if (fromTarget.sqrMagnitude > AttackDistanceSqr)
                {
                    continue;
                }

                // don't attack, if behind
                if (Vector3.Dot(fromTarget, target.Target.forward) < 0)
                {
                    continue;
                }

                // play animation
                passengerAnimation.Attack();

                for (int i = 0; i < shotsAmount; i++)
                {
                    // always check for state and target
                    if (State != PassengerState.Active || target == null)
                    {
                        attackCoroutine = null;
                        yield break;
                    }

                    Vector3 direction = AimToTarget(i);

                    var missileObj = ObjectPool.Instance.GetObject(projectileName, projectileSpawn.position, direction);
                    var missile = missileObj.GetComponent<Weapons.Missile>();
                    missile.Set(data.ProjectileDamage, vehicle.gameObject);
                    missile.Launch(data.ProjectileSpeed);

                    yield return new WaitForSeconds(fireRate);
                }
            }
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

            const float maxAngle = 5.0f;
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

            // apply angle
            direction = Quaternion.AngleAxis(anglex, transform.up)
                * direction;

            return direction;
        }
        #endregion
    }
}
