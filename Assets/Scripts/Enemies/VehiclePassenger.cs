using UnityEngine;
using System.Collections;

namespace SD.Enemies
{
    enum PassengerState
    {
        Nothing,
        Alive,
        Attacking,
        Dead,
        Damaging
    }

    [RequireComponent(typeof(Collider))]
    class VehiclePassenger : MonoBehaviour, IDamageable
    {
        [SerializeField]
        bool canAttack;
        [SerializeField]
        float fireRateInSeconds;
        [SerializeField]
        int shotsAmount = 1;
        // TODO: object pool
        [SerializeField]
        GameObject projectile;
        [SerializeField]
        Transform projectileSpawn;

        EnemyVehicle vehicle;
        [SerializeField]
        Animator passengerAnimator;

        // TODO: object pool
        [SerializeField]
        ParticleSystem blood;

        public event PassengerDied OnPassengerDeath;

        [SerializeField]
        bool isDriver;
        public bool IsDriver => isDriver;

        public PassengerState State { get; private set; }

        float startHealth;
        public float Health { get; private set; }

        public void Init(int startHealth)
        {
            this.startHealth = this.Health = startHealth;
            this.vehicle = GetComponentInParent<EnemyVehicle>();
        }

        public void Reinit()
        {
            Health = startHealth;
            State = PassengerState.Alive;
        }

        public void ReceiveDamage(Damage damage)
        {
            if (State == PassengerState.Attacking)
            {
                // stop attacking
                StopAllCoroutines();
            }

            State = PassengerState.Damaging;
            Health -= damage.CalculateDamageValue(transform.position);

            // TODO: object pool
            blood.transform.position = damage.Point;
            blood.transform.rotation = Quaternion.LookRotation(damage.Type == DamageType.Bullet ? damage.Normal : Vector3.up);

            if (Health > 0)
            {
                passengerAnimator.ResetTrigger("Attack");
                passengerAnimator.SetTrigger("Damage");
            }
            else
            {
                passengerAnimator.ResetTrigger("Attack");
                passengerAnimator.ResetTrigger("Damage");
                passengerAnimator.SetTrigger("Die");
                OnPassengerDeath(isDriver);
            }
        }

        public void StartAttack(Transform target)
        {
            if (!canAttack)
            {
                return;
            }

            if (Health <= 0)
            {
                return;
            }

            passengerAnimator.SetTrigger("Attack");

            StartCoroutine(WaitForAttack(target));
        }

        IEnumerator WaitForAttack(Transform target)
        {
            for (int i = 0; i < shotsAmount; i++)
            {
                Vector3 direction = target.position - projectileSpawn.position;
                direction.Normalize();

                // TODO: object pool
                Instantiate(projectile, projectileSpawn.position, Quaternion.LookRotation(direction));

                yield return new WaitForSeconds(fireRateInSeconds);
            }
        }
    }
}
