using System.Collections;
using System.Collections.Generic;
using SD.Vehicles;
using UnityEngine;

public class SteeringWheel : MonoBehaviour
{
    const float MaxAngle = 130.0f;
    const float Threshold = 0.1f;

    [SerializeField]
    private float lerpSpeed = 10.0f;
    [SerializeField]
    private IVehicle vehicle;
    [SerializeField]
    private Transform rotator;

    private float steering;
    private float steeringNormalized;
    private float targetSteering;

    public float SteeringNormalized => steeringNormalized;

    void Start()
    {
        Debug.Assert(rotator != null);

        steering = 0.0f;
        steeringNormalized = 0.5f;
        targetSteering = 0.0f;
    }

    public void Steer(float steeringInput)
    {
        targetSteering = Mathf.Clamp(steeringInput, -1.0f, 1.0f);
    }

    void Update()
    {
        steering = Mathf.Lerp(steering, targetSteering, lerpSpeed * Time.deltaTime);

        // normalize steering to [0..1]
        steeringNormalized = steering * 0.5f + 0.5f;

        // set steering wheel rotation
        Vector3 angle = new Vector3(0, 0, Mathf.Lerp(-MaxAngle, MaxAngle, steeringNormalized));
        rotator.localEulerAngles = angle;
    }
}
