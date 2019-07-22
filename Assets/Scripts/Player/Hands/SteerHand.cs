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
        Player player;

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
            // only if ready, otherwise dont sample animation,
            // as steering hand is used
            if (player.State == PlayerState.Ready)
            {
                // get steering from steering wheel
                // and then sample hand animation
                steerState.enabled = true;
                steerState.weight = 1.0f;
                steerState.normalizedTime = steeringWheel.SteeringNormalized;

                steerHandAnimation.Sample();
            }
            else if (steerHandAnimation.isPlaying)
            {
                steerHandAnimation.Stop();
            }
        }
    }
}