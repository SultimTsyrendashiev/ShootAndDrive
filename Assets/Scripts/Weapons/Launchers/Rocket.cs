using UnityEngine;

namespace SD.Weapons
{
    /// <summary>
    /// Explodes on collision
    /// </summary>
    class Rocket : Missile
    {
        void OnCollisionEnter(Collision col)
        {
            // on contact damageable take full damage
            ApplyFullDamage(col.collider);

            // there is always at least one contact;
            // ignore collider that took full damage
            Explode(col.contacts[0].point, col.collider);
        }
    }
}
