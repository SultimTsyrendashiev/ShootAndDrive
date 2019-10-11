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

        [SerializeField]
        ParticleSystem smoke;

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

            Vector3 v = Random.onUnitSphere;

            if (v.y < 0)
            {
                v.y = -v.y;
            }

            if (v.y < 0.2f)
            {
                v.y = 0.2f;
            }

            v *= Random.Range(0.6f * maxVelocity, maxVelocity);

            Vector3 av = Random.onUnitSphere * Random.Range(0.6f * maxAngularVelocity, maxAngularVelocity);
            reinittable.ReinitRigidbodies(v, av);

            smoke?.Play();
        }
    }
}