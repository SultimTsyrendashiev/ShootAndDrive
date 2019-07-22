using System.Collections;
using UnityEngine;

namespace SD.Enemies
{
    class EnemyBike : EnemyVehicle
    {
        Vector3 velocity;

        protected override void Activate()
        {
            VehicleRigidbody.isKinematic = true;
            velocity = transform.forward * Data.Speed;
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
            const float maxTorqueX = 5;

            const float minTorqueY = -20;
            const float maxTorqueY = 14;

            float horizontal = Random.Range(-maxTorqueX, maxTorqueX);
            float vertical = -Random.Range(minTorqueY, maxTorqueY);

            float mass = VehicleRigidbody.mass;
            VehicleRigidbody.AddTorque(horizontal * mass * transform.up, ForceMode.Impulse);
            VehicleRigidbody.AddTorque(vertical * mass * transform.right, ForceMode.Impulse);
        }

        protected override void DoVehicleCollision()
        {
            KillAllPassengers();
        }
    }
}
