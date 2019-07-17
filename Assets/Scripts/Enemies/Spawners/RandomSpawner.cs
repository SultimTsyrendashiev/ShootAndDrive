using System.Collections;
using UnityEngine;

namespace SD.Enemies.Spawner
{
    class RandomSpawner : ISpawner
    {
        readonly string[] names = { SpawnersController.EnemySedan };
        const int count = 4;
        const float distanceBetween = 5;

        public float Distance => count * distanceBetween;

        public void Spawn(Vector3 position, Vector3 playerDir, Vector2 bounds)
        {
            Debug.Assert(playerDir.magnitude == 1, "RandomSpawner::Player direction is not normalized");

            Vector3 spawnPos = position;

            for (int i = 0; i < count; i++)
            {
                int index = Random.Range(0, names.Length);
                spawnPos.x = Random.Range(position.x + bounds[0], position.x + bounds[1]);

                ObjectPool.Instance.GetObject(names[index], spawnPos);

                spawnPos += playerDir * distanceBetween;
            }
        }
    }
}
