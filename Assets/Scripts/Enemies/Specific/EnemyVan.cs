using System.Collections;
using UnityEngine;

namespace SD.Enemies
{
    // TODO: doors opening
    class EnemyVan : EnemyVehicle
    {
        /// <summary>
        /// When passengers must start attack
        /// </summary>
        const float AttackDistance = 10;

        /// <summary>
        /// Speed when all passengers died
        /// </summary>
        float driveAwaySpeed = 19;

        Vector3 velocity;
        Transform target;
        bool doorsOpened;

        protected override void Activate()
        {
            VehicleRigidbody.isKinematic = true;
            velocity = transform.forward * Data.Speed;

            // find target
            var player = FindObjectOfType<PlayerLogic.Player>();
            target = player.transform;

            if (player.Vehicle != null)
            {
                const float diff = 0.5f;
                driveAwaySpeed = player.Vehicle.DefaultSpeed - diff;
            }

            CloseDoors();
        }

        void FixedUpdate()
        {
            if (State == EnemyVehicleState.Active)
            {
                // distance between van and target along target forward vector
                float projLength = Vector3.Dot(VehicleRigidbody.position - target.position, target.forward);

                // clamp position, if close to target
                if (Mathf.Abs(projLength) <= AttackDistance)
                {
                    VehicleRigidbody.position = target.position + target.forward * AttackDistance;

                    // open doors if not opened
                    if (!doorsOpened)
                    {
                        OpenDoors();
                    }

                    return;
                }

                VehicleRigidbody.position += velocity * Time.fixedDeltaTime;
            }
            else if (State == EnemyVehicleState.DeadPassengers)
            {
                // drive away
                VehicleRigidbody.position += velocity * Time.fixedDeltaTime;
            }
        }


        // when all passengers died, 
        // just drive away;
        protected override void DoPassengerDeath()
        {
            velocity = transform.forward * driveAwaySpeed;
        }

        /// <summary>
        /// Close doors on activating, i.e. without any effects
        /// </summary>
        void CloseDoors()
        {
            doorsOpened = false;

            // deactivate passengers
            foreach (var p in Passengers)
            {
                p.SetTarget(null);
            }

            // TODO: reset doors' transforms
        }

        /// <summary>
        /// Open doors with animation, etc.
        /// Also, this method must activate passengers for attacking
        /// </summary>
        IEnumerator OpenDoors()
        {
            doorsOpened = true;

            // TODO: start animation (or maybe activate physics model)

            // wait for opening
            float toWait = 0.5f;
            yield return new WaitForSeconds(toWait);

            // activate passengers
            foreach (var p in Passengers)
            {
                p.SetTarget(target);
            }
        }
    }
}
