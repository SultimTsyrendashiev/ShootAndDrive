using UnityEngine;

namespace SD.Enemies
{
    [RequireComponent(typeof(Collider))]
    class VehiclePassenger : MonoBehaviour, IDamageable
    {
        [SerializeField]
        bool isDriver;
        float startHealth;

        EnemyVehicle vehicle;
        [SerializeField]
        Animation passengerAnimation;

        // TODO: object pool
        [SerializeField]
        ParticleSystem blood;

        public event PassengerDied OnPassengerDeath;
        public bool IsDriver => isDriver;
        public float Health { get; private set; }

        public void Init(int startHealth)
        {
            this.startHealth = this.Health = startHealth;
            this.vehicle = GetComponentInParent<EnemyVehicle>();
        }

        public void Reinit()
        {
            Health = startHealth;
        }

        public void ReceiveDamage(Damage damage)
        {
            Health -= damage.CalculateDamageValue(transform.position);

            // TODO: object pool
            blood.transform.position = damage.Point;
            blood.transform.rotation = Quaternion.LookRotation(damage.Type == DamageType.Bullet ? damage.Normal : Vector3.up);

            if (Health <= 0)
            {
                OnPassengerDeath(isDriver);
            }
        }
    }
}
