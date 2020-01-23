using System.Collections;
using UnityEngine;

namespace SD.Enemies
{
    class EnemySedan : EnemyVehicle
    {
        [SerializeField]
        Collider vehicleAimTarget;

        [SerializeField]
        float deathTorque;

        Vector3 velocity;

        protected override void Activate()
        {
            VehicleRigidbody.isKinematic = true;
            velocity = transform.forward * Data.Speed;

            if (vehicleAimTarget != null)
            {
                vehicleAimTarget.enabled = false;
            }
        }

        void FixedUpdate()
        {
            if (State == EnemyVehicleState.Active)
            {
                VehicleRigidbody.position += velocity * Time.fixedDeltaTime;
            }
        }

        protected override void DoDriverDeath()
        {
            // just enable physics for this vehicle
            SetKinematic(false);

            // set its velocity
            VehicleRigidbody.velocity = velocity;

            // and add random torque
            float maxTorque = deathTorque;
            float t = Random.Range(-maxTorque, maxTorque);

            VehicleRigidbody.AddTorque(t * VehicleRigidbody.mass * transform.up, ForceMode.Impulse);
        }

        protected override void DoVehicleCollision(GameObject initiator)
        {
            KillAllPassengers(initiator);
        }

        protected override void DoPassengerDeath()
        {
            if (vehicleAimTarget != null)
            {
                // when all passengers are dead, set vehicle as aim target
                vehicleAimTarget.enabled = true;
            }
        }

        //IEnumerator StopVehicle()
        //{
        //    const float stopEpsilon = 0.1f;

        //    float brakeTime = Data.BrakingTime;
        //    Vector3 velocity = VehicleRigidbody.velocity;

        //    while (velocity.sqrMagnitude > stopEpsilon)
        //    {
        //        velocity = Vector3.Lerp(velocity, Vector3.zero, Time.fixedDeltaTime / brakeTime);
        //        VehicleRigidbody.velocity = velocity;

        //        yield return new WaitForFixedUpdate();
        //    }
        //}
    }
}
