using UnityEngine;

namespace SD.Enemies
{
    delegate void VehicleDeath();

    [RequireComponent(typeof(Collider))]
    class EnemyVehicleDamageReceiver : MonoBehaviour, IDamageable
    {
        public string HitParticleName = "Sparks";

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
                ParticlesPool.Instance.Play(HitParticleName, damage.Point, Quaternion.LookRotation(damage.Normal));
            }
            else if (damage.Type == DamageType.Explosion && Health > 0)
            {
                // TODO: change mesh to wreck
            }

            if (Health <= 0)
            {
                OnVehicleDeath();
            }
        }
    }
}
