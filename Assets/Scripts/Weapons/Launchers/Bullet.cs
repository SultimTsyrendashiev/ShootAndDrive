using UnityEngine;

namespace SD.Weapons
{
    class Bullet : Missile
    {
        void OnTriggerEnter(Collider col)
        {
            // on contact damageable take full damage
            ApplyFullDamage(col, false);
        }
    }
}
