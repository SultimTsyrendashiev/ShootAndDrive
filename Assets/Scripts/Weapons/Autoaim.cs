using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SD.Weapons
{
    public static class Autoaim
    {        
        /// <summary>
        /// Find object using sphere cast, assuming that there is no objects between result and source.
        /// If nothing to aim, "aimedDirection" will be equal to "direction"
        /// </summary>
        /// <param name="radius">radius of sphere cast</param>
        /// <param name="aimedDirection">result</param>
        /// <param name="mask">layers to find in</param>
        /// <returns>true, if anything is found</returns>
        public static bool Aim(Vector3 from, Vector3 direction, float radius, out Vector3 aimedDirection, float range, int mask)
        {
            RaycastHit hit;

            if (Physics.SphereCast(from, radius, direction, out hit, range, mask))
            {
                aimedDirection = (hit.transform.position - from).normalized;
                return true;
            }

            aimedDirection = direction.normalized;
            return false;
        }
    }
}
