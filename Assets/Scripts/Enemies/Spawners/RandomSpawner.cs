using System.Collections.Generic;
using UnityEngine;

namespace SD.Enemies.Spawner
{
    class RandomSpawner : ISpawner
    {
        const int       Count = 4;
        const float     DistanceBetween = 5;
        // safe distance between spawned enemies
        const float     SafeDistance = 0.5f;

        // what enemies to spawn
        readonly string[] names = { SpawnersController.EnemySedan, SpawnersController.EnemyTruck };
        readonly IBackgroundController background;

        // objects that spawned at one call of 'Spawn';
        // temporary collection for checking intersections
        //     between spawnable objects;
        // using list, as amount in list in very small
        List<ISpawnable> batch;

        public float Distance => Count * DistanceBetween;

        public RandomSpawner(IBackgroundController background)
        {
            this.background = background;
            batch = new List<ISpawnable>(Count);
        }

        public void Spawn(Vector3 position, Transform player, Vector2 bounds, ICollection<ISpawnable> allSpawned)
        {
            // how many times to try to create an object,
            // if there was a collision
            const int maxTries = 3;

            for (int i = 0; i < Count; i++)
            {
                // get object
                int index = Random.Range(0, names.Length);
                var obj = ObjectPool.Instance.GetObject(names[index]);
                var spawnable = obj.GetComponent<ISpawnable>();
                spawnable.GetExtents(out Vector3 smin, out Vector3 smax);

                // position
                Vector3 spawnPos = position;
                bool failed = false;

                for (int j = 0; j < maxTries; j++)
                {
                    float x = Random.Range(bounds[0], bounds[1]);

                    int row = Random.Range(0, Count);
                    float z = row * DistanceBetween;

                    spawnPos = position + player.right * x + player.forward * z;

                    // firstly, check with previous
                    if (!CheckIntersection(spawnPos + smin, spawnPos + smax))
                    {
                        // if no intersection, then ok
                        break;
                    }
                    else if (j == maxTries - 1)
                    {
                        // if last try
                        failed = true;
                    }
                }

                if (!failed)
                {
                    // add only after checking
                    batch.Add(spawnable);
                    // register
                    allSpawned.Add(spawnable);

                    // set new position
                    obj.transform.position = spawnPos;
                }
                else
                {
                    // all tries are failed, return back to pool
                    spawnable.Return();
                }
            }

            // spawning ended, clear temorary list
            batch.Clear();
        }

        /// <summary>
        /// Check intersection with previously spawned in a batch
        /// </summary>
        bool CheckIntersection(Vector3 smin, Vector3 smax)
        {
            foreach (var b in batch)
            {
                // get AABB
                b.GetExtents(out Vector3 min, out Vector3 max);

                if (smin.x - SafeDistance <= max.x && smax.x + SafeDistance >= min.x &&
                    smin.y - SafeDistance <= max.y && smax.y + SafeDistance >= min.y &&
                    smin.z - SafeDistance <= max.z && smax.z + SafeDistance >= min.z)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
