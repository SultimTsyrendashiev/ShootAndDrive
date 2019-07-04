using UnityEngine;

namespace SD.Enemies
{
    delegate void VehicleDeath();

    [RequireComponent(typeof(Collider))]
    class EnemyVehicleDamageReceiver : MonoBehaviour, IDamageable
    {
        // TODO: object pool
        [SerializeField]
        ParticleSystem sparks;

        public event VehicleDeath OnVehicleDeath;
        public float Health { get; private set; }
        float startHealth;

        public void Init(int startHealth)
        {
            this.startHealth = startHealth;
        }

        /// <summary>
        /// Restores vehicle's health
        /// </summary>
        public void Reinit()
        {
            Health = startHealth;
        }

        public void ReceiveDamage(Damage damage)
        {
            Health -= damage.CalculateDamageValue(transform.position);

            if (damage.Type == DamageType.Bullet)
            {
                sparks.transform.position = damage.Point;
                sparks.transform.rotation = Quaternion.LookRotation(damage.Normal);
            }

            if (Health <= 0)
            {
                OnVehicleDeath();
            }
        }
    }
}
