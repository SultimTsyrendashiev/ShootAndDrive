using System.Collections.Generic;
using UnityEngine;
using SD.ObjectPooling;

namespace SD
{
    class ObjectPool : MonoBehaviour, IObjectPool
    {
        /// <summary>
        /// Contains all prefabs
        /// </summary>
        [SerializeField]
        ObjectPoolPrefabs prefabs;
        // Contains all already allocated objects
        Dictionary<string, AllocatedPrefab> allocated;

        public static IObjectPool Instance { get; private set; }

        void Awake()
        {
            Debug.Assert(Instance == null, "Several object pools", this);
            Instance = this;

            allocated = new Dictionary<string, AllocatedPrefab>();

            foreach (var o in prefabs.Prefabs)
            {
                Register(o);
            }
        }
        
        /// <summary>
        /// Add prefab to object pool
        /// </summary>
        public void Register(GameObject prefab)
        {
            var pooled = prefab.GetComponent<IPooledObject>();
            Debug.Assert(pooled != null, "Prefab must contain 'IPooledObject' component", this);

            AllocatedPrefab ap = new AllocatedPrefab(transform, pooled);
            allocated.Add(prefab.name, ap);
        }

        public GameObject GetObject(string name)
        {
            // check if there is prefab
            Debug.Assert(allocated.ContainsKey(name), "Object pool doesn't contain this key: " + name, this);

            // get object from local pool
            return allocated[name].GetObject();
        }

        public GameObject GetObject(string name, Vector3 position)
        {
            GameObject result = GetObject(name);
            result.transform.position = position;

            // get rotation from prefab
            result.transform.rotation = allocated[name].Prefab.ThisObject.transform.rotation;

            return result;
        }

        public GameObject GetObject(string name, Vector3 position, Quaternion rotation)
        {
            GameObject result = GetObject(name);
            result.transform.position = position;
            result.transform.rotation = rotation;

            return result;
        }

        public GameObject GetObject(string name, Vector3 position, Vector3 direction)
        {
            return GetObject(name, position, Quaternion.LookRotation(direction));
        }
    }
}
