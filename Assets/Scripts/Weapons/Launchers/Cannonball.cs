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
            if (damage.Type == DamageType.Bullet)
            {
                // just generate particles
                ParticlesPool.Instance.Play(sparksName, damage.Point, Quaternion.LookRotation(damage.Normal));
            }
        }
    }
}
