using UnityEngine;

namespace SD
{
    interface IBackgroundController
    {
        /// <summary>
        /// Remove all background blocks from the scene
        /// </summary>
        void Reinit();

        ///// <summary>
        ///// Remove specified background blocks from the scene
        ///// </summary>
        ///// <param name="ignoreCutsceneBlocks">if true, cutscene background blocks will not be removed</param>
        //void Reinit(bool ignoreCutsceneBlocks);

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
        /// <param name="target">target to track. 
        /// If null, background controller will not delete or create blocks</param>
        void SetTarget(Transform target);

        /// <summary>
        /// Create special background for cutscene
        /// </summary>
        /// <param name="position">position of cutscene background</param>
        void CreateCutsceneBackground(Vector3 position);

        /// <summary>
        /// Is this box is out of bounds?
        /// Note: checks only oldest block
        /// </summary>
        bool IsOut(Vector3 min, Vector3 max);
    }
}
