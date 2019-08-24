using System.Collections;
using UnityEngine;

namespace SD.Enemies
{
    class EnemyVan : EnemyVehicle
    {
        /// <summary>
        /// When passengers must start attack
        /// </summary>
        const float             AttackDistance = 15;
        const float             SideSpeed = 1;
        const float             DriveAwayBoundEpsilon = 2;

        // static variable, true if there is already one van taht follow player
        static bool             AlreadyFollowing = false;
        bool                    isFollowing;

        /// <summary>
        /// Speed when all passengers died
        /// </summary>
        // float                   driveAwaySpeed;

        Vector3                 targetVelocity;
        Vector3                 velocity;
        IEnemyTarget            currentTarget;
        bool                    doorsOpened;

        VanDoor[]               doors;

        PlayerLogic.PlayerVehicle       playerVehicle;
        IBackgroundController           background;

        float                   xBound;

        protected override void InitEnemy()
        {
            background = GameController.Instance.Background;
            doors = GetComponentsInChildren<VanDoor>(true);

            foreach (var d in doors)
            {
                d.Init(this);
            }
        }

        void FindPlayerVechicle()
        {
            playerVehicle = GameController.Instance.CurrentPlayer.Vehicle;
            if (playerVehicle != null)
            {
                float targetSpeed = playerVehicle.DefaultSpeed;
                targetVelocity = transform.forward * targetSpeed;

                // driveAwaySpeed = 0;
            }
        }

        void FindTarget()
        {
            currentTarget = GameController.Instance.EnemyTarget;
        }

        protected override void Activate()
        {
            VehicleRigidbody.isKinematic = true;
            velocity = transform.forward * Data.Speed;

            if (currentTarget == null)
            {
                FindTarget();
            }

            if (playerVehicle == null)
            {
                FindPlayerVechicle();
            }

            isFollowing = false;

            CloseDoors();
        }

        protected override void Deactivate()
        {
            // if exactly this van followed
            if (AlreadyFollowing && isFollowing)
            {
                isFollowing = false;
                AlreadyFollowing = false;
            }
        }

        void FixedUpdate()
        {
            // dont override position, if collided
            if (!VehicleRigidbody.isKinematic)
            {
                return;
            }

            if (State == EnemyVehicleState.Active)
            {
                var targetTransform = currentTarget.Target;

                // distance between van and target along target forward vector
                float projLength = Vector3.Dot(VehicleRigidbody.position - targetTransform.position, targetTransform.forward);

                // if nobody follows or this van follows
                bool canFollow = !AlreadyFollowing || isFollowing;

                // clamp position, if close to target
                if (Mathf.Abs(projLength) <= AttackDistance && canFollow)
                {
                    float x = Mathf.Lerp(VehicleRigidbody.position.x, targetTransform.position.x, SideSpeed * Time.fixedDeltaTime);

                    Vector3 pos = new Vector3(x, transform.position.y,
                        targetTransform.position.z + AttackDistance);

                    VehicleRigidbody.position = pos;

                    // open doors if not opened
                    if (!doorsOpened)
                    {
                        OpenDoors();
                    }

                    isFollowing = true;
                    AlreadyFollowing = true;

                    return;
                }

                if (!AlreadyFollowing)
                {
                    // if not nobody follows, move with default speed
                    VehicleRigidbody.position += velocity * Time.fixedDeltaTime;
                }
                else
                {
                    // if somebody already follows, move with speed of player
                    VehicleRigidbody.position += targetVelocity * Time.fixedDeltaTime;
                }

                // if exactly this van followed, but at this moment stopped
                if (AlreadyFollowing && isFollowing)
                {
                    isFollowing = false;
                    AlreadyFollowing = false;
                }
            }
            else if (State == EnemyVehicleState.DeadPassengers)
            {
                // drive away:
                // move to bound, if far from it,
                if (Mathf.Abs(VehicleRigidbody.position.x - xBound) > DriveAwayBoundEpsilon)
                {
                    var targetTransform = currentTarget.Target;

                    float x = Mathf.Lerp(VehicleRigidbody.position.x, xBound, SideSpeed * Time.fixedDeltaTime);

                    Vector3 pos = new Vector3(x, transform.position.y,
                        targetTransform.position.z + AttackDistance);

                    VehicleRigidbody.position = pos;
                }
                // otherwise stop

                //else
                //{
                //    // velocity changed to side velocity after passengers death
                //    VehicleRigidbody.position += velocity * Time.fixedDeltaTime;
                //}
            }
        }


        // when all passengers died, 
        // just drive away;
        protected override void DoPassengerDeath()
        {
            Vector3 pos = VehicleRigidbody.position;
            Vector2 b = background.GetBlockBounds(pos);

            // find nearest bound
            if (Mathf.Abs(b[0] - pos.x) < Mathf.Abs(b[1] - pos.x))
            {
                xBound = b[0];
                //velocity = -transform.right * SideSpeed;
            }
            else
            {
                xBound = b[1];
                //velocity = transform.right * SideSpeed;
            }

            // if exactly this van followed
            if (AlreadyFollowing && isFollowing)
            {
                isFollowing = false;
                AlreadyFollowing = false;
            }
        }

        /// <summary>
        /// Close doors on activating, i.e. without any effects
        /// </summary>
        void CloseDoors()
        {
            doorsOpened = false;

            foreach (var d in doors)
            {
                d.Close();
            }

            // deactivate passengers
            foreach (var p in Passengers)
            {
                p.SetTarget(null);
            }
        }

        /// <summary>
        /// Open doors with animation, etc.
        /// Also, this method must activate passengers for attacking
        /// </summary>
        void OpenDoors()
        {
            doorsOpened = true;

            foreach (var d in doors)
            {
                d.Open();
            }

            // activate passengers
            foreach (var p in Passengers)
            {
                p.SetTarget(currentTarget);
            }
        }

        protected override void DoVehicleCollision()
        {
            // manually set not kinematic, as there is no driver
            SetKinematic(false);

            KillAllPassengers(null);
        }
    }
}
