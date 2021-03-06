﻿using UnityEngine;
using SD.Background;

namespace SD
{
    /// <summary>
    /// Background block, which is alligned with axes
    /// </summary>
    interface IBackgroundBlock
    {
        float Length { get; }
        Vector3 Center { get; set; }

        /// <summary>
        /// For block allocating
        /// </summary>
        GameObject CurrentObject { get; }

        /// <summary>
        /// Get block's horizontal bounds in world space
        /// </summary>
        /// <returns>
        /// x is a left bound,
        /// y is a right bound
        /// </returns>
        Vector2 GetHorizontalBounds();

        /// <summary>
        /// Does this block contain a point?
        /// </summary>
        bool Contains(Vector3 position);

        /// <summary>
        /// Get min on z axis
        /// </summary>
        float GetMinZ();

        /// <summary>
        /// Get max on z axis
        /// </summary>
        float GetMaxZ();

        BackgroundBlockType BlockType { get; }
    }
}
