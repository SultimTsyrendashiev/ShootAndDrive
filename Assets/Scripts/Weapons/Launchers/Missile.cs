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
        private float               lifetime = 5;

        private float               damageValue;
        private float               damageRadius;
        private Rigidbody           rb;
        private GameObject          owner;

        public GameObject           ThisObject => gameObject;
        public PooledObjectType     Type => PooledObjectType.Important;
        public int                  AmountInPool => 4;

        public void OnInit()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void Enable() { }

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
            Explode();
        }

        void Explode()
        {
            StopAllCoroutines();
            gameObject.SetActive(false);

            // disable, as there can be different owners after explosion
            IgnoreCollisionWithOwner(false);

            // foreach collider apply damage
            Collider[] cs = Physics.OverlapSphere(transform.position, damageRadius, explosionMask.value);
            Damage dmg = Damage.CreateExpolosionDamage(damageValue, damageRadius, transform.position, PlayerLogic.Player.Instance.gameObject);

            foreach (Collider c in cs)
            {
                float sqrLength = (transform.position - c.transform.position).sqrMagnitude;

                IDamageable d = c.gameObject.GetComponent<IDamageable>();

                if (d != null)
                {
                    d.ReceiveDamage(dmg);
                }
            }

            ParticlesPool.Instance.Play(ExplosionName, transform.position, Quaternion.identity);
        }

        IEnumerator WaitToDisable()
        {
            yield return new WaitForSeconds(lifetime);
            Explode();
        }

        void IgnoreCollisionWithOwner(bool ignore)
        {
            Collider[] ownerColl = owner.GetComponentsInChildren<Collider>();
            foreach (var c in ownerColl)
            {
                Physics.IgnoreCollision(GetComponent<Collider>(), c);
            }
        }
    }
}
