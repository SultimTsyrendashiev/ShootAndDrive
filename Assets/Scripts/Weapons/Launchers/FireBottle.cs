using UnityEngine;

namespace SD.Weapons
{
    /// <summary>
    /// Explodes on contact, but will send damage for some time
    /// </summary>
    class FireBottle : Missile
    {
        protected override DamageType DamageType => DamageType.Fire;

        void OnCollisionEnter(Collision col)
        {
            // there is always at least one contact;
            // all damageables in radius will receive full damage
            Explode(col.contacts[0].point, null);
        }
    }
}
