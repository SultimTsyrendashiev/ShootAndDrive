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

        const float DistanceBetweenSpawners = 40;

        List<ISpawner>  spawners;
        //Queue<int>      spawnersQueue; // holds indices to the list
        Vector3         lastPosition;

        public void Init()
        {
            spawners = new List<ISpawner>();
            //spawnersQueue = new Queue<int>();

            AddSpawners();
        }

        /// <summary>
        /// Add spawners to list
        /// </summary>
        void AddSpawners()
        {
            spawners.Add(new RandomSpawner());
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
            var background = FindObjectOfType<Background.BackgroundController>();
            Vector3 currentPos = position;
            Vector3 playerForward = player.forward;

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

                    // TODO: delete
                    Vector2 bounds = new Vector2(-7, 7);

                    spawner.Spawn(currentPos, playerForward, bounds);

                    // to the next spawner point
                    float d = spawner.Distance + DistanceBetweenSpawners;

                    currentPos += playerForward * d;
                }

                // wait and then check again
                yield return null;
            }
        }

        public void Stop()
        {
            StopAllCoroutines();
        }
    }
}
