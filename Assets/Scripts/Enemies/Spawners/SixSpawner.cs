using System.Collections.Generic;
using UnityEngine;

namespace SD.Enemies.Spawner
{
    class SixSpawner : ISpawner
    {
        const float Length = 30;
        const float Safe = 60;

        // what enemies to spawn
        readonly string[] names = {
             SpawnersController.EnemySedan,
             SpawnersController.EnemyBike
        };

        public float Distance => Length + Safe;

        public void Spawn(Vector3 position, Transform player, Vector2 bounds, ICollection<ISpawnable> allSpawned)
        {
            Vector3[] positions = new Vector3[6];
            for (int i = 0; i < 6; i++)
            {
                positions[i] = position;
            }

            // x
            positions[0].x = positions[3].x = bounds[0] + 1.5f;
            positions[2].x = positions[5].x = bounds[1] - 1.5f;

            // z
            positions[3].z = positions[4].z = positions[5].z = position.z + Length;

            bool useChessLayout = Random.Range(0.0f, 1.0f) > 0.75f;

            int indexA = Random.Range(0, names.Length);
            int indexB = Random.Range(0, names.Length);

            for (int i = 0; i < 6; i++)
            {
                bool useA = useChessLayout ?
                    i == 0 || i == 2 || i == 4 :
                    i == 0 || i == 1 || i == 2;

                var obj = ObjectPool.Instance.GetObject(names[useA ? indexA : indexB]);
                obj.transform.position = positions[i];

                var spawnable = obj.GetComponent<ISpawnable>();
                allSpawned.Add(spawnable);
            }
        }
    }
}
