using UnityEngine;

namespace SD.Enemies
{
    delegate void VehicleDeath();

    [RequireComponent(typeof(Collider))]
    class EnemyVehicleDamageReceiver : MonoBehaviour, IDamageable
    {
        EnemyVehicle vehicle;
        string hitParticlesName;
        float startHealth;

        public event VehicleDeath OnVechicleDeath;
        public float Health { get; private set; }

        public void Init(EnemyVehicle vehicle)
        {
            this.vehicle = vehicle;

            startHealth = vehicle.Data.StartHealth;
            hitParticlesName = vehicle.Data.HitParticlesName;
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
                ParticlesPool.Instance.Play(hitParticlesName, damage.Point, Quaternion.LookRotation(damage.Normal));
            }
            else if (damage.Type == DamageType.Explosion && Health > 0)
            {
                // TODO: change mesh to wreck
            }

            if (Health <= 0)
            {
                OnVechicleDeath();
            }
        }
    }
}
