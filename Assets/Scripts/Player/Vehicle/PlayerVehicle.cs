using System.Collections;
using System.Collections.Generic;
using SD.Background;
using UnityEngine;

namespace SD.PlayerLogic
{
    [RequireComponent(typeof(Collider))]
    class PlayerVehicle : MonoBehaviour, IVehicle, IDamageable
    {
        public int                  MaxHealth = 1000;


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


        public event                FloatChange OnVehicleHealthChange;
        public event                FloatChange OnDistanceChange;

        Transform                   playerTransform;
        Rigidbody                   playerRigidbody;

        public float                DefaultSpeed = 20;
        public float                SideSpeed = 10;

        //List<PlayerVehicleDamageReceiver> damageReceivers;
        PlayerVehicleDamageReceiver damageReceiver;
        public event CollideVehicle OnVehicleCollision;

        // to check horizontal bounds
        IBackgroundController       background;

        [SerializeField]
        float                       currentSpeed;
        float                       currentSideSpeed;

        #region particles
        [SerializeField]
        string                      HitParticlesName = "Sparks";
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

        public float Health { get; private set; }

        float prevUpdatedDistance = -1;
        public float TravelledDistance { get; private set; }

        public ISteeringWheel SteeringWheel { get; private set; }

        public Player Player { get; private set; }

        public void Init(Player player)
        {
            Player = player;

            playerTransform = Player.transform;
            playerRigidbody = Player.GetComponent<Rigidbody>();

            SteeringWheel = GetComponentInChildren<SteeringWheel>(true);

            // init damage receiver
            damageReceiver = GetComponentInChildren<PlayerVehicleDamageReceiver>(true);
            damageReceiver.SetVehicle(this);
        }

        public void Reinit(bool accelerate)
        {
            playerRigidbody.isKinematic = true;

            // default values
            currentSpeed = accelerate ? 0 : DefaultSpeed;
            currentSideSpeed = accelerate ? 0 : SideSpeed;

            TravelledDistance = 0;

            Health = MaxHealth;

            engineSmoke.Stop();
            engineFire.Stop();
        }

        void Start()
        {
            background = GameController.Instance.Background;
        }

        public void ReceiveDamage(Damage damage)
        {
            if (Health == 0)
            {
                return;
            }

            Health -= damage.CalculateDamageValue(playerTransform.position);

            // play damage particle system
            ParticlesPool.Instance.Play(HitParticlesName,
                damage.Type == DamageType.Bullet ? damage.Point : transform.position, Quaternion.LookRotation(
                damage.Type == DamageType.Bullet ? damage.Normal : damage.Point - transform.position));

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

        void FixedUpdate()
        {
            // if alive and speed isn't default, accelerate
            if (Health > 0 && (currentSpeed < DefaultSpeed || currentSideSpeed < SideSpeed))
            {
                if (currentSpeed < DefaultSpeed - SpeedEpsilon || currentSideSpeed < SideSpeed - SpeedEpsilon)
                {
                    const float lerpMultiplier = 1.0f;

                    currentSpeed = Mathf.Lerp(currentSpeed, DefaultSpeed, Time.fixedDeltaTime * lerpMultiplier);
                    currentSideSpeed = Mathf.Lerp(currentSideSpeed, SideSpeed, Time.fixedDeltaTime * lerpMultiplier);
                }
                else
                {
                    currentSpeed = DefaultSpeed;
                    currentSideSpeed = SideSpeed;
                }
            }


            // ignore if too smale
            if (currentSpeed < SpeedEpsilon && currentSideSpeed < SpeedEpsilon)
            {
                return;
            }


            // slow down this vehicle if vehicle is broken
            if (Health <= 0)
            {
                const float lerpMultiplier = 2.0f;

                currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.fixedDeltaTime * lerpMultiplier);
                currentSideSpeed = Mathf.Lerp(currentSideSpeed, 0, Time.fixedDeltaTime * lerpMultiplier);
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

            if (TravelledDistance - prevUpdatedDistance > DistanceUpdateEpsilon)
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
        }
    }
}
