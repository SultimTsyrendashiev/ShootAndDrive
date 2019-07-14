using UnityEngine;

namespace SD.PlayerLogic
{
    [RequireComponent(typeof(Animation))]
    public class SteerHand : MonoBehaviour
    {
        const string SteerAnimationName = "SteeringWheelHands";

        /// <summary>
        /// Transform for finding steering wheel
        /// </summary>
        [SerializeField]
        Transform player;

        ISteeringWheel steeringWheel;
        Animation steerHandAnimation;
        AnimationState steerState;

        void Start()
        {
            steeringWheel = player.GetComponentInChildren<ISteeringWheel>();
            steerHandAnimation = GetComponent<Animation>();
            steerState = steerHandAnimation[SteerAnimationName];
        }

        void LateUpdate()
        {
            // get steering from steering wheel
            // and then sample hand animation
            steerState.enabled = true;
            steerState.weight = 1.0f;
            steerState.normalizedTime = steeringWheel.SteeringNormalized;

            steerHandAnimation.Sample();
        }
    }
}