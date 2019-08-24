using System.Collections.Generic;
using UnityEngine;
    
namespace SD.Enemies.Spawner
{
    class SpawnersController
    {
        const float CleanTimePeriod = 1.0f;
        const float MaxSpawnDistance = 200;
        
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

        Vector3                 currentPos;
        bool                    shouldSpawn;

        // when to return to pool out of bounds enemies
        float                   cleanTime;

        IEnemyTarget            target;

        IBackgroundController   background;
        IBackgroundController   Background
        {
            get
            {
                // if not set
                if (background == null)
                {
                    // try to find
                    background = GameController.Instance.Background;
                }

                return background;
            }
        }

        public SpawnersController()
        {
            shouldSpawn = false;

            spawners = new List<ISpawner>();
            //spawnersQueue = new Queue<int>();
            spawnedObjects = new LinkedList<ISpawnable>();

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
            spawners.Add(new TruckSpawner());
        }

        void FindTarget()
        {
            target = GameController.Instance.EnemyTarget;
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
        /// Restarts spawner in safe distance from the enemy target.
        /// This target is specified in game controller
        /// </summary>
        public void RestartSpawn()
        {
            if (target == null)
            {
                Debug.Log("SpawnersController::Target is null");
                return;
            }

            const float safeDistance = 200;
            RestartSpawn(target.Target.position + Vector3.forward * safeDistance);
        }

        /// <summary>
        /// Restarts spawner in specified position
        /// </summary>
        public void RestartSpawn(Vector3 startPosition)
        {
            // delete all previously spawned objects
            DisableAll();

            shouldSpawn = true;
            currentPos = startPosition;
            cleanTime = Time.time + CleanTimePeriod;
        }

        /// <summary>
        /// Start enemy spawning
        /// </summary>
        /// <param name="position">start point</param>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public void Update()
        {
            float currentTime = Time.time;

            if (currentTime > cleanTime)
            {
                // return to pool, if out of background
                DisableUnused();

                cleanTime = currentTime + CleanTimePeriod;
            }

            if (!shouldSpawn)
            {
                return;
            }

            if (target == null)
            {
                // find target
                FindTarget();

                // and try next time
                return;
            }

            // spawn to the limit of background blocks
            while ((currentPos - target.Target.position).sqrMagnitude < MaxSpawnDistance * MaxSpawnDistance)
            {
                // add to the queue new spawner indices
                //SetNextSpawner();
                //int index = spawnersQueue.Dequeue();

                int index = Random.Range(0, spawners.Count);
                ISpawner spawner = spawners[index];

                Vector2 bounds = Background.GetBlockBounds(currentPos);

                spawner.Spawn(currentPos, target.Target, bounds, spawnedObjects);

                // to the next spawner point
                float d = spawner.Distance;

                currentPos += target.Target.forward * d;
            }
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

            do
            {
                LinkedListNode<ISpawnable> next = node.Next;
                ISpawnable s = node.Value;

                s.GetExtents(out Vector3 min, out Vector3 max);

                // check, if out of background
                if (Background.IsOut(s.Position + min, s.Position + max))
                {
                    // return to object pool
                    s.Return();

                    // remove from active
                    spawnedObjects.Remove(node);
                }

                node = next;

            } while (node != null);
        }

        /// <summary>
        /// Disable all spawned objects
        /// </summary>
        void DisableAll()
        {
            if (spawnedObjects.Count == 0)
            {
                return;
            }

            LinkedListNode<ISpawnable> node = spawnedObjects.First;

            do
            {
                LinkedListNode<ISpawnable> next = node.Next;
                ISpawnable s = node.Value;

                // return to object pool
                s.Return();

                // remove from active
                spawnedObjects.Remove(node);

                node = next;

            } while (node != null);
        }
    }
}
