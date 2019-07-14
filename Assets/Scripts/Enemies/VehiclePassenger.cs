using UnityEngine;
using System.Collections;

namespace SD.Enemies
{
    enum PassengerState
    {
        Nothing,
        Active,
        Attacking,
        Dead,
        Damaging
    }

    [RequireComponent(typeof(Collider))]
    class VehiclePassenger : MonoBehaviour, IDamageable
    {
        [SerializeField]
        EnemyData               data;

        [SerializeField]
        Transform               projectileSpawn;
        [SerializeField]
        Animator                passengerAnimator;

        Coroutine               attackCoroutine;

        // Current vehicle of this passenger
        EnemyVehicle            vehicle;

        // Current target of this passenger
        Transform target;

        public PassengerState   State { get; private set; }
        public float            Health { get; private set; }

        /// <summary>
        /// Called on death, sends info about this enemy
        /// </summary>
        public event PassengerDied OnPassengerDeath;

        /// <summary>
        /// Called from vehicle class to init
        /// </summary>
        public void Init(EnemyVehicle vehicle)
        {
            this.vehicle = vehicle;
            this.target = null;
            State = PassengerState.Nothing;
        }

        /// <summary>
        /// Called on respawn
        /// </summary>
        public void Reinit()
        {
            Health = data.StartHealth;
            State = PassengerState.Active;

            if (target != null)
            {
                StartAttack();
            }
        }

        /// <summary>
        /// Called on vehicle disable
        /// (when its state turns to 'Nothing')
        /// </summary>
        public void Deactivate()
        {
            StopAllCoroutines();
            State = PassengerState.Nothing;
        }

        public void ReceiveDamage(Damage damage)
        {
            if (State == PassengerState.Dead)
            {
                return;
            }

            if (Health <= 0 || State == PassengerState.Nothing)
            {
                Debug.Log("Wrone damageable state", this);
            }

            // stop attacking
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                passengerAnimator.ResetTrigger("Attack");
            }

            State = PassengerState.Damaging;
            Health -= damage.CalculateDamageValue(transform.position);

            // play blood particle system
            ParticlesPool.Instance.Play(data.BloodParticlesName, damage.Point,
               Quaternion.LookRotation(damage.Type == DamageType.Bullet ? damage.Normal : Vector3.up));
            
            if (Health > 0)
            {
                // don't play damaging animation more than once
                if (State == PassengerState.Damaging)
                {
                    return;
                }

                passengerAnimator.SetTrigger("Damage");

                // TODO: wait

                // return state and start attacking 
                // after receiving damage
                State = PassengerState.Active;
                StartAttack();
            }
            else // death
            {
                Health = 0;
                State = PassengerState.Dead;

                passengerAnimator.ResetTrigger("Damage");
                passengerAnimator.SetTrigger("Die");

                OnPassengerDeath(data);
            }
        }

        void StartAttack()
        {
            if (!data.CanAttack)
            {
                return;
            }

            if (target == null)
            {
                return;
            }

            // this coroutine will be disabled when
            // passenger will receive damage
            attackCoroutine = StartCoroutine(WaitForAttack());
        }

        IEnumerator WaitForAttack()
        {
            Vector2   timeBetweenRounds = data.TimeBetweenRounds;
            int     shotsAmount = data.ShotsAmount;
            string  projectileName = data.ProjectileName;
            float   fireRate = data.FireRate;

            while (isActiveAndEnabled)
            {
                yield return new WaitForSeconds(Random.Range(timeBetweenRounds[0], timeBetweenRounds[1]));
             
                // must be active
                if (State != PassengerState.Active)
                {
                    yield break;
                }

                State = PassengerState.Attacking;
                passengerAnimator.SetTrigger("Attack");

                for (int i = 0; i < shotsAmount; i++)
                {
                    Vector3 direction = target.position - projectileSpawn.position;
                    direction.Normalize();

                    ObjectPool.Instance.GetObject(projectileName, projectileSpawn.position, direction);

                    yield return new WaitForSeconds(fireRate);
                }

                // return to previous state
                State = PassengerState.Active;
            }
        }

        public void Kill()
        {
            Damage fatalDamage = Damage.CreateBulletDamage(Health,
                    transform.forward, transform.position, transform.forward, gameObject);

            ReceiveDamage(fatalDamage);
        }

        internal void SetTarget(Transform target)
        {
            this.target = target;

            // start if object is enabled and ready
            if (State == PassengerState.Active)
            {
                StartAttack();
            }
        }
    }
}
