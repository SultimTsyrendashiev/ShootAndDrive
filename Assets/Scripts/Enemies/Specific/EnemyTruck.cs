using UnityEngine;

namespace SD.Enemies
{
    class EnemyTruck : EnemyVehicle
    {
        Vector3 velocity;

        protected override void Activate()
        {
            VehicleRigidbody.isKinematic = true;
            velocity = transform.forward * Data.Speed;
        }

        void FixedUpdate()
        {
            VehicleRigidbody.position += velocity * Time.fixedDeltaTime;
        }
    }
}
