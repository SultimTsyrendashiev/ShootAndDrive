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

        public GameObject Initiator;

        /// <summary>
        /// Should this vehicle call 'Collide' method of caller vehicle?
        /// </summary>
        public bool ThisWithOther;

        public Vector3 CollisionPoint;
        public Vector3 CollisionNormal;
    }
}
