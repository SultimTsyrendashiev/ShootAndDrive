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
            const float maxTorqueZ = 70;

            const float minTorqueY = -30;
            const float maxTorqueY = 50;

            float z = Random.Range(-maxTorqueZ, maxTorqueZ);
            float y = -Random.Range(minTorqueY, maxTorqueY);

            float mass = VehicleRigidbody.mass;
            Vector3 torque = 
                z * mass * transform.forward +
                y * mass * transform.right;

            VehicleRigidbody.AddTorque(torque, ForceMode.Impulse);
        }

        protected override void DoVehicleCollision()
        {
            KillAllPassengers(null);
        }
    }
}
