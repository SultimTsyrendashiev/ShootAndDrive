using UnityEngine;

namespace SD
{
    interface IVehicle
    {
        void Collide(IVehicle other, VehicleCollisionInfo info);
    }

    struct VehicleCollisionInfo
    {
        public float Damage;
    }
}
