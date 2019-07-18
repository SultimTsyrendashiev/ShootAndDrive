using System;
using System.Collections;
using UnityEngine;

namespace SD.PlayerLogic
{
    public class CameraShaker : MonoBehaviour
    {
        public enum CameraAnimation
        {
            //SmallCollision,
            Collision,
            Explosion,
            Damage,
            Death
        }


        const float MagnitudeEpsilon = 0.01f;


        [SerializeField]
        Transform cameraParent;
        [SerializeField]
        float maxAngle = 2;
        [SerializeField]
        bool smooth;
        [SerializeField]
        float smoothness;
        [SerializeField]
        int axis = 0;


        Animation cameraAnimation;
        //[SerializeField]
        //string smallCollisionAnimationName;
        [SerializeField]
        string collisionAnimationName;
        [SerializeField]
        string damageAnimationName;
        [SerializeField]
        string explosionAnimationName;
        [SerializeField]
        string deathAnimationName;

        bool isShaking;

        float shakeMagnitude;
        float shakeDuration;
        float shakePercentage;

        float initMagnitude;
        float initDuration;


        public static CameraShaker Instance { get; private set; }


        void Start()
        {
            Instance = this;

            Debug.Assert(cameraParent != null, "Camera's parent is not set", this);
            Debug.Assert(cameraParent.localEulerAngles.sqrMagnitude < 0.01f, "Camera's parent must have default rotation", cameraParent);
            Debug.Assert(axis >= 0 && axis < 3, "Shaking axis must be in [0..2]", this);

            cameraAnimation = GetComponentInChildren<Animation>(true);
            Debug.Assert(cameraAnimation != null, "There must be a camera animation", this);

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

        public void PlayAnimation(CameraAnimation type)
        {
            string animName = null;

            switch (type)
            {
                //case CameraAnimation.SmallCollision:
                //    animName = smallCollisionAnimationName;
                //    break;
                case CameraAnimation.Collision:
                    animName = collisionAnimationName;
                    break;

                case CameraAnimation.Damage:
                    // ignore if some other animation is playing
                    if (cameraAnimation.isPlaying)
                    {
                        return;
                    }

                    animName = damageAnimationName;
                    break;

                case CameraAnimation.Explosion:
                    // ignore if some other animation is playing
                    if (cameraAnimation.isPlaying)
                    {
                        return;
                    }

                    animName = explosionAnimationName;
                    break;

                case CameraAnimation.Death:
                    animName = deathAnimationName;
                    break;

                default:
                    Debug.Log("Wrong camera animation enum", this);
                    return;
            }

            // reset to start
            if (cameraAnimation.isPlaying)
            {
                cameraAnimation[animName].time = 0;
            }

            cameraAnimation.Play(animName);
        }
    }
}
