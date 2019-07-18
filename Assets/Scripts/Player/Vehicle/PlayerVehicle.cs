using System.Collections;
using System.Collections.Generic;
using SD.Background;
using UnityEngine;

namespace SD.PlayerLogic
{
    delegate void CollideVehicle(IVehicle other, float damage);

    class PlayerVehicle : MonoBehaviour, IVehicle, IDamageable
    {
        public const int MaxHealth = 1000;
        const float SteeringEpsilon = 0.01f;

        public event FloatChange OnVehicleHealthChange;
        public event FloatChange OnDistanceChange;

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
        float travelledDistance;

        #region particles
        [SerializeField]
        string HitParticlesName = "Sparks";
        [SerializeField]
        string SmokeParticlesName = "VehicleSmoke";
        [SerializeField]
        Transform EngineSmokeTransform;
        #endregion

        public float Health { get; private set; } = MaxHealth;
        public ISteeringWheel SteeringWheel { get; private set; }

        public Player Player { get; private set; }

        public void Init(Player player, IBackgroundController background)
        {
            Player = player;
            this.background = background;

            playerTransform = Player.transform;
            playerRigidbody = Player.GetComponent<Rigidbody>();
            playerRigidbody.isKinematic = true;

            SteeringWheel = GetComponentInChildren<SteeringWheel>(true);
      
            // default values
            currentSpeed = DefaultSpeed;
            currentSideSpeed = SideSpeed;
            travelledDistance = 0;

            // init damage receiver
            damageReceiver = GetComponentInChildren<PlayerVehicleDamageReceiver>(true);
            damageReceiver.Init(this);


            // init damage receivers
            //var cs = GetComponentsInChildren<Collider>(true);
            //damageReceivers = new List<PlayerVehicleDamageReceiver>();

            //foreach (var c in cs)
            //{
            //    var dr = c.GetComponent<PlayerVehicleDamageReceiver>();

            //    if (dr == null)
            //    {
            //        // if it's pickup receiver then ignore it
            //        if (c.GetComponent<PlayerPickupReceiver>())
            //        {
            //            continue;
            //        }

            //        Debug.Assert(dr != null, "This collider must contain PlayerVehicleDamageReceiver component", c);
            //    }

            //    dr.Init(this);
            //    damageReceivers.Add(dr);
            //}
        }

        public void ReceiveDamage(Damage damage)
        {
            Health -= damage.CalculateDamageValue(playerTransform.position);

            // play particle system
            if (damage.Type == DamageType.Bullet)
            {
                ParticlesPool.Instance.Play(HitParticlesName, damage.Point, Quaternion.LookRotation(damage.Normal));
            }

            if (Health <= 0)
            {
                Health = 0;
                StartCoroutine(BreakVehicle());

                // play smoke particle system
                ParticlesPool.Instance.Play(SmokeParticlesName, EngineSmokeTransform.position, EngineSmokeTransform.rotation);
            }

            OnVehicleHealthChange(Health);
        }

        void IVehicle.Collide(IVehicle otherVehicle, VehicleCollisionInfo info)
        {
            print("PlayerVehicle Collide");

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

            travelledDistance += forwardDistance;
            OnDistanceChange(travelledDistance);
        }
    }
}
