using UnityEngine;

namespace SD.Weapons
{
    class Bullet : Missile
    {
        [SerializeField]
        string particlesName = "Sparks";

        void OnTriggerEnter(Collider col)
        {
            // on contact damageable take full damage
            ApplyFullDamage(col, false);
            Deactivate();
        }

        public override void ReceiveDamage(Damage damage)
        {
            ParticlesPool.Instance.Play(particlesName, transform.position, transform.rotation);
            Deactivate();
        }
    }
}
