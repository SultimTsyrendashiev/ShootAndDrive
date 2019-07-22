using UnityEngine;

namespace SD
{
    interface IAttackable
    {
        /// <summary>
        /// Reinit this entity to start attack
        /// </summary>
        void Reinit();
        /// <summary>
        /// Set target for this entity
        /// </summary>
        void SetTarget(Transform target);
    }
}
