using System;
using System.Collections;
using UnityEngine;

namespace SD.Weapons
{
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    class Missile : MonoBehaviour, IPooledObject
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
            transform.forward = rb.velocity;
        }

        void OnCollisionEnter(Collision col)
        {
            // there is always at least one contact
            Explode(col.contacts[0].point);
        }

        void Explode(Vector3 position)
        {
            StopAllCoroutines();
            gameObject.SetActive(false);

            // disable, as there can be different owners after explosion
            IgnoreCollisionWithOwner(false);

            // foreach collider apply damage
            Collider[] cs = Physics.OverlapSphere(position, damageRadius, explosionMask.value);
            Damage dmg = Damage.CreateExpolosionDamage(damageValue, damageRadius, position, owner);

            foreach (Collider c in cs)
            {
                float sqrLength = (position - c.transform.position).sqrMagnitude;

                IDamageable d = c.gameObject.GetComponent<IDamageable>();

                if (d != null)
                {
                    d.ReceiveDamage(dmg);
                }
            }

            ParticlesPool.Instance.Play(ExplosionName, position, Quaternion.identity);
        }

        IEnumerator WaitToDisable()
        {
            yield return new WaitForSeconds(lifetime);
            Explode(transform.position);
        }

        void IgnoreCollisionWithOwner(bool ignore)
        {
            Collider[] ownerColl = owner.GetComponentsInChildren<Collider>();
            foreach (var c in ownerColl)
            {
                Physics.IgnoreCollision(GetComponent<Collider>(), c, ignore);
            }
        }
    }
}
