using System.Collections.Generic;
using UnityEngine;

namespace SD.Enemies.Spawner
{
    class CircleSpawner : ISpawner
    {
        const float Length = 50;
        const float Safe = 70;

        // what enemies to spawn on circle
        readonly string[] circleNames = {
             SpawnersController.EnemyBike
        };

        // what enemy to spawn inside a circle
        readonly string[] insideNames = {
             SpawnersController.EnemySedan,
             //SpawnersController.EnemyCoupe
        };

        public float Distance => Length + Safe;

        public void Spawn(Vector3 position, Transform player, Vector2 bounds, ICollection<ISpawnable> allSpawned)
        {
            int amount = Random.Range(0.0f, 1.0f) > 0.5f ? 5 : 6;

            Vector3[] positions = new Vector3[amount];
            for (int i = 0; i < amount; i++)
            {
                positions[i] = position;
            }

            Debug.Assert(amount == 5 || amount == 6, "Circle spawner musst spawn only 4 or 5 enemies");

            if (amount == 5)
            {
                // center
                positions[0].z += Length / 2;

                bool useRhombus = Random.Range(0.0f, 1.0f) > 0.5f;

                if (useRhombus)
                {
                    // x
                    positions[2].x = bounds[0] + 1.5f;
                    positions[4].x = bounds[1] - 1.5f;

                    // z
                    positions[2].z = positions[4].z = position.z + Length / 2;
                    positions[1].z = position.z + Length;
                }
                else
                {
                    // x
                    positions[1].x = positions[2].x = bounds[0] + 1.5f;
                    positions[3].x = positions[4].x = bounds[1] - 1.5f;

                    // z
                    positions[1].z = positions[4].z = position.z + Length;
                }
            }
            else
            {
                // x
                positions[1].x = positions[2].x = bounds[0] + 1.5f;
                positions[4].x = positions[5].x = bounds[1] - 1.5f;

                // z
                positions[1].z = positions[5].z = position.z + Length;
                positions[2].z = positions[4].z = position.z + Length / 3;

                // center
                positions[0].z = position.z + Length * 2 / 3;
            }


            string nameC = circleNames[Random.Range(0, circleNames.Length)];
            string nameI = insideNames[Random.Range(0, insideNames.Length)];

            for (int i = 0; i < amount; i++)
            {
                bool useC = i != 0; // index of inside position is 0

                var obj = ObjectPool.Instance.GetObject(useC ? nameC : nameI);
                obj.transform.position = positions[i];

                var spawnable = obj.GetComponent<ISpawnable>();
                allSpawned.Add(spawnable);
            }
        }
    }
}
