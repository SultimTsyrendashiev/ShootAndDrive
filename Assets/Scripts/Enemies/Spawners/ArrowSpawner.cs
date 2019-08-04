using System.Collections.Generic;
using UnityEngine;

namespace SD.Enemies.Spawner
{
    class ArrowSpawner : ISpawner
    {
        const float Length = 40;
        const float Safe = 80;

        // what enemies to spawn
        readonly string[] names = {
             SpawnersController.EnemySedan,
             SpawnersController.EnemyBike
        };

        public float Distance => Length + Safe;

        public void Spawn(Vector3 position, Transform player, Vector2 bounds, ICollection<ISpawnable> allSpawned)
        {
            Vector3[] positions = new Vector3[5];
            for (int i = 0; i < 5; i++)
            {
                positions[i] = position;
            }

            // at bounds
            positions[0].x = bounds[0] + 1.5f;
            positions[4].x = bounds[1] - 1.5f;

            // at the middle of center and bounds
            positions[1].x = (positions[0].x + positions[2].x) / 2;
            positions[3].x = (positions[2].x + positions[4].x) / 2;

            positions[0].z += Length;
            positions[4].z += Length;

            positions[1].z += Length / 2;
            positions[3].z += Length / 2;


            int indexA = Random.Range(0, names.Length);
            int indexB = Random.Range(0, names.Length);

            for (int i = 0; i < 5; i++)
            {
                bool useA = i == 1 || i == 3;

                var obj = ObjectPool.Instance.GetObject(names[useA ? indexA : indexB]);
                obj.transform.position = positions[i];

                var spawnable = obj.GetComponent<ISpawnable>();
                allSpawned.Add(spawnable);
            }
        }
    }
}
