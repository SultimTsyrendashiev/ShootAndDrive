using UnityEngine;

namespace SD.PlayerLogic
{
    [RequireComponent(typeof(Animation))]
    public class SteerHand : MonoBehaviour
    {
        const string SteerAnimationName = "SteeringWheelHands";
        const string EnableAnimationName = "LeftHandEnable";

        /// <summary>
        /// Transform for finding steering wheel
        /// </summary>
        [SerializeField]
        Player player;

        ISteeringWheel steeringWheel;
        Animation steerHandAnimation;
        AnimationState steerState;

        bool canBeUpdated;
        float startTime;

        void Awake()
        {
            steeringWheel = player.GetComponentInChildren<ISteeringWheel>();
            steerHandAnimation = GetComponent<Animation>();
            steerState = steerHandAnimation[SteerAnimationName];

            Debug.Assert(player != null, "Player is not set", this);
            Debug.Assert(player.Vehicle != null, "Player's vehicle is not set", this);

            // set this hand as a child for the vehicle;
            // used for proper vehicle rotation (i.e. with steering hand)
            transform.SetParent(player.Vehicle.RotatingTransform, true);

            canBeUpdated = false;

            player.OnPlayerStateChange += ChangeState;
        }

        void ChangeState(PlayerState state)
        {
            canBeUpdated = false;
            steerHandAnimation.Stop();

            switch (state)
            {
                case PlayerState.Ready:
                    // if ready, start animation
                    EnableAnimation();

                    return;
                default:
                    return;
            }
        }

        void OnEnable()
        {
            canBeUpdated = false;
            EnableAnimation();
        }

        void EnableAnimation()
        {
            steerHandAnimation.Stop();

            steerHandAnimation.Play(EnableAnimationName, PlayMode.StopAll);
            startTime = Time.time + steerHandAnimation[EnableAnimationName].length;
        }

        void LateUpdate()
        {
            if (Time.time < startTime)
            {
                return;
            }
            else
            {
                canBeUpdated = true;
            }


            // only if ready, otherwise dont sample animation,
            // as steering hand is used
            if (canBeUpdated)
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
}