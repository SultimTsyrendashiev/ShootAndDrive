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

            Debug.Assert(player != null, "Player is not set", this);
            Debug.Assert(player.Vehicle != null, "Player's vehicle is not set", this);

            // set this hand as a child for the vehicle;
            // used for proper vehicle rotation (i.e. with steering hand)
            transform.SetParent(player.Vehicle.RotatingTransform, true);
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