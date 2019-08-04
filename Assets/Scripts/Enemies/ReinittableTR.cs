using System.Collections;
using UnityEngine;

namespace SD.Enemies
{
    class ReinittableTR
    {
        Transform       parent;

        Transform[]     transforms;
        Rigidbody[]     rigidbodies;

        // default local positions and rotations;
        // used for resetting to default
        Vector3[]       defaultPositions;
        Quaternion[]    defaultRotations;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="parent">this transform will not be reset to default</param>
        public ReinittableTR(Transform parent, Transform[] transforms, Rigidbody[] rigidbodies)
        {
            this.parent = parent;
            this.transforms = transforms;
            this.rigidbodies = rigidbodies;
        }

        /// <summary>
        /// Load positions and rotations from transforms
        /// </summary>
        public void Load()
        {
            defaultPositions = new Vector3[transforms.Length];
            defaultRotations = new Quaternion[transforms.Length];

            for (int i = 0; i < transforms.Length; i++)
            {
                defaultPositions[i] = transforms[i].localPosition;
                defaultRotations[i] = transforms[i].localRotation;
            }
        }

        /// <summary>
        /// Load positions and rotations from specified transforms
        /// </summary>
        public void Load(Transform[] newTransforms)
        {
            defaultPositions = new Vector3[newTransforms.Length];
            defaultRotations = new Quaternion[newTransforms.Length];

            for (int i = 0; i < newTransforms.Length; i++)
            {
                defaultPositions[i] = newTransforms[i].localPosition;
                defaultRotations[i] = newTransforms[i].localRotation;
            }
        }

        /// <summary>
        /// Load positions and rotations from specified ones
        /// </summary>
        /// <param name="positions">local positions for each transform</param>
        /// <param name="rotations">local positions for each transform</param>
        public void Load(Vector3[] positions, Quaternion[] rotations)
        {
            defaultPositions = new Vector3[transforms.Length];
            defaultRotations = new Quaternion[transforms.Length];

            for (int i = 0; i < transforms.Length; i++)
            {
                defaultPositions[i] = positions[i];
                defaultRotations[i] = rotations[i];
            }
        }

        /// <summary>
        /// Pose to default
        /// </summary>
        public void ReinitTransforms()
        {
            Debug.Assert(defaultPositions != null && defaultRotations != null, "Default values are not set", parent);

            // reset to default, except this transform
            for (int i = 0; i < transforms.Length; i++)
            {
                var t = transforms[i];

                // don't affect main parent
                if (t != parent)
                {
                    t.localPosition = defaultPositions[i];
                    t.localRotation = defaultRotations[i];
                }
            }
        }

        /// <summary>
        /// Pose to default
        /// </summary>
        public void ReinitTransforms(Transform[] newTransforms)
        {
            Debug.Assert(defaultPositions != null && defaultRotations != null, "Default values are not set", parent);

            // reset to default, except this transform
            for (int i = 0; i < transforms.Length; i++)
            {
                var t = transforms[i];
                var nt = newTransforms[i];

                // don't affect main parent
                if (t != parent)
                {
                    t.localPosition = nt.localPosition;
                    t.localRotation = nt.localRotation;
                }
            }
        }

        /// <summary>
        /// Set rigidbody's velocity
        /// </summary>
        public void ReinitRigidbodies(Vector3 velocity, Vector3 angularVelocity)
        {
            foreach (var rb in rigidbodies)
            {
                // set init velocities
                rb.velocity = velocity;
                rb.angularVelocity = angularVelocity;
            }
        }
    }
}
