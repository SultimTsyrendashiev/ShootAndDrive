using UnityEngine;

namespace SD.Enemies
{
    class TestEnemy : MonoBehaviour, IEnemy, IDamageable
    {
        public float Health => 10;

        [SerializeField]
        private ParticleSystem particles;

        public void ReceiveDamage(Damage damage)
        {
            if (damage.Type == DamageType.Bullet)
            {
                particles.transform.position = damage.Point;
                particles.transform.rotation = Quaternion.LookRotation(damage.Normal);

                particles.Emit(15);
            }
        }
    }
}
