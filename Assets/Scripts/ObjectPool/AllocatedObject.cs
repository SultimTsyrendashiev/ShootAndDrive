using System.Collections.Generic;
using UnityEngine;

namespace SD.ObjectPooling
{
    class AllocatedPrefab
    {
        /// <summary>
        /// How mmany objects to allocate at once
        /// </summary>
        // const int ChunkSize = 4;

        IPooledObject           prefab;
        PooledObjectType        type;
        Transform               pool;
        List<IPooledObject>     allocated;
        int                     prevReturnedIndex;

        public AllocatedPrefab(Transform pool, IPooledObject prefab)
        {
            Debug.Assert(prefab.AmountInPool > 0, "Start amount is 0: " + prefab.ThisObject.name);

            this.prefab = prefab;
            this.type = prefab.Type;
            this.pool = pool;
            this.allocated = new List<IPooledObject>();
            this.prevReturnedIndex = -1;

            for (int i = 0; i < prefab.AmountInPool; i++)
            {
                AllocateNew(false);
            }
        }

        public GameObject GetObject()
        {
            int next = prevReturnedIndex + 1;
            next = next < allocated.Count ? next : 0;

            // check only next
            var nextObj = allocated[next].ThisObject;

            // if next not active, return it
            if (!nextObj.activeSelf)
            {
                nextObj.SetActive(true);
            }
            // if not available but important,
            // allocate new
            else if (type == PooledObjectType.Important)
            {
                prevReturnedIndex = allocated.Count;
                return AllocateNew(true);
            }
            // if not available and not important,
            // just return some object

            prevReturnedIndex = next;
            return nextObj;
        }

        GameObject AllocateNew(bool active)
        {
            GameObject result = GameObject.Instantiate(prefab.ThisObject);

            IPooledObject pooled = result.GetComponent<IPooledObject>();
            pooled.OnInit();

            result.name = prefab.ThisObject.name;
            result.SetActive(active);
            result.transform.parent = pool;

            allocated.Add(pooled);

            return result;
        }
    }
}
