using System.Collections.Generic;
using UnityEngine;

namespace SD.Enemies.Spawner
{
    class VanSpawner : ISpawner
    {
        const float DistanceBetween = 30;
        const float Safe = 130;

        public float Distance => (amount - 1) * DistanceBetween + Safe;

        int amount;

        public void Spawn(Vector3 position, Transform player, Vector2 bounds, ICollection<ISpawnable> allSpawned)
        {
            amount = Random.Range(1, 3);

            for (int i = 0; i < amount; i++)
            {
                Vector3 pos = position;
                pos.z += i * DistanceBetween;
                pos.x = Random.Range(bounds[0] + 1.5f, bounds[1] - 1.5f);

                var obj = ObjectPool.Instance.GetObject(SpawnersController.EnemyVan);
                obj.transform.position = pos;

                var spawnable = obj.GetComponent<ISpawnable>();
                allSpawned.Add(spawnable);
            }
        }
    }
}
