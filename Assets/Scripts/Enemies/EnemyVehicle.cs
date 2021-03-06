﻿using UnityEngine;

namespace SD.Enemies
{
    /// <summary>
    /// Represents enemy as vehicle with passengers.
    /// There must be vehicle damage receiver as a child object,
    /// also passengers, if needed
    /// </summary>
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    abstract class EnemyVehicle : MonoBehaviour, IVehicle, IEnemy, IDamageable
    {
        [SerializeField]
        EnemyVehicleData data;
        public EnemyVehicleData Data => data;

        EnemyVehicleDamageReceiver[] damageReceivers;
        int alivePassengersAmount;

        // approximate vechicle collider
        Collider apxVehicleCollider;

        public EnemyVehicleState        State       { get; private set; }
        protected VehiclePassenger[]    Passengers  { get; private set; }
        public bool                     AliveDriver => State != EnemyVehicleState.DeadDriver && alivePassengersAmount > 0;
        public Rigidbody                VehicleRigidbody { get; private set; }
        public float                    Health { get; private set; }

        // for object pool
        GameObject          IPooledObject.ThisObject    => gameObject;
        PooledObjectType    IPooledObject.Type          => PooledObjectType.Important;
        int                 IPooledObject.AmountInPool  => 8;

        // events
        public static event EnemyDeath          OnEnemyDeath;
        public static event VehicleDestroyed    OnVehicleDestroy;

        #region virtual
        /// <summary>
        /// Called on initializing.
        /// Override this method for special enemy init
        /// </summary>
        protected virtual void InitEnemy() { }
        /// <summary>
        /// Called on activating (reinitting)
        /// </summary>
        protected virtual void Activate() { }
        /// <summary>
        /// Called on deactivating
        /// </summary>
        protected virtual void Deactivate() { }

        /// <summary>
        /// Called on death of all passengers
        /// </summary>
        protected virtual void DoPassengerDeath() { }
        
        /// <summary>
        /// Called on death of driver
        /// </summary>
        protected virtual void DoDriverDeath() { }

        /// <summary>
        /// Called on vehicle collision
        /// </summary>
        protected virtual void DoVehicleCollision(GameObject initiator) { }
        #endregion

        public void Init()
        {
            VehicleRigidbody = GetComponent<Rigidbody>();

            // get all passengers and init them
            Passengers = GetComponentsInChildren<VehiclePassenger>(true);
            foreach (var p in Passengers)
            {
                p.Init(this);
                p.OnPassengerDeath += PassengerDied;
            }

            // get vehicle collision model and init it
            damageReceivers = GetComponentsInChildren<EnemyVehicleDamageReceiver>(true);
            foreach (var d in damageReceivers)
            {
                d.Init(this);
            }

            apxVehicleCollider = GetComponent<Collider>();

            // specific init
            InitEnemy();
        }

        /// <summary>
        /// To enable GC
        /// </summary>
        void UnsignFromEvents()
        {
            foreach (var p in Passengers)
            {
                p.OnPassengerDeath -= PassengerDied;
            }
        }

        void OnDestroy()
        {
            UnsignFromEvents();
        }

        /// <summary>
        /// Must be called on respawn.
        /// Restores enemy to alive state.
        /// </summary>
        public void Reinit()
        {
            State = EnemyVehicleState.Active;
            alivePassengersAmount = Passengers.Length;

            foreach (var p in Passengers)
            {
                p.Reinit();
            }

            // restore health
            Health = data.StartHealth;
            foreach (var d in damageReceivers)
            {
                d.ActivateNonKinematicCollider(true);
            }

            SetTarget(GameController.Instance.EnemyTarget);

            // specific activate
            Activate();
        }

        public void ReceiveDamage(Damage damage)
        {
            Health -= damage.CalculateDamageValue(transform.position);

            if (damage.Type == DamageType.Bullet)
            {
                ParticlesPool.Instance.Play(data.HitParticlesName, damage.Point, Quaternion.LookRotation(damage.Normal));
            }
            else if (damage.Type == DamageType.Explosion && Health > 0)
            {
                // TODO: change mesh parts to wrecked ones
                ParticlesPool.Instance.Play(data.HitParticlesName, transform.position, Quaternion.LookRotation(damage.Point - transform.position));
            }

            if (Health <= 0)
            {
                Explode(damage.Initiator);
            }
        }

        /// <summary>
        /// Return to object pool
        /// </summary>
        public void Return()
        {
            if (State == EnemyVehicleState.Nothing)
            {
                return;
            }

            State = EnemyVehicleState.Nothing;

            foreach (var p in Passengers)
            {
                p.Deactivate();
            }

            Deactivate();
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Explode this vehicle.
        /// Must be called on vehicle destuction
        /// </summary>
        /// <param name="initiator">damage initiator, f.e. player</param>
        public void Explode(GameObject initiator)
        {
            // ignore if not exist
            if (State == EnemyVehicleState.Nothing)
            {
                return;
            }

            KillAllPassengers(initiator);

            // generate wreck
            if (!string.IsNullOrEmpty(data.WreckName))
            {
                ObjectPool.Instance.GetObject(data.WreckName, transform.position, transform.rotation);
            }

            // and explosion
            ParticlesPool.Instance.Play(data.ExplosionName, transform.position, transform.rotation);

            // call event
            OnVehicleDestroy?.Invoke(data, transform, initiator);

            // disable vehicle after exlosion
            Return();
        }

        /// <summary>
        /// Set rigidbody kinematic.
        /// Also, disables mesh collider
        /// </summary>
        protected void SetKinematic(bool kinematic)
        {
            // disable non-convex mesh collider
            // as rigidbody doesnt work with them
            foreach (var d in damageReceivers)
            {
                d.ActivateNonKinematicCollider(kinematic);
            }

            VehicleRigidbody.isKinematic = kinematic;
        }

        /// <summary>
        /// Called on death of one of passengers
        /// </summary>
        void PassengerDied(EnemyData passengerData, Transform enemyPosition, GameObject initiator)
        {
            // check for incorrect states;
            // must be 'Active' or 'DeadDriver'
            if (State == EnemyVehicleState.DeadPassengers
                || (State == EnemyVehicleState.DeadDriver && passengerData.IsDriver)
                || State == EnemyVehicleState.Nothing)
            {
                Debug.Log("Called from wrong state", this);
                return;
            }

            // decrement
            alivePassengersAmount--;

            // call event
            OnEnemyDeath?.Invoke(passengerData, enemyPosition, initiator);

            // if there are passengers, but driver died
            if (passengerData.IsDriver)
            {
                State = EnemyVehicleState.DeadDriver;
                
                DoDriverDeath();
            }
            
            // if there are no alive passengers
            if (alivePassengersAmount <= 0)
            {
                alivePassengersAmount = 0;
                State = EnemyVehicleState.DeadPassengers;

                DoPassengerDeath();
            }
        }

        /// <summary>
        /// Usually called when vehicle is destroyed
        /// </summary>
        public void KillAllPassengers(GameObject initiator)
        {
            foreach (var p in Passengers)
            {
                p.Kill(initiator);
            }
        }

        /// <summary>
        /// Set target to passengers
        /// </summary>
        public void SetTarget(IEnemyTarget target)
        {
            foreach (var p in Passengers)
            {
                p.SetTarget(target);
            }
        }

        void OnCollisionEnter(Collision col)
        {
            var other = col.collider.GetComponent<IVehicle>();

            if (other != null)
            {
                VehicleCollisionInfo info = new VehicleCollisionInfo();

                // if driver is still alive, 
                // then send full damage,
                // otherwise nothing
                info.Damage = State == EnemyVehicleState.Active ? data.CollisionDamage : 0;

                // set to true as other vehicle must call 'Collide' method
                // on this one with other data
                info.ThisWithOther = true;

                info.Initiator = gameObject;

                // there is always at least one contact,
                // so this check is unnecessary
                if (col.contacts.Length > 0)
                {
                    info.CollisionPoint = col.contacts[0].point;
                    info.CollisionNormal = col.contacts[0].normal;
                }

                other.Collide(this, info);
            }
        }

        public void Collide(IVehicle other, VehicleCollisionInfo info)
        {
            ReceiveDamage(Damage.CreateBulletDamage(
                info.Damage, Vector3.forward, transform.position, Vector3.up, null));

            if (info.ThisWithOther)
            {
                VehicleCollisionInfo backInfo = new VehicleCollisionInfo();
                backInfo.Damage = State == EnemyVehicleState.Active ? data.CollisionDamage : 0;

                // don't process it on other side (to prevent loop)
                backInfo.ThisWithOther = false;

                backInfo.Initiator = gameObject;

                backInfo.CollisionPoint = info.CollisionPoint;
                backInfo.CollisionNormal = -info.CollisionNormal;

                other.Collide(this, backInfo);
            }

            DoVehicleCollision(info.Initiator);
        }

        void ISpawnable.GetExtents(out Vector3 min, out Vector3 max)
        {
            min = apxVehicleCollider.bounds.min;
            max = apxVehicleCollider.bounds.max;
        }

        Vector3 ISpawnable.Position => transform.position;
    }
}
