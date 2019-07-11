using System.Collections;
using UnityEngine;

namespace SD.Enemies
{
    class EnemySedan : EnemyVehicle
    {
        Rigidbody vehicleRigidbody;

        protected override void Activate()
        {
            vehicleRigidbody.isKinematic = true;
            vehicleRigidbody.velocity = transform.forward * Data.Speed;
        }

        protected override void DoDriverDeath()
        {
            StartCoroutine(StopVehicle());
        }

        IEnumerator StopVehicle()
        {
            const float stopEpsilon = 0.1f;

            float brakeTime = 1;
            Vector3 velocity = vehicleRigidbody.velocity;

            while (velocity.sqrMagnitude > stopEpsilon)
            {
                velocity = Vector3.Lerp(velocity, Vector3.zero, Time.fixedDeltaTime / brakeTime);
                vehicleRigidbody.velocity = velocity;

                yield return new WaitForFixedUpdate();
            }
        }
    }
}
