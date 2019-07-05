using UnityEngine;

namespace SD.ObjectPooling
{
    [CreateAssetMenu(menuName = "Object Pool Prefabs Data", order = 51)]
    class ObjectPoolPrefabs : ScriptableObject
    {
        public GameObject[] Prefabs;
    }
}
