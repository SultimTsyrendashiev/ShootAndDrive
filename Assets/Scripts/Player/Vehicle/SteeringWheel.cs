using UnityEngine;

namespace SD.PlayerLogic
{
    class SteeringWheel : MonoBehaviour, ISteeringWheel
    {
        const float MaxAngle = 130.0f;

        [SerializeField]
        private float lerpSpeed = 10.0f;
        [SerializeField]
        private Transform rotator;
        [SerializeField]
        private float maxDelta = 0.25f;

        private float targetSteering;

        public float Steering { get; private set; }
        public float SteeringNormalized { get; private set; }

        void Start()
        {
            Debug.Assert(rotator != null);

            Steering = 0.0f;
            SteeringNormalized = 0.5f;
            targetSteering = 0.0f;
        }

        public void Steer(float steeringInput)
        {
            targetSteering = Mathf.Clamp(steeringInput, -1.0f, 1.0f);

            if (targetSteering - Steering > maxDelta)
            {
                targetSteering = Steering + maxDelta;
            }
            else if (Steering - targetSteering > maxDelta)
            {
                targetSteering = Steering - maxDelta;
            }
        }

        public void Restart()
        {
            Steering = 0.0f;
            SteeringNormalized = 0.5f;
            targetSteering = 0;
        }

        public void Stop()
        {
            // to make smooth transition
            // Steering = 0.0f;
            // SteeringNormalized = 0.5f;
            targetSteering = 0;
        }

        void Update()
        {
            Steering = Mathf.Lerp(Steering, targetSteering, lerpSpeed * Time.deltaTime);

            // normalize steering to [0..1]
            SteeringNormalized = Steering * 0.5f + 0.5f;

            // set steering wheel rotation
            Vector3 angle = new Vector3(0, 0, Mathf.Lerp(-MaxAngle, MaxAngle, SteeringNormalized));
            rotator.localEulerAngles = angle;
        }
    }
}
