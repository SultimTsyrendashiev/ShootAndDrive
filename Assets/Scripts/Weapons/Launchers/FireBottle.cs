using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace SD.Weapons
{
    /// <summary>
    /// Explodes on contact, but will send damage for some time
    /// </summary>
    class FireBottle : Missile
    {
        /// <summary>
        /// How long fire must damage
        /// </summary>
        [SerializeField]
        float fireDamageTime = 2.0f;

        /// <summary>
        /// Rate of fire damage
        /// </summary>
        [SerializeField]
        float fireDamageRate = 0.075f;

        [SerializeField]
        string fireParticlesName = "Fire";

        protected override DamageType DamageType => DamageType.Fire;

        // components that will be disables when fire bottle explodes
        MeshRenderer[] meshRenderers;
        Collider[] colliders;

        protected override void DoInit()
        {
            meshRenderers = GetComponentsInChildren<MeshRenderer>();
            colliders = GetComponentsInChildren<Collider>();
        }

        protected override void DoReinit()
        {
            ActivateComponents(true);
        }

        void OnCollisionEnter(Collision col)
        {
            var damaged = new List<IDamageable>();

            // there is always at least one contact;
            // all damageables in radius will receive full damage;
            
            // dont deactivate gameobject itself, only renderables and colliders
            Damage dmg = Explode(col.contacts[0].point, null, false, damaged);

            ActivateComponents(false);

            if (damaged.Count > 0)
            {
                // send fire damage over time
                StartCoroutine(SendDamageOverTime(dmg, damaged));
            }
        }
        
        void ActivateComponents(bool active)
        {
            foreach (var c in meshRenderers)
            {
                c.enabled = active;
            }

            foreach (var c in colliders)
            {
                c.enabled = active;
            }

            PhysicsModel.isKinematic = !active;
        }

        IEnumerator SendDamageOverTime(Damage dmg, List<IDamageable> damaged)
        {
            float passedTime = 0;

            while (passedTime <= fireDamageTime)
            {
                foreach (var d in damaged)
                {
                    if (d.Health > 0)
                    {
                        d.ReceiveDamage(dmg);
                        ParticlesPool.Instance.Play(fireParticlesName, dmg.Point, Quaternion.LookRotation(Vector3.up));
                    }
                }

                yield return new WaitForSeconds(fireDamageRate);
                passedTime += fireDamageRate;
            }

            // disable this gameobject, i.e. return to pool
            gameObject.SetActive(false);
        }
    }
}
