using System;
using System.Collections;
using UnityEngine;

namespace SD.Weapons
{
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    class Missile : MonoBehaviour
    {
        [SerializeField]
        LayerMask                   explosionMask;

        // TODO: object pool
        [SerializeField]
        private GameObject          explosion; // prefab
        private GameObject          explosionObj; // scene object
        private ParticleSystem[]    explosionParticles;

        [SerializeField]
        private float               lifetime = 5;

        private float               damageValue;
        private float               damageRadius;
        private Rigidbody           rb;
        private GameObject          owner;          /// owner entity (player, enemy, etc)

        public void Init(float damageValue, float damageRadius, GameObject owner)
        {
            this.damageValue = damageValue;
            this.damageRadius = damageRadius;
            this.owner = owner;

            // ignore collision with owner to prevent instant explosion
            Collider ownerColl = owner.GetComponent<Collider>();
            if (ownerColl != null)
            {
                Physics.IgnoreCollision(GetComponent<Collider>(), ownerColl);
            }

            rb = GetComponent<Rigidbody>();

            explosionObj = Instantiate(explosion);
            explosionParticles = explosionObj.GetComponentsInChildren<ParticleSystem>();
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
            gameObject.SetActive(false);

            Collider[] cs = Physics.OverlapSphere(transform.position, damageRadius, explosionMask.value);
            Damage dmg = Damage.CreateExpolosionDamage(damageValue, damageRadius, transform.position, Player.Player.Instance.gameObject);

            foreach (Collider c in cs)
            {
                float sqrLength = (transform.position - c.transform.position).sqrMagnitude;

                IDamageable d = c.gameObject.GetComponent<IDamageable>();

                if (d != null)
                {
                    d.ReceiveDamage(dmg);
                }
            }

            explosionObj.transform.position = transform.position;
            foreach (var p in explosionParticles)
            {
                p.Play();
            }
        }

        IEnumerator WaitToDisable()
        {
            yield return new WaitForSeconds(lifetime);
            Explode();
        }
    }
}
