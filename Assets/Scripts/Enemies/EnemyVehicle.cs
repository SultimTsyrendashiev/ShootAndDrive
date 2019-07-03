using System.Collections;
using UnityEngine;

namespace SD.Enemies
{
    delegate void PassengerDied(bool isDriver);

    abstract class EnemyVehicle : MonoBehaviour, IEnemy
    {
        public EnemyVehicleState State { get; private set; }
        protected VehiclePassenger[] Passengers { get; private set; }
        public bool AliveDriver { get; private set; }

        [SerializeField]
        int passengersStartHealth;
        [SerializeField]
        int vehicleStarthealth;
        EnemyVehicleDamageReceiver damageReceiver;

        int alivePassengersAmount;

        /// <summary>
        /// Called on activating
        /// </summary>
        protected virtual void Activate() { }
        /// <summary>
        /// Called on deactivating
        /// </summary>
        protected virtual void Deactivate() { }
        /// <summary>
        /// Called on death
        /// </summary>
        protected abstract void Die();

        /// <summary>
        /// Must be called only once
        /// </summary>
        public void Init()
        {
            // get all passengers and init them
            Passengers = GetComponentsInChildren<VehiclePassenger>(true);
            foreach (var p in Passengers)
            {
                p.Init(passengersStartHealth);
                p.OnPassengerDeath += PassengerDied;
            }

            // get vehicle collision model and init it
            damageReceiver = GetComponentInChildren<EnemyVehicleDamageReceiver>(true);
            damageReceiver.Init(vehicleStarthealth);
            damageReceiver.OnVehicleDeath += Explode;
        }

        /// <summary>
        /// Must be called on respawn.
        /// Restores enemy to alive state.
        /// </summary>
        void Reinit()
        {
            alivePassengersAmount = Passengers.Length;
            AliveDriver = true;

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

            Deactivate();
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Explode this vehicle.
        /// Must be called on vehicle 
        /// </summary>
        void Explode()
        {
            if (State == EnemyVehicleState.Destroyed
                || State == EnemyVehicleState.Nothing)
            {
                return;
            }

            State = EnemyVehicleState.Destroyed;
        }

        /// <summary>
        /// Called on death of one of passengers
        /// </summary>
        void PassengerDied(bool driver)
        {
            // check for incorrect states
            if (State == EnemyVehicleState.Dead 
                || State == EnemyVehicleState.DeadDriver 
                || State == EnemyVehicleState.Nothing)
            {
                Debug.Log("Wrong state", this);
                return;
            }

            // ignore if destroyed
            if (State != EnemyVehicleState.Destroyed)
            {
                return;
            }

            alivePassengersAmount--;

            // if there are passengers
            // but driver died for first time
            if (alivePassengersAmount > 0 && driver
                && State != EnemyVehicleState.DeadDriver)
            {
                State = EnemyVehicleState.DeadDriver;
                AliveDriver = false;

                Die();

                return;
            }

            // if there are no alive passengers
            if (alivePassengersAmount <= 0)
            {
                State = EnemyVehicleState.Dead;
                Die();
            }
        }
    }
}
