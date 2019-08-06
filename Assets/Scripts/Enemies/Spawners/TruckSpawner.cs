using System.Collections.Generic;
using UnityEngine;

namespace SD.Enemies.Spawner
{
    class TruckSpawner : ISpawner
    {
        const float Safe = 50;

        public float Distance => Safe;

        public void Spawn(Vector3 position, Transform player, Vector2 bounds, ICollection<ISpawnable> allSpawned)
        {
            Vector3 pos = position;
            pos.x = Random.Range(bounds[0] + 1.5f, bounds[1] - 1.5f);

            var obj = ObjectPool.Instance.GetObject(SpawnersController.EnemyTruck);
            obj.transform.position = pos;

            var spawnable = obj.GetComponent<ISpawnable>();
            allSpawned.Add(spawnable);
        }
    }
}
