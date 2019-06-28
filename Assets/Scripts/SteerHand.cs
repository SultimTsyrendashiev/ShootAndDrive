using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animation))]
public class SteerHand : MonoBehaviour
{
    const string SteerAnimationName = "SteeringWheelHands";

    [SerializeField]
    SteeringWheel steeringWheel;

    Animation steerHandAnimation;
    AnimationState steerState;

    void Start()
    {
        Debug.Assert(steeringWheel != null);

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
