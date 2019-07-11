using UnityEngine;

namespace SD
{
    /// <summary>
    /// Inherited classes can be placed to object pool
    /// </summary>
    interface IPooledObject
    {
        /// <summary>
        /// Current object in a scene
        /// </summary>
        GameObject ThisObject { get; }
        PooledObjectType Type { get; }
        /// <summary>
        /// How many objects to allocate
        /// on initialization of object pool
        /// </summary>
        int AmountInPool { get; }

        /// <summary>
        /// Called on initialization in object pool
        /// </summary>
        void Init();
        /// <summary>
        /// Called on getting from object pool
        /// </summary>
        void Reinit();
        
        // NOTE: this method is unnecessary
        // as deactivating object returns it to pool
        // void ReturnToPool();
    }
}
