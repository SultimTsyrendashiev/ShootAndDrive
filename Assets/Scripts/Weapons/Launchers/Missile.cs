using System.Collections.Generic;
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
        float                       lifetime = 5;

        float                       timeToDestroy;
        bool                        exploded;

        // entity that launched this missile
        GameObject                  owner;
        // damage value, set by owner
        float                       damageValue;

        [SerializeField]
        float                       damageRadius;

        [SerializeField]
        float                       maxAngularSpeed = 100;

        public GameObject           ThisObject => gameObject;
        public PooledObjectType     Type => PooledObjectType.Important;
        public int                  AmountInPool => 4;

        protected virtual DamageType DamageType => DamageType.Explosion;

        float IDamageable.Health => 1;

        protected Rigidbody PhysicsModel { get; private set; }

        public void Init()
        {
            PhysicsModel = GetComponent<Rigidbody>();
            MissileInit();
        }

        public void Reinit()
        {
            exploded = false;
            MissileReinit();
        }

        protected virtual void MissileReinit() { }
        protected virtual void MissileInit() { }

        /// <summary>
        /// Set parameters of this missile.
        /// Must be called before launching,
        /// as it also disables collision
        /// detection with owner object
        /// </summary>
        /// <param name="owner">owner object (player, enemy, etc)</param>
        public void Set(float damageValue, GameObject owner)
        {
            this.damageValue = damageValue;
            this.owner = owner;

            // ignore collision with owner to prevent instant explosion
            IgnoreCollisionWithOwner(true);
        }

        public virtual void Launch(float missileSpeed)
        {
            PhysicsModel.velocity = transform.forward * missileSpeed;
            timeToDestroy = Time.time + lifetime;

            if (maxAngularSpeed > 0)
            {
                PhysicsModel.angularVelocity = Random.onUnitSphere * Random.Range(-maxAngularSpeed, maxAngularSpeed);
            }
        }

        //void FixedUpdate()
        //{
        //    // just make sure that missile's rotation is correct
        //    transform.forward = PhysicsModel.velocity;
        //}


        /// <summary>
        /// Explode this missile
        /// </summary>
        /// <param name="position">position of explosion</param>
        /// <param name="ignore">what collider to ignore</param>
        /// <param name="disableAfterExplosion">should game object be deactivated after explosion?</param>
        /// <param name="list"> list of all IDamageables that were wounded. Note: it will be cleared</param>
        /// <returns>damage that was sended</returns>
        protected Damage Explode(Vector3 position, Collider ignore, bool disableAfterExplosion = true, List<IDamageable> list = null)
        {
            // stop waiting for lifetime explosion
            exploded = true;

            // disable, as there can be different owners after explosion
            IgnoreCollisionWithOwner(false);

            // foreach collider apply damage
            Collider[] cs = Physics.OverlapSphere(position, damageRadius, explosionMask.value);

            list?.Clear();

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

                IDamageable d = c.GetComponent<IDamageable>();

                if (d != null)
                {
                    d.ReceiveDamage(dmg);

                    // add to list if exist
                    list?.Add(d);
                }
            }

            //if (list != null)
            //{
            //    foreach (var d in list)
            //    {
            //        d.ReceiveDamage(dmg);
            //    }
            //}

            if (ExplosionName.Length > 0)
            {
                ParticlesPool.Instance.Play(ExplosionName, position, Quaternion.identity);
            }

            if (disableAfterExplosion)
            {
                gameObject.SetActive(false);
            }

            return dmg;
        }

        /// <summary>
        /// Apply full damage to the collider
        /// (if it's 'IDamageable' and not owner)
        /// </summary>
        /// <param name="explosion">should damage type be an explosion?</param>
        protected void ApplyFullDamage(Collider c, bool explosion = true)
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
                Damage dmg = explosion ?
                    Damage.CreateExpolosionDamage(damageValue, damageRadius, c.transform.position, owner) :
                    Damage.CreateBulletDamage(damageValue, transform.forward, transform.position, -transform.forward, owner);
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

        void Update()
        {
            if (!exploded && Time.time > timeToDestroy)
            {
                Explode(transform.position, null);
            }
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
