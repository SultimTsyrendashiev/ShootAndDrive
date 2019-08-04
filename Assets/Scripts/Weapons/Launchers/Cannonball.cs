using UnityEngine;

namespace SD.Weapons
{
    /// <summary>
    /// Sends damage on contact, but will not be exploded
    /// </summary>
    class Cannonball : Missile
    {
        const float speedEpsilon = 1;

        [SerializeField]
        string sparksName = "Sparks";
     
        void OnTriggerEnter(Collider col)
        {
            // on contact damageable take full damage
            ApplyFullDamage(col);
        }

        public override void ReceiveDamage(Damage damage)
        {
            ParticlesPool.Instance.Play(sparksName,
                damage.Type == DamageType.Bullet ? damage.Point : transform.position, Quaternion.LookRotation(
                damage.Type == DamageType.Bullet ? damage.Normal : damage.Point - transform.position));
        }
    }
}
