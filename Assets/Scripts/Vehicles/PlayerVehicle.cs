using UnityEngine;
using System.Collections;
using SD.Background;

namespace SD.PlayerLogic
{
    delegate void CollideVehicle(IVehicle player, IVehicle other);

    [RequireComponent(typeof(Collider))]
    class PlayerVehicle : MonoBehaviour, IVehicle, IDamageable
    {
        const int MaxHealth = 100;

        const float SteeringEpsilon = 0.01f;

        public static event FloatChange OnVehicleHealthChange;
        public static event FloatChange OnDistanceChange;

        Player          player;
        Transform       playerTransform;
        Rigidbody       playerRigidbody;

        public float    Speed = 20;
        public float    SideSpeed = 10;
        public event    CollideVehicle OnVehicleCollision;

        float currentSpeed;
        float currentSideSpeed;
        float travelledDistance;

        public float Health { get; private set; } = MaxHealth;
        public ISteeringWheel SteeringWheel { get; private set; }

        void Awake()
        {
            SteeringWheel = GetComponentInChildren<SteeringWheel>(true);
        }

        void Start()
        {
            player = GetComponentInParent<Player>();

            playerTransform = player.transform;
            playerRigidbody = player.GetComponent<Rigidbody>();
            playerRigidbody.isKinematic = true;

            currentSpeed = Speed;
            currentSideSpeed = SideSpeed;
            travelledDistance = 0;

            OnVehicleHealthChange(Health);
            OnDistanceChange(travelledDistance);
        }

        public void ReceiveDamage(Damage damage)
        {
            Health -= damage.CalculateDamageValue(transform.position);
            OnVehicleHealthChange(Health);

            if (Health <= 0)
            {
                StartCoroutine(BreakVehicle());
            }
        }

        IEnumerator BreakVehicle()
        {
            const float speedEpsilon = 0.1f;
            const float lerpMultiplier = 2;

            while (true)
            {
                if (currentSpeed < speedEpsilon && currentSideSpeed < speedEpsilon)
                {
                    currentSpeed = 0;
                    currentSideSpeed = 0;

                    yield break;
                }

                currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * lerpMultiplier);
                currentSideSpeed = Mathf.Lerp(currentSideSpeed, 0, Time.deltaTime * lerpMultiplier);

                yield return null;
            }
        }

        public void FixedUpdate()
        {
            if (currentSpeed == 0 && currentSideSpeed == 0)
            {
                return;
            }

            // get data from steering wheel
            float steering = SteeringWheel.Steering;

            // change position of the vehicle
            Vector3 newPosition = playerRigidbody.position;

            float forwardDistance = currentSpeed * Time.fixedDeltaTime;
            newPosition += playerTransform.forward * forwardDistance;

            if (Mathf.Abs(steering) > SteeringEpsilon)
            {
                newPosition += playerTransform.right * steering * currentSideSpeed * Time.fixedDeltaTime;

                Vector2 backgroundBounds = BackgroundController.Instance.GetCurrentBounds(newPosition);

                if (newPosition.x > backgroundBounds[1])
                {
                    newPosition.x = backgroundBounds[1];
                }
                else if (newPosition.x < backgroundBounds[0])
                {
                    newPosition.x = backgroundBounds[0];
                }
            }

            playerRigidbody.MovePosition(newPosition);

            travelledDistance += forwardDistance;
            OnDistanceChange(travelledDistance);
        }

        public void OnCollisionEnter(Collision col)
        {
            IVehicle otherVehicle = col.gameObject.GetComponent<IVehicle>();

            if (otherVehicle != null)
            {
                OnVehicleCollision(this, otherVehicle);
            }
        }
    }
}
