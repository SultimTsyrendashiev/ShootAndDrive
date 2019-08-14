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
        
        /// <summary>
        /// Should this vehicle call 'Collide' method of caller vehicle?
        /// </summary>
        public bool ThisWithOther;
    }
}
