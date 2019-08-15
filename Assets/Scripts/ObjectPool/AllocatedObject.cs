using System.Collections.Generic;
using UnityEngine;

namespace SD.ObjectPooling
{
    class AllocatedPrefab
    {
        PooledObjectType        type;
        Transform               pool;
        List<IPooledObject>     allocated;
        int                     prevReturnedIndex;

        public IPooledObject    Prefab { get; }
        /// <summary>
        /// How many objects are allocated?
        /// </summary>
        public int              Amount => allocated.Count;

        public AllocatedPrefab(Transform pool, IPooledObject prefab)
        {
            Debug.Assert(!(prefab.AmountInPool == 0 && prefab.Type == PooledObjectType.NotImportant), 
                "Start amount of not important is 0: " + prefab.ThisObject.name);

            this.Prefab = prefab;
            this.type = prefab.Type;
            this.pool = pool;
            this.allocated = new List<IPooledObject>();
            this.prevReturnedIndex = -1;

            for (int i = 0; i < prefab.AmountInPool; i++)
            {
                AllocateNew(false, false);
            }
        }

        /// <summary>
        /// Get object with default position and rotation
        /// </summary>
        /// <returns></returns>
        public GameObject GetObject()
        {
            int next = prevReturnedIndex + 1;
            next = next < allocated.Count ? next : 0;

            // check only next
            var nextPooled = allocated[next];
            var nextObj = nextPooled.ThisObject;

            // if next not active, return it
            if (!nextObj.activeSelf)
            {
                nextObj.SetActive(true);
            }
            // if not available but important,
            // allocate new
            else if (type == PooledObjectType.Important)
            {
                // find not active
                int available = FindAvailable();

                // if not found, create new
                if (available == -1)
                {
                    prevReturnedIndex = allocated.Count;
                    return AllocateNew(true, true); // create active and reinit
                }
                // otherwise
                else
                {
                    next = available;

                    nextPooled = allocated[next];
                    nextObj = nextPooled.ThisObject;

                    nextObj.SetActive(true);
                }
            }
            // if not available and not important,
            // just return some object

            prevReturnedIndex = next;

            // set default rotation and position
            nextObj.transform.position = Prefab.ThisObject.transform.position;
            nextObj.transform.rotation = Prefab.ThisObject.transform.rotation;

            // reinit when get object from pool
            nextPooled.Reinit();
            return nextObj;
        }

        int FindAvailable()
        {
            int startIndex =prevReturnedIndex + 1;

            for (int i = startIndex; i < allocated.Count; i++)
            {
                if (!allocated[i].ThisObject.activeSelf)
                {
                    return i;
                }
            }

            for (int i = 0; i < startIndex; i++)
            {
                if (!allocated[i].ThisObject.activeSelf)
                {
                    return i;
                }
            }

            // can't find
            return -1;
        }

        GameObject AllocateNew(bool active, bool toReinit)
        {
            GameObject result = Object.Instantiate(Prefab.ThisObject);

            IPooledObject pooled = result.GetComponent<IPooledObject>();
            pooled.Init();

            result.name = Prefab.ThisObject.name;
            result.SetActive(active);
            result.transform.parent = pool;

            if (toReinit)
            {
                pooled.Reinit();
            }

            allocated.Add(pooled);

            return result;
        }

        public void Resize(int newAmount)
        {
            if (allocated.Count == newAmount)
            {
                return;
            }

            List<int> available = new List<int>();

            // find all inactive objects
            for (int i = 0; i < allocated.Count; i++)
            {
                if (allocated[i].ThisObject.activeSelf)
                {
                    available.Add(i);
                }
            }

            // destroy only not active objects
            for (int i = 0; i < available.Count; i++)
            {
                allocated.RemoveAt(available[i]);

                // return, if desired amount is set
                if (allocated.Count == newAmount)
                {
                    return;
                }
            }

            // if desired amount is not set,
            // some active objects must be destroyed too
            for (int i = 0; i < allocated.Count; i++)
            {
                allocated.RemoveAt(i);

                if (allocated.Count == newAmount)
                {
                    return;
                }
            }
        }
    }
}
