using UnityEngine;
using SD.PlayerLogic;

namespace SD.Vehicles
{
    delegate void CollideVehicle(IVehicle player, IVehicle other);

    [RequireComponent(typeof(Collider))]
    class PlayerVehicle : MonoBehaviour, IVehicle, IDamageable
    {
        Player          player;
        Transform       playerTransform;
        Rigidbody       playerRigidbody;

        public float    Speed = 20;
        public float    SideSpeed = 10;
        public event    CollideVehicle OnVehicleCollision;

        public float Health { get; private set; }
        public ISteeringWheel SteeringWheel { get; private set; }

        void Start()
        {
            SteeringWheel = GetComponentInChildren<SteeringWheel>(true);

            player = GetComponentInParent<Player>();

            playerTransform = player.transform;
            playerRigidbody = player.GetComponent<Rigidbody>();
            playerRigidbody.isKinematic = true;
        }

        public void ReceiveDamage(Damage damage)
        {
            Health -= damage.CalculateDamageValue(transform.position);
        }

        public void FixedUpdate()
        {
            // get data from steering wheel
            float steering = SteeringWheel.Steering;

            // change position of the vehicle
            Vector3 newPosition = playerRigidbody.position;
            newPosition += playerTransform.forward * Speed * Time.fixedDeltaTime;
            newPosition += playerTransform.right * steering * SideSpeed * Time.fixedDeltaTime;

            playerRigidbody.MovePosition(newPosition);
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
