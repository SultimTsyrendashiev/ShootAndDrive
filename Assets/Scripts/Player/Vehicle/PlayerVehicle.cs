using System.Collections;
using System.Collections.Generic;
using SD.Background;
using UnityEngine;

namespace SD.PlayerLogic
{
    [RequireComponent(typeof(Collider))]
    class PlayerVehicle : MonoBehaviour, IPlayerVehicle
    {
        #region const
        // percentage of health when to start playing smoke particle system
        const float                 SmokeHealthPercentage = 0.2f;
        const float                 FireHealthPercentage = 0.02f;

        // when to call distance change event
        const float                 DistanceUpdateEpsilon = 0.1f;

        const float                 SteeringEpsilon = 0.01f;

        // how long to wait for explosion when health < 0
        const float                 TimeToExplode = 1.8f;

        // dont move vehicle, if speed is less than this value
        const float                 SpeedEpsilon = 0.1f;

        // how long it takes to accelerate from 0 to default speed
        const float                 AccelerationTime = 2.5f;
        // how long it takes to stop, if vehicle has default speed
        const float                 BrakeTime = 2.0f;
        #endregion

        Transform                   playerTransform;
        Rigidbody                   playerRigidbody;


        //List<PlayerVehicleDamageReceiver> damageReceivers;
        PlayerVehicleDamageReceiver damageReceiver;

        public event CollideVehicle OnVehicleCollision;
        public event FloatChange    OnVehicleHealthChange;
        public event FloatChange    OnDistanceChange;
        public event FloatChange    OnSteering;

        #region speed
        public float                DefaultSpeed = 20;
        public float                DefaultSideSpeed = 10;

        [SerializeField]
        float                       currentSpeed;
        float                       currentSideSpeed;

        float                       targetSpeed;
        float                       targetSideSpeed;
        #endregion

        #region particles
        [SerializeField]
        ParticleSystem              hitParticles;
        //[SerializeField]
        //string SmokeParticlesName = "VehicleSmoke";

        [SerializeField]
        ParticleSystem              engineSmoke;
        [SerializeField]
        ParticleSystem              engineFire;
        #endregion

        [SerializeField]
        Transform                   rotatingTransform;
        /// <summary>
        /// Will be rotated when vehicle is steering
        /// </summary>
        public Transform            RotatingTransform => rotatingTransform;

        // approximate vechicle collider
        Collider                    apxVehicleCollider;


        public int                  MaxHealth = 1000;
        public float                Health { get; private set; }

        float                       prevUpdatedDistance = -1;
        public float                TravelledDistance { get; private set; }

        public ISteeringWheel       SteeringWheel { get; private set; }
        public Player               Player { get; private set; }


        /// <summary>
        /// Init this vehicle. Must be called only once
        /// </summary>
        /// <param name="player"></param>
        public void Init(Player player)
        {
            Player = player;

            playerTransform = Player.transform;
            playerRigidbody = Player.GetComponent<Rigidbody>();

            SteeringWheel = GetComponentInChildren<SteeringWheel>(true);

            // init damage receiver
            damageReceiver = GetComponentInChildren<PlayerVehicleDamageReceiver>(true);
            damageReceiver.SetVehicle(this);

            Player.OnPlayerDeath += ProcessPlayerDeath;
        }

        void OnDestroy()
        {
            if (Player != null)
            {
                Player.OnPlayerDeath -= ProcessPlayerDeath;
            }
        }

        void ProcessPlayerDeath(GameScore score)
        {
            // stop the vehicle, if player died
            Brake();

            SteeringWheel.Stop();
        }

        /// <summary>
        /// Reinit this vehicle. Reset to default state.
        /// Should be called when player restarts
        /// </summary>
        /// <param name="accelerate">true, if start speed of this vehicle must be 0</param>
        public void Reinit(bool accelerate)
        {
            playerRigidbody.isKinematic = true;

            // default values
            currentSpeed = accelerate ? 0 : DefaultSpeed;
            currentSideSpeed = accelerate ? 0 : DefaultSideSpeed;

            targetSpeed = currentSpeed;
            targetSideSpeed = currentSideSpeed;

            TravelledDistance = 0;

            try
            {
                OnDistanceChange(TravelledDistance);
            }
            catch { }

            Health = MaxHealth;

            try
            {
                OnVehicleHealthChange(Health);
            }
            catch { }

            engineSmoke.Stop();
            engineFire.Stop();
        }

        #region interface impl
        public void ReceiveDamage(Damage damage)
        {
            if (Health == 0)
            {
                return;
            }

            Health -= damage.CalculateDamageValue(playerTransform.position);

            // play damage particle system
            hitParticles.transform.position = damage.Type == DamageType.Bullet ? damage.Point : transform.position;
            hitParticles.transform.rotation = Quaternion.LookRotation(
                damage.Type == DamageType.Bullet ? damage.Normal : damage.Point - transform.position);

            hitParticles.Play();

            if (Health <= SmokeHealthPercentage * MaxHealth)
            {
                engineSmoke.Play();
            }

            if (Health <= FireHealthPercentage * MaxHealth)
            {
                engineFire.Play();
            }

            if (Health <= 0)
            {
                Health = 0;

                // stop vehicle
                Brake();

                // explode
                StartCoroutine(WaitToExplode(TimeToExplode));
            }

            OnVehicleHealthChange(Health);
        }

        // this should be called only from other vehicles
        void IVehicle.Collide(IVehicle otherVehicle, VehicleCollisionInfo info)
        {
            if (Health == 0)
            {
                return;
            }

            float damage = info.Damage;

            // receive damage
            if (damage > 0)
            {
                // receive damage
                ReceiveDamage(Damage.CreateBulletDamage(
                    damage, -Vector3.forward, info.CollisionPoint, info.CollisionNormal, info.Initiator));
            }

            // collide other with this, player don't send them damage
            var backInfo = new VehicleCollisionInfo();
            backInfo.ThisWithOther = false;
            backInfo.Initiator = Player.gameObject;
            backInfo.CollisionPoint = info.CollisionPoint;
            backInfo.CollisionNormal = -info.CollisionNormal;

            otherVehicle.Collide(this, backInfo);

            // call event even if damage == 0
            OnVehicleCollision(otherVehicle, damage);
        }

        public void Accelerate()
        {
            targetSpeed = DefaultSpeed;
            targetSideSpeed = DefaultSideSpeed;
        }

        public void Brake()
        {
            targetSpeed = 0;
            targetSideSpeed = 0;
        }
        #endregion

        void FixedUpdate()
        {
            // process brake / acceleration
            if (currentSpeed != targetSpeed || currentSideSpeed != targetSideSpeed)
            {
                if (Mathf.Abs(currentSpeed - targetSpeed) > SpeedEpsilon ||
                    Mathf.Abs(currentSideSpeed - targetSideSpeed) > SpeedEpsilon)
                {
                    float time = currentSpeed < targetSpeed ? AccelerationTime : BrakeTime;

                    currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.fixedDeltaTime / time);
                    currentSideSpeed = Mathf.Lerp(currentSideSpeed, targetSideSpeed, Time.fixedDeltaTime / time);
                }
                else
                {
                    currentSpeed = targetSpeed;
                    currentSideSpeed = targetSideSpeed;
                }
            }

            // ignore if too small
            if (currentSpeed < SpeedEpsilon && currentSideSpeed < SpeedEpsilon)
            {
                return;
            }

            // get data from steering wheel
            float steering = SteeringWheel.Steering;

            // change position of the vehicle
            Vector3 newPosition = playerRigidbody.position;

            float forwardDistance = currentSpeed * Time.fixedDeltaTime;
            newPosition += playerTransform.forward * forwardDistance;

            if (steering > SteeringEpsilon || steering < -SteeringEpsilon)
            {
                newPosition += playerTransform.right * steering * currentSideSpeed * Time.fixedDeltaTime;

                var background = GameController.Instance.Background;

                if (background != null)
                {
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
            }

            playerRigidbody.MovePosition(newPosition);

            TravelledDistance += forwardDistance;

            if (Mathf.Abs(TravelledDistance - prevUpdatedDistance) > DistanceUpdateEpsilon)
            {
                OnDistanceChange(TravelledDistance);
                prevUpdatedDistance = TravelledDistance;
            }
        }

        void Update()
        {
            if (rotatingTransform != null)
            {
                float steering = SteeringWheel.Steering;

                Vector3 euler = rotatingTransform.localEulerAngles;
                euler.y = Mathf.LerpAngle(euler.y, steering * 3.0f, Time.fixedDeltaTime * 8);
                euler.z = Mathf.LerpAngle(euler.z, steering * 1.0f, Time.fixedDeltaTime * 9);

                rotatingTransform.localEulerAngles = euler;
            }

            OnSteering?.Invoke(SteeringWheel.Steering);
        }

        #region explosion
        /// <summary>
        /// Wait some time and then explode this vehicle
        /// </summary>
        IEnumerator WaitToExplode(float timeToWait)
        {
            yield return new WaitForSeconds(timeToWait);
            Explode();
        }

        /// <summary>
        /// Explode this vehicle
        /// </summary>
        public void Explode()
        {
            // explosion particle system
            ParticlesPool.Instance.Play("Explosion", engineFire.transform.position, Quaternion.identity);

            // kill player, but not instantly
            StartCoroutine(KillPlayerByExplosion());
        }

        IEnumerator KillPlayerByExplosion()
        {
            yield return new WaitForSeconds(0.2f);
            Player.Kill();
        }
        #endregion
    }
}
