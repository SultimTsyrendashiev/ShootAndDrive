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
    class VehiclePassenger : MonoBehaviour, IDamageable, IAttackable
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

        public PassengerState State { get; private set; }
        public float Health { get; private set; }

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

            TryToStartAttack();
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
            if (State == PassengerState.Nothing)
            {
                Debug.Log("Wrong damageable state", this);
                return;
            }

            // always play blood particle system
            ParticlesPool.Instance.Play(data.BloodParticlesName, damage.Point,
               Quaternion.LookRotation(damage.Type == DamageType.Bullet ? damage.Normal : Vector3.up));

            if (State == PassengerState.Dead)
            {
                return;
            }

            // stop attacking
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                //passengerAnimator.ResetTrigger("Attack");
            }

            State = PassengerState.Damaging;
            Health -= damage.CalculateDamageValue(transform.position);
            
            if (Health > 0)
            {
                // don't play damaging animation more than once
                if (State == PassengerState.Damaging)
                {
                    return;
                }

                //passengerAnimator.SetTrigger("Damage");

                // TODO: wait

                // return state and start attacking 
                // after receiving damage
                State = PassengerState.Active;
                TryToStartAttack();
            }
            else // death
            {
                Health = 0;
                State = PassengerState.Dead;

                // disable autoaim target
                Collider[] cs = GetComponentsInChildren<Collider>(false);
                foreach(var c in cs)
                {
                    if (c.gameObject.layer == LayerMask.NameToLayer(LayerNames.AutoaimTargets))
                    {
                        c.enabled = false;
                    }
                }

                //passengerAnimator.ResetTrigger("Damage");
                //passengerAnimator.SetTrigger("Die");

                OnPassengerDeath(data);
            }
        }

        bool TryToStartAttack()
        {
            if (!data.CanAttack)
            {
                return false;
            }

            if (target == null)
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
            Vector2   timeBetweenRounds = data.TimeBetweenRounds;
            int     shotsAmount = data.ShotsAmount;
            string  projectileName = data.ProjectileName;
            float   fireRate = data.FireRate;

            while (isActiveAndEnabled)
            {
                yield return new WaitForSeconds(Random.Range(timeBetweenRounds[0], timeBetweenRounds[1]));
             
                // must be active
                if (State != PassengerState.Active && target != null)
                {
                    yield break;
                }

                State = PassengerState.Attacking;
                //passengerAnimator.SetTrigger("Attack");

                for (int i = 0; i < shotsAmount; i++)
                {
                    // always check for state and target
                    if (State != PassengerState.Attacking && target != null)
                    {
                        yield break;
                    }

                    Vector3 direction = AimToTarget(i);

                    ObjectPool.Instance.GetObject(projectileName, projectileSpawn.position, direction);

                    yield return new WaitForSeconds(fireRate);
                }

                // return to previous state
                State = PassengerState.Active;
            }
        }

        Vector3 AimToTarget(int shotIndex)
        {
            Debug.Assert(target != null, "This method must not be called when target is null", this);

            //Vector3 direction = target.position - projectileSpawn.position;

            Vector3 direction = target.position - projectileSpawn.position;
            direction.Normalize();

            const float angleBetweenShots = 2.5f;
            int amount = data.ShotsAmount - 1;

            // [0..1]
            float anglex = (float)shotIndex / amount;
            // [-1..1]
            anglex = anglex * 2 - 1;

            anglex *= angleBetweenShots;

            // apply angle
            direction = Quaternion.AngleAxis(anglex, transform.up)
                * direction;

            return direction;
        }

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

        /// <summary>
        /// Set target for this passenger.
        /// If target is null, 
        /// </summary>
        public void SetTarget(Transform target)
        {
            this.target = target;

            // start if object is enabled and ready,
            // try to start attack
            if (State == PassengerState.Active)
            {
                TryToStartAttack();
            }
        }
    }
}
