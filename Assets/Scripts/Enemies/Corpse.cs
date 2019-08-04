using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SD.Enemies
{
    /// <summary>
    /// Ragdoll
    /// </summary>
    class Corpse : MonoBehaviour, IPooledObject
    {
        /// <summary>
        /// Velocity will be applied only to this rigidbody
        /// </summary>
        [SerializeField]
        Rigidbody mainRigidbody;

        public GameObject ThisObject => gameObject;
        public PooledObjectType Type => PooledObjectType.NotImportant; // dont allocate more than specified
        public int AmountInPool => 5;

        ReinittableTR reinittable;

        public void Init()
        {
            Debug.Assert(mainRigidbody != null, "Main rigidbody is not set");

            var transforms = GetComponentsInChildren<Transform>(true);
            var rigidbodies = GetComponentsInChildren<Rigidbody>(true);

            reinittable = new ReinittableTR(transform, transforms, rigidbodies);
            reinittable.Load();
        }

        public void Reinit()
        {
            // reset tranforms
            reinittable.ReinitTransforms();

            // reset all parts' velocities to zero
            reinittable.ReinitRigidbodies(Vector3.zero, Vector3.zero);
        }

        public void Launch(Vector3 velocity, Vector3 angularVelocity)
        {
            mainRigidbody.velocity = velocity;
            mainRigidbody.angularVelocity = angularVelocity;
        }
    }
}
