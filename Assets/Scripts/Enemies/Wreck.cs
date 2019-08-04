using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SD.Enemies
{
    class Wreck : MonoBehaviour, IPooledObject
    {
        [SerializeField]
        float maxVelocity = 10;
        [SerializeField]
        float maxAngularVelocity = 30;

        public GameObject ThisObject => gameObject;
        public PooledObjectType Type => PooledObjectType.NotImportant;
        public int AmountInPool => 4;

        ReinittableTR reinittable;

        public void Init()
        {
            var transforms = GetComponentsInChildren<Transform>(true);
            var rigidbodies = GetComponentsInChildren<Rigidbody>(true);

            reinittable = new ReinittableTR(transform, transforms, rigidbodies);
            reinittable.Load();
        }

        public void Reinit()
        {
            reinittable.ReinitTransforms();

            Vector3 v = Random.onUnitSphere * Random.Range(0, maxVelocity);
            Vector3 av = Random.onUnitSphere * Random.Range(0, maxAngularVelocity);
            reinittable.ReinitRigidbodies(v, av);
        }
    }
}