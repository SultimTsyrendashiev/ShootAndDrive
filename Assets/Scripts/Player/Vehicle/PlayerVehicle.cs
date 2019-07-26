using System.Collections;
using System.Collections.Generic;
using SD.Background;
using UnityEngine;

namespace SD.PlayerLogic
{
    [RequireComponent(typeof(Collider))]
    class PlayerVehicle : MonoBehaviour, IVehicle, IDamageable
    {
        public int      MaxHealth = 1000;
        // percentage of health when to start playing smoke particle system
        const float     PlaySmokeHealthPercentage = 0.2f;
        const float     SteeringEpsilon = 0.01f;

        public event    FloatChange OnVehicleHealthChange;
        public event    FloatChange OnDistanceChange;

        Transform       playerTransform;
        Rigidbody       playerRigidbody;

        public float    DefaultSpeed = 20;
        public float    SideSpeed = 10;

        //List<PlayerVehicleDamageReceiver> damageReceivers;
        PlayerVehicleDamageReceiver damageReceiver;
        public event CollideVehicle OnVehicleCollision;

        // to check horizontal bounds
        IBackgroundController background;

        [SerializeField]
        float currentSpeed;
        float currentSideSpeed;

        #region particles
        [SerializeField]
        string HitParticlesName = "Sparks";
        //[SerializeField]
        //string SmokeParticlesName = "VehicleSmoke";

        [SerializeField]
        ParticleSystem engineSmoke;
        #endregion

        // approximate vechicle collider
        Collider apxVehicleCollider;

        public float Health { get; private set; }
        public float TravelledDistance { get; private set; }
        public ISteeringWheel SteeringWheel { get; private set; }

        public Player Player { get; private set; }

        public void Init(Player player)
        {
            Player = player;

            playerTransform = Player.transform;
            playerRigidbody = Player.GetComponent<Rigidbody>();
            playerRigidbody.isKinematic = true;

            SteeringWheel = GetComponentInChildren<SteeringWheel>(true);
      
            // default values
            currentSpeed = DefaultSpeed;
            currentSideSpeed = SideSpeed;
            TravelledDistance = 0;

            // init damage receiver
            Health = MaxHealth;
            damageReceiver = GetComponentInChildren<PlayerVehicleDamageReceiver>(true);
            damageReceiver.Init(this);
        }

        void Start()
        {
            background = FindObjectOfType<BackgroundController>();
        }

        public void ReceiveDamage(Damage damage)
        {
            Health -= damage.CalculateDamageValue(playerTransform.position);

            // play particle system
            if (damage.Type == DamageType.Bullet)
            {
                ParticlesPool.Instance.Play(HitParticlesName, damage.Point, Quaternion.LookRotation(damage.Normal));
            }

            if (Health <= PlaySmokeHealthPercentage)
            {
                engineSmoke.Play();
            }

            if (Health <= 0)
            {
                Health = 0;
                StartCoroutine(BreakVehicle());
            }

            OnVehicleHealthChange(Health);
        }

        // this should be called only from other vehicles
        void IVehicle.Collide(IVehicle otherVehicle, VehicleCollisionInfo info)
        {
            float damage = info.Damage;

            // receive damage
            if (damage > 0)
            {
                // receive damage
                ReceiveDamage(Damage.CreateBulletDamage(
                    damage, -transform.forward, transform.position, transform.up, null));

            }

            // call event even if damage == 0
            OnVehicleCollision(otherVehicle, damage);
        }

        //public void GetGlobalExtents(out Vector3 globalMin, out Vector3 globalMax)
        //{
        //    globalMin = apxVehicleCollider.bounds.min;
        //    globalMax = apxVehicleCollider.bounds.max;

        //    globalMin += transform.position;
        //    globalMin += transform.position;
        //}

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

                Vector2 backgroundBounds = background.GetBlockBounds(newPosition);

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

            TravelledDistance += forwardDistance;
            OnDistanceChange(TravelledDistance);
        }
    }
}
