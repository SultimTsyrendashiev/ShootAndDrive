using System.Collections;
using UnityEngine;

namespace SD.Enemies
{
    class EnemySedan : EnemyVehicle
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
            print("Driver died");

            //StartCoroutine(StopVehicle());

            // just enable physics for this vehicle
            VehicleRigidbody.isKinematic = false;

            // and add random torque
            const float maxTorque = 80;

            float t = Random.Range(-maxTorque, maxTorque);
            if (t < -maxTorque / 2)
            {
                t = -maxTorque / 2;
            }
            else if (t > maxTorque / 2)
            {
                t = maxTorque / 2;
            }

            VehicleRigidbody.AddTorque(transform.up, ForceMode.Impulse);
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
