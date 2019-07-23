using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace SD.Enemies.Spawner
{
    class SpawnersController : MonoBehaviour
    {
        public const string EnemySedan  = "EnemySedan";
        public const string EnemyBike   = "EnemyBike";
        public const string EnemyCoupe  = "EnemyCoupe";
        public const string EnemyTruck  = "EnemyTruck";
        public const string EnemyVan    = "EnemyVan";

        const float DistanceBetweenSpawners = 50;

        // all spawned enemies by this class,
        // used for checking out of bounds of background
        LinkedList<ISpawnable>  spawnedObjects;

        List<ISpawner>          spawners;
        //Queue<int>            spawnersQueue; // holds indices to the list
        Vector3                 lastPosition;

        IBackgroundController background;

        public void Init()
        {
            spawners = new List<ISpawner>();
            //spawnersQueue = new Queue<int>();

            spawnedObjects = new LinkedList<ISpawnable>();

            AddSpawners();
        }

        void Start()
        {
            background = FindObjectOfType<Background.BackgroundController>();
        }

        /// <summary>
        /// Add spawners to list
        /// </summary>
        void AddSpawners()
        {
            spawners.Add(new RandomSpawner(background));
        }

        ///// <summary>
        ///// Add spawners to the queue
        ///// </summary>
        //void SetNextSpawner()
        //{
        //    int index = Random.Range(0, spawners.Count);
        //    spawnersQueue.Enqueue(index);
        //}

        /// <summary>
        /// Start enemy spawning
        /// </summary>
        /// <param name="position">start point</param>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public IEnumerator StartSpawn(Vector3 position, Transform player)
        {
            Vector3 currentPos = position;

            while (true)
            {
                // spawn to the limit of background blocks
                while ((currentPos - player.position).sqrMagnitude < Mathf.Pow(background.CurrentLength, 2))
                {
                    // add to the queue new spawner indices
                    //SetNextSpawner();
                    //int index = spawnersQueue.Dequeue();

                    int index = Random.Range(0, spawners.Count);
                    ISpawner spawner = spawners[index];

                    Vector2 bounds = background.GetBlockBounds(currentPos);

                    spawner.Spawn(currentPos, player, bounds, spawnedObjects);

                    // to the next spawner point
                    float d = spawner.Distance + DistanceBetweenSpawners;

                    currentPos += player.forward * d;
                }

                // return to pool, if out of background
                DisableUnused();

                // wait and then check again
                yield return null;
            }
        }

        public void Stop()
        {
            StopAllCoroutines();
        }

        /// <summary>
        /// Disable objects if they are out of background
        /// </summary>
        public void DisableUnused()
        {
            if (spawnedObjects.Count == 0)
            {
                return;
            }

            LinkedListNode<ISpawnable> node = spawnedObjects.First;

            // backward as items are removed while iterating over list
            do
            {
                LinkedListNode<ISpawnable> next = node.Next;
                ISpawnable s = node.Value;

                s.GetExtents(out Vector3 min, out Vector3 max);

                // check, if out of background
                if (background.IsOut(s.Position + min, s.Position + max))
                {
                    // return to object pool
                    s.Return();

                    // remove from active
                    spawnedObjects.Remove(node);
                }

                node = next;

            } while (node != null);
        }
    }
}
