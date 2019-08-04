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

        const float                 SteeringEpsilon = 0.01f;

        // how long to wait for explosion when health < 0
        const float                 TimeToExplode = 1.0f;

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

                // slow down and explode
                StartCoroutine(SlowDownVehicle());
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
        /// Slows down this vehicle and after some time explodes
        /// </summary>
        IEnumerator SlowDownVehicle()
        {
            const float speedEpsilon = 0.1f;
            const float lerpMultiplier = 2.0f;

            while (true)
            {
                if (currentSpeed < speedEpsilon && currentSideSpeed < speedEpsilon)
                {
                    currentSpeed = 0;
                    currentSideSpeed = 0;

                    break;
                }

                currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * lerpMultiplier);
                currentSideSpeed = Mathf.Lerp(currentSideSpeed, 0, Time.deltaTime * lerpMultiplier);

                yield return null;
            }
        }

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
            ParticlesPool.Instance.Play("Explosion", transform.position, Quaternion.identity);

            // TODO:
            // add rotation to vehicle, 
            // but camera shouldn't see

            // kill player, but not instantly
            StartCoroutine(WaitForExplosion());
        }

        IEnumerator WaitForExplosion()
        {
            yield return new WaitForSeconds(0.2f);
            Player.Kill();
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
