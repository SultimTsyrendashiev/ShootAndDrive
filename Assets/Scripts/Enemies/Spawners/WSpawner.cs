using System.Collections.Generic;
using UnityEngine;

namespace SD.Enemies.Spawner
{
    class WSpawner : ISpawner
    {
        const float Length = 20;
        const float Safe = 60;

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

            // x
            positions[0].x = bounds[0] + 1.5f;
            positions[1].x = (positions[0].x + positions[2].x) / 2;

            positions[4].x = bounds[1] - 1.5f;
            positions[3].x = (positions[4].x + positions[2].x) / 2;

            // z
            positions[0].z = positions[2].z = positions[4].z = position.z + Length;

            bool useLineLayout = Random.Range(0.0f, 1.0f) > 0.5f;

            int indexA = Random.Range(0, names.Length);
            int indexB = Random.Range(0, names.Length);

            for (int i = 0; i < 5; i++)
            {
                bool useA = useLineLayout ?
                    i == 0 || i == 2 || i == 4 :
                    i != 2;

                var obj = ObjectPool.Instance.GetObject(names[useA ? indexA : indexB]);
                obj.transform.position = positions[i];

                var spawnable = obj.GetComponent<ISpawnable>();
                allSpawned.Add(spawnable);
            }
        }
    }
}
