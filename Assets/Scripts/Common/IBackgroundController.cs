using UnityEngine;

namespace SD
{
    interface IBackgroundController
    {
        /// <summary>
        /// Get current block's bounds
        /// </summary>
        /// <returns>
        /// x is a left bound,
        /// y is a right bound
        /// </returns>
        Vector2 GetCurrentBounds(Vector3 position);

        /// <summary>
        /// Check for current blocks:
        /// if distance between last block position 
        /// and camera position is less than needed,
        /// new blocks will be created.
        /// Also, invisible for this camera ones 
        /// will be deleted
        /// </summary>
        /// <param name="camera position"></param>
        void UpdateCameraPosition(Vector3 cameraPosition);
    }
}
