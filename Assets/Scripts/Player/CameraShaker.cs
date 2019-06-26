using System;
using System.Collections;
using UnityEngine;

namespace SD.Player
{
    public class CameraShaker : MonoBehaviour
    {
        [SerializeField]
        private Transform cameraParent;

        private int axis = 0;

        private static CameraShaker instance;
        public static CameraShaker Instance { get { return instance; } }

        void Start()
        {
            instance = this;

            Debug.Assert(cameraParent != null);
            Debug.Assert(cameraParent.localEulerAngles.sqrMagnitude < 0.01f);
            Debug.Assert(axis >= 0 && axis < 3);
        }

        public void Shake(float power, float duration)
        {
            StartCoroutine(ProcessShake(power, duration));
        }

        IEnumerator ProcessShake(float power, float duration)
        {
            cameraParent.localEulerAngles = Vector3.zero;

            float timer = 0.0f;
            Vector3 euler = Vector3.zero;

            float upTime = duration * 0.35f;
            float downTime = duration - upTime;

            // up
            while (timer < upTime)
            {
                euler[axis] = Mathf.Lerp(0, power, timer / upTime);
                timer += Time.deltaTime;

                // wait one frame
                yield return null;
            }

            timer -= upTime;

            // down
            while (timer < downTime)
            {
                euler[axis] = Mathf.Lerp(power, 0, timer / downTime);
                timer += Time.deltaTime;

                // wait one frame
                yield return null;
            }

            cameraParent.localEulerAngles = Vector3.zero;
        }
    }
}
