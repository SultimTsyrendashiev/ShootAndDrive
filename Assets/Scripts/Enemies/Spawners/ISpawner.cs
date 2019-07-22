using System.Collections.Generic;
using UnityEngine;

namespace SD
{
    interface ISpawner
    {
        /// <summary>
        /// Distance that will be used for spawning.
        /// After it (and some more, which is specified in SpawnersController) 
        /// new spawner will be enabled
        /// </summary>
        float Distance { get; }

        /// <summary>
        /// Spawn specified enemies
        /// </summary>
        /// <param name="names">enemies' names in object pool</param>
        /// <param name="position">where to start</param>
        /// <param name="player">player's transform, used for forward and right vectors</param>
        /// <param name="bounds">bounds of background block; enemies must be spawned in this bounds</param>
        /// <returns></returns>
        void Spawn(Vector3 position, Transform player, Vector2 bounds, ICollection<ISpawnable> allSpawned);
    }
}
