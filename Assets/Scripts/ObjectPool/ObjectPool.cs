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

        bool isInitialized = false;

        public static IObjectPool Instance { get; private set; }

        void Awake()
        {
            if (isInitialized)
            {
                return;
            }

            if (Instance != null)
            {
                Debug.Log("Several object pools. Destroying: ", this);

                // deactivate
                Destroy(this);
            }

            Init();
        }

        /// <summary>
        /// Must be called after initialization of all other systems
        /// </summary>
        public void Init()
        {
            isInitialized = true;

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
            Debug.Assert(pooled != null, "Prefab must contain 'IPooledObject' component", prefab);

            AllocatedPrefab ap = new AllocatedPrefab(transform, pooled);
            allocated.Add(prefab.name, ap);
        }

        public GameObject GetObject(string name)
        {
            // check if there is prefab
            if (!allocated.ContainsKey(name))
            {
                Debug.LogError("Object pool doesn't contain this key: " + name, this);
                return null;
            }

            // get object from local pool
            return allocated[name].GetObject();
        }

        public GameObject GetObject(string name, Vector3 position)
        {
            GameObject result = GetObject(name);
            result.transform.position = position;

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

        /// <summary>
        /// Change amount of allocated objects in a scene
        /// </summary>
        void Resize(string prefabName, int newAmount)
        {
            Debug.Assert(newAmount > 0, "New amount must be > 0", this);

            if (!allocated.ContainsKey(prefabName))
            {
                Debug.Log("Can't change amount in ObjectPool: prefab doesn't exist: " + prefabName, this);
                return;
            }

            var ap = allocated[prefabName];

            if (ap.Prefab.Type != PooledObjectType.NotImportant)
            {
                Debug.Log("Can't change amount in ObjectPool: only NotImportant can be resized", this);
            }

            ap.Resize(newAmount);
        }
    }
}
