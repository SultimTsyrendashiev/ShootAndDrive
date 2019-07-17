using UnityEngine;

namespace SD.Enemies
{
    class EnemyTruck : EnemyVehicle
    {
        Rigidbody vehicleRigidbody;
        Vector3 velocity;

        protected override void Activate()
        {
            vehicleRigidbody.isKinematic = true;
            velocity = transform.forward * Data.Speed;
        }

        void FixedUpdate()
        {
            vehicleRigidbody.position += velocity * Time.fixedDeltaTime;
        }

        // do nothing as there is no driver
        protected override void DoDriverDeath()
        { }
    }
}
