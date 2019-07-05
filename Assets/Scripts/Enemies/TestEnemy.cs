using UnityEngine;

namespace SD.Enemies
{
    class TestEnemy : MonoBehaviour, IEnemy, IDamageable
    {
        public float Health => 10;

        public void ReceiveDamage(Damage damage)
        {
            string particlesName;

            if (name.Contains("Enemy"))
            {
                particlesName = "Blood";
            }
            else
            {
                particlesName = "Sparks";
            }

            Quaternion rotation= damage.Type == DamageType.Bullet?
                Quaternion.LookRotation(damage.Normal):
                Quaternion.LookRotation(Vector3.up);

            ParticlesPool.Instance.Play(particlesName, damage.Point, rotation);
        }
    }
}
