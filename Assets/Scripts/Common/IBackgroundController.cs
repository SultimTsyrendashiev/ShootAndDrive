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
    }
}
