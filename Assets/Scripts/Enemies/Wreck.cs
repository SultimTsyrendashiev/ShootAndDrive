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

        Transform[] transforms;
        Rigidbody rb;

        // default local positions and rotations;
        // used for resetting to default
        Vector3[] defaultPositions;
        Quaternion[] defaultRotations;

        public void Init()
        {
            transforms = GetComponentsInChildren<Transform>();
            defaultPositions = new Vector3[transforms.Length];
            defaultRotations = new Quaternion[transforms.Length];

            for (int i = 0; i < transforms.Length; i++)
            {
                defaultPositions[i] = transforms[i].localPosition;
                defaultRotations[i] = transforms[i].localRotation;
            }

            rb = GetComponent<Rigidbody>();
        }

        public void Reinit()
        {
            // reset to default, except this transform
            for (int i = 0; i < transforms.Length; i++)
            {
                var t = transforms[i];

                if (t != transform)
                {
                    t.localPosition = defaultPositions[i];
                    t.localRotation = defaultRotations[i];
                }
            }

            // set init velocities
            rb.velocity = Random.onUnitSphere * Random.Range(0, maxVelocity);
            rb.angularVelocity = Random.onUnitSphere * Random.Range(0, maxAngularVelocity);
        }
    }
}