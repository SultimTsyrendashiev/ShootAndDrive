using UnityEngine;

namespace SD.ObjectPooling
{
    [CreateAssetMenu(menuName = "Particles Pool Prefabs Data", order = 51)]
    class ParticlesPoolPrefabs : ScriptableObject
    {
        public GameObject[] Prefabs;
    }
}
