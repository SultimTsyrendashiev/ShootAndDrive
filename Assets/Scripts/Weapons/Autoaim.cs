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
            Transform target = GetTarget(from, direction, radius, range, mask);

            if (target != null)
            {
                aimedDirection = (target.position - from).normalized;
                return true;
            }

            aimedDirection = direction.normalized;
            return false;
        }

        public static Transform GetTarget(Vector3 from, Vector3 direction, float radius, float range, int mask)
        {
            if (Physics.SphereCast(from, radius, direction, out RaycastHit hit, range, mask))
            {
                return hit.collider.transform;
            }

            return null;
        }

        public static bool AimMissile(Transform spawn, Vector3 target, float maxVelocity, out float velocity, float angle = 20)
        {
            Vector3 diff = target - spawn.position;

            // rotate spawn transform,
            // so its forward vector points to target
            Vector3 newForward = diff;
            newForward.y = 0;

            spawn.forward = newForward;

            // also, rotate in world coordinates on some angle to up
            Vector3 euler = spawn.eulerAngles;
            euler.x = -angle; // angle is inverted

            spawn.eulerAngles = euler;

            // now all we need is to calculate start velocity (float);

            // spawn transform is rotated to target,
            // so we can consider 2 dimensional case

            // get parameters
            float L = diff.z; // forward component
            float h = diff.y; // up component

            // assume, that gravity points down
            float g = Physics.gravity.y;

            // if target is behind of a spawn
            if (L <= 0)
            {
                velocity = maxVelocity;
                return false;
            }

            float s = Mathf.Sin(2 * angle);
            float c = Mathf.Pow(Mathf.Cos(angle), 2);

            float v = L * Mathf.Sqrt(-g / (L * s - 2 * h * c));
            
            if (float.IsNaN(v) || float.IsInfinity(v))
            {
                velocity = maxVelocity;
                return false;
            }

            // clamp
            if (v > maxVelocity)
            {
                velocity = maxVelocity;
                return false;
            }

            velocity = v;
            return true;
        }
    }
}
