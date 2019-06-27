using System;
using System.Collections;
using UnityEngine;

namespace SD.Player
{
    public class CameraShaker : MonoBehaviour
    {
        const float MagnitudeEpsilon = 0.01f;

        [SerializeField]
        private Transform cameraParent;
        [SerializeField]
        private float maxAngle = 2;
        [SerializeField]
        private bool smooth;
        [SerializeField]
        private float smoothness;

        private int axis = 0;

        private bool isShaking;

        private float shakeMagnitude;
        private float shakeDuration;
        private float shakePercentage;

        private float initMagnitude;
        private float initDuration;

        private static CameraShaker instance;
        public static CameraShaker Instance { get { return instance; } }

        void Start()
        {
            instance = this;

            Debug.Assert(cameraParent != null);
            Debug.Assert(cameraParent.localEulerAngles.sqrMagnitude < 0.01f);
            Debug.Assert(axis >= 0 && axis < 3);

            isShaking = false;
            shakeMagnitude = 0.0f;
            shakeDuration = 0.0f;
        }

        public void Shake(float magnitude, float duration)
        {
            if (Mathf.Abs(shakeMagnitude + magnitude) > maxAngle)
            {
                return;
            }

            isShaking = true;

            shakeMagnitude += magnitude;
            shakeDuration += duration;

            initMagnitude = shakeMagnitude;
            initDuration = shakeDuration;
        }

        void Update()
        {
            if (!isShaking)
            {
                return;
            }

            Vector3 newRotation = UnityEngine.Random.onUnitSphere;
            newRotation.x = -shakeMagnitude;
            newRotation.y = 0;
            newRotation.z = 0;

            shakePercentage = shakeDuration / initDuration;

            shakeMagnitude = initMagnitude * shakePercentage;
            shakeDuration = Mathf.Lerp(shakeDuration, 0, Time.deltaTime / initDuration);

            if (smooth)
            {
                cameraParent.localRotation = Quaternion.Lerp(
                    cameraParent.localRotation, 
                    Quaternion.Euler(newRotation), 
                    Time.deltaTime * smoothness);
            }
            else
            {
                cameraParent.localRotation = Quaternion.Euler(newRotation);
            }

            if (shakeMagnitude < MagnitudeEpsilon)
            {
                shakeMagnitude = 0.0f;
                shakeDuration = 0.0f;
                isShaking = false;
            }
        }
    }
}
