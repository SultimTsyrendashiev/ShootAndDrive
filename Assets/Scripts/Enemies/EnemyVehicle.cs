using System.Collections;
using UnityEngine;

namespace SD.Enemies
{
    // common delegates
    delegate void EnemyDied(EnemyData data);
    delegate void VehicleDestroyed(EnemyVehicleData data);

    // from passengers of vehicles
    delegate void PassengerDied(EnemyData data);

    /// <summary>
    /// Represents enemy as vehicle with passengers.
    /// There must be vehicle damage receiver as a child object,
    /// also passengers, if needed
    /// </summary>
    abstract class EnemyVehicle : MonoBehaviour, IEnemy, IPooledObject
    {
        [SerializeField]
        EnemyVehicleData data;

        EnemyVehicleDamageReceiver damageReceiver;
        int alivePassengersAmount;

        public EnemyVehicleState        State       { get; private set; }
        protected VehiclePassenger[]    Passengers  { get; private set; }
        public bool                     AliveDriver => State != EnemyVehicleState.DeadDriver && alivePassengersAmount > 0;
        public EnemyVehicleData         Data        => data;

        GameObject          IPooledObject.ThisObject    => gameObject;
        PooledObjectType    IPooledObject.Type          => PooledObjectType.Important;
        int                 IPooledObject.AmountInPool  => 8;

        public static event EnemyDied           OnEnemyDeath;
        public static event VehicleDestroyed    OnVehicleDestroy;

        /// <summary>
        /// Called on activating
        /// </summary>
        protected virtual void Activate() { }
        /// <summary>
        /// Called on deactivating
        /// </summary>
        protected virtual void Deactivate() { }

        /// <summary>
        /// Called on death of all passengers.
        /// Virtual, as it's only impprtant when driver dies
        /// </summary>
        protected virtual void DoPassengerDeath() { }
        
        /// <summary>
        /// Called on death of driver
        /// </summary>
        protected abstract void DoDriverDeath();

        public void Init()
        {
            // get all passengers and init them
            Passengers = GetComponentsInChildren<VehiclePassenger>(true);
            foreach (var p in Passengers)
            {
                p.Init(this);
                p.OnPassengerDeath += PassengerDied;
            }

            // get vehicle collision model and init it
            damageReceiver = GetComponentInChildren<EnemyVehicleDamageReceiver>(true);
            damageReceiver.Init(this);
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

            damageReceiver.Reinit();
        }

        /// <summary>
        /// Spawn enemy on given position
        /// </summary>
        public void Spawn(Vector3 position)
        {
            if (State != EnemyVehicleState.Nothing)
            {
                Debug.Log("Already spawned", this);
            }

            transform.position = position;

            gameObject.SetActive(true);
            State = EnemyVehicleState.Active;

            Reinit();
            Activate();
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
        public void Explode()
        {
            // ignore if not exist
            if (State == EnemyVehicleState.Nothing)
            {
                return;
            }

            KillAllPassengers();

            // generate explosiion
            ParticlesPool.Instance.Play(data.ExplosionName, transform.position, transform.rotation);

            // call event
            OnVehicleDestroy(data);
            State = EnemyVehicleState.Nothing;
        }

        /// <summary>
        /// Called on death of one of passengers
        /// </summary>
        void PassengerDied(EnemyData passengerData)
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
            OnEnemyDeath(passengerData);

            // if there are passengers, but driver died
            if (alivePassengersAmount > 0 && passengerData.IsDriver)
            {
                State = EnemyVehicleState.DeadDriver;
                
                DoDriverDeath();
            }
            // if there are no alive passengers
            else if (alivePassengersAmount <= 0)
            {
                alivePassengersAmount = 0;
                State = EnemyVehicleState.DeadPassengers;

                DoPassengerDeath();
            }
        }

        /// <summary>
        /// Usually called when vehicle is destroyed
        /// </summary>
        public void KillAllPassengers()
        {
            foreach (var p in Passengers)
            {
                p.Kill();
            }
        }

        public void SetTarget(Transform target)
        {
            foreach (var p in Passengers)
            {
                p.SetTarget(target);
            }
        }
    }
}
