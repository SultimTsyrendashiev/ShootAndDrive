using UnityEngine;

namespace SD
{
    interface IBackgroundController
    {
        float CurrentLength { get; }

        void Reinit();

        /// <summary>
        /// Get containing block's bounds
        /// </summary>
        /// <returns>
        /// x is a left bound,
        /// y is a right bound
        /// </returns>
        Vector2 GetBlockBounds(Vector3 position);

        /// <summary>
        /// Check for current blocks:
        /// if distance between last block position 
        /// and camera position is less than needed,
        /// new blocks will be created.
        /// Also, invisible for this camera ones 
        /// will be deleted
        /// </summary>
        /// <param name="camera position"></param>
        void UpdateTargetPosition(Vector3 cameraPosition);
        /// <summary>
        /// Set target to track
        /// </summary>
        void SetTarget(Transform target);

        /// <summary>
        /// Is this box is out of bounds?
        /// Note: checks only oldest block
        /// </summary>
        bool IsOut(Vector3 min, Vector3 max);
    }
}
