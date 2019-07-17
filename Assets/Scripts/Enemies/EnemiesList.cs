using UnityEngine;

namespace SD.Enemies
{
    class EnemiesList : ScriptableObject
    {
        [SerializeField]
        string[] enemies;

        /// <summary>
        /// Get enemies' names
        /// to get them from object pool
        /// </summary>
        public string[] Enemies => enemies;
    }
}
