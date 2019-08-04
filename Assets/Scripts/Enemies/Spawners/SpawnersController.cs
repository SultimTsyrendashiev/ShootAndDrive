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

        // all registered spawners
        List<ISpawner>          spawners;
        // all spawned enemies,
        // used for checking out of bounds of background
        LinkedList<ISpawnable>  spawnedObjects;
        //Queue<int>            spawnersQueue; // holds indices to the list

        Transform               player;
        Vector3                 currentPos;
        bool                    shouldSpawn;

        IBackgroundController   background;

        public void Init()
        {
            shouldSpawn = false;

            spawners = new List<ISpawner>();
            //spawnersQueue = new Queue<int>();
            spawnedObjects = new LinkedList<ISpawnable>();
        }

        void Start()
        {
            background = FindObjectOfType<Background.BackgroundController>();
            AddSpawners();
        }

        /// <summary>
        /// Add spawners to list
        /// </summary>
        void AddSpawners()
        {
            //spawners.Add(new RandomSpawner());
            spawners.Add(new ArrowSpawner());
            spawners.Add(new SixSpawner());
            spawners.Add(new WSpawner());
            spawners.Add(new CircleSpawner());
            spawners.Add(new VanSpawner());
        }

        ///// <summary>
        ///// Add spawners to the queue
        ///// </summary>
        //void SetNextSpawner()
        //{
        //    int index = Random.Range(0, spawners.Count);
        //    spawnersQueue.Enqueue(index);
        //}

        public void StartSpawn(Vector3 startPosition, Transform player)
        {
            shouldSpawn = true;
            currentPos = startPosition;
            this.player = player;
        }

        /// <summary>
        /// Start enemy spawning
        /// </summary>
        /// <param name="position">start point</param>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public void Update()
        {
            if (!shouldSpawn)
            {
                return;
            }

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
                float d = spawner.Distance;

                currentPos += player.forward * d;
            }

            // return to pool, if out of background
            DisableUnused();
        }

        public void Stop()
        {
            shouldSpawn = false;
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
