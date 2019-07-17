using System;
using System.Collections;
using UnityEngine;

namespace SD.Weapons
{
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    abstract class Missile : MonoBehaviour, IPooledObject, IDamageable
    {
        [SerializeField]
        LayerMask                   explosionMask;

        public string               ExplosionName = "Explosion";

        [SerializeField]
        float               lifetime = 5;

        // Entity that launched this missile
        GameObject          owner;
        Rigidbody           rb;
        float               damageValue;
        float               damageRadius;

        public GameObject           ThisObject => gameObject;
        public PooledObjectType     Type => PooledObjectType.Important;
        public int                  AmountInPool => 4;

        protected virtual DamageType DamageType => DamageType.Explosion;

        float IDamageable.Health => 1;

        public void Init()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void Reinit() { }

        /// <summary>
        /// Set parameters of this missile.
        /// Must be called before launching,
        /// as it also disables collision
        /// detection with owner object
        /// </summary>
        /// <param name="owner">owner object (player, enemy, etc)</param>
        public void Set(float damageValue, float damageRadius, GameObject owner)
        {
            this.damageValue = damageValue;
            this.damageRadius = damageRadius;
            this.owner = owner;

            // ignore collision with owner to prevent instant explosion
            IgnoreCollisionWithOwner(true);
        }

        public void Launch(float missileSpeed)
        {
            rb.velocity = transform.forward * missileSpeed;
            StartCoroutine(WaitToDisable());
        }

        void FixedUpdate()
        {
            // just make sure that missile's rotation is correct
            transform.forward = rb.velocity;
        }

        protected void Explode(Vector3 position, Collider ignore)
        {
            StopAllCoroutines();

            // disable, as there can be different owners after explosion
            IgnoreCollisionWithOwner(false);

            // foreach collider apply damage
            Collider[] cs = Physics.OverlapSphere(position, damageRadius, explosionMask.value);

            Damage dmg;
            Debug.Assert(DamageType == DamageType.Explosion || DamageType == DamageType.Fire, "Wrong damage type", this);

            if (DamageType == DamageType.Explosion)
            {
                dmg = Damage.CreateExpolosionDamage(damageValue, damageRadius, position, owner);
            }
            else
            {
                dmg = Damage.CreateFireDamage(damageValue, owner);
            }

            foreach (Collider c in cs)
            {
                if (c == ignore)
                {
                    continue;
                }

                if (IsOwner(c))
                {
                    continue;
                }

                IDamageable d = c.gameObject.GetComponent<IDamageable>();
                
                if (d != null)
                {
                    d.ReceiveDamage(dmg);
                }
            }

            if (ExplosionName.Length > 0)
            {
                ParticlesPool.Instance.Play(ExplosionName, position, Quaternion.identity);
            }

            gameObject.SetActive(false);
        }

        /// <summary>
        /// Apply full damage to the collider
        /// (if it's 'IDamageable' and not owner)
        /// </summary>
        protected void ApplyFullDamage(Collider c)
        {
            // ignore if it's owner
            if (IsOwner(c))
            {
                return;
            }

            IDamageable d = c.gameObject.GetComponent<IDamageable>();

            if (d != null)
            {
                // damage in collider's position
                Damage dmg = Damage.CreateExpolosionDamage(damageValue, damageRadius, c.transform.position, owner);
                d.ReceiveDamage(dmg);
            }
        }

        bool IsOwner(Collider c)
        {
            if (c.gameObject == owner)
            {
                return true;
            }

            var drb = c.attachedRigidbody;
            if (drb != null && drb.gameObject == owner)
            {
                return true;
            }

            return false;
        }

        IEnumerator WaitToDisable()
        {
            yield return new WaitForSeconds(lifetime);
            Explode(transform.position, null);
        }

        void IgnoreCollisionWithOwner(bool ignore)
        {
            Collider[] ownerColl = owner.GetComponentsInChildren<Collider>();
            foreach (var c in ownerColl)
            {
                Physics.IgnoreCollision(GetComponent<Collider>(), c, ignore);
            }
        }

        public virtual void ReceiveDamage(Damage damage)
        {
            // by default, if any damage occurs, explode
            Explode(transform.position, null);
        }
    }
}
