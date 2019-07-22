using UnityEngine;

namespace SD
{
    interface IObjectPool
    {
        /// <summary>
        /// Get object with default rotation and position
        /// </summary>
        GameObject GetObject(string name);

        /// <summary>
        /// Get object with default rotation in specified position
        /// </summary>
        GameObject GetObject(string name, Vector3 position);
        GameObject GetObject(string name, Vector3 position, Vector3 direction);
        GameObject GetObject(string name, Vector3 position, Quaternion rotation);
    }
}
