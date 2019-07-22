using System.Collections;
using UnityEngine;

namespace SD.Enemies
{
    class EnemyBomber : EnemyVehicle
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
            Explode();
        }
    }
}
