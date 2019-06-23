using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    public static class Autoaim
    {
        /// <summary>
        /// Find object using sphere cast, assuming that there is no objects between result and source
        /// </summary>
        /// <param name="radius">radius of sphere cast</param>
        /// <param name="aimedDirection">result</param>
        /// <param name="mask">layers to find in</param>
        /// <returns>true, if anything is found</returns>
        public static bool Aim(Vector3 from, Vector3 direction, float radius, out Vector3 aimedDirection, int mask)
        {
            RaycastHit hit;

            if (Physics.SphereCast(from, radius, direction, out hit, mask))
            {
                aimedDirection = hit.point - from;
                return true;
            }

            aimedDirection = new Vector3();
            return false;
        }
    }
}
