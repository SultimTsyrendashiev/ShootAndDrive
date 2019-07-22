using UnityEngine;

namespace SD
{
    interface ISpawnable : IPooledObject
    {
        Vector3 Position { get; }

        /// <summary>
        /// Get extents of AABB for checking intersections on spawn
        /// </summary>
        void GetExtents(out Vector3 min, out Vector3 max);

        /// <summary>
        /// Return to pool
        /// </summary>
        void Return();
    }
}
