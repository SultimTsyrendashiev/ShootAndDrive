﻿using System.Collections;
using UnityEngine;

namespace SD.Enemies
{
    class EnemyVan : EnemyVehicle
    {
        /// <summary>
        /// When passengers must start attack
        /// </summary>
        const float     AttackDistance = 12;
        const float     SideSpeed = 2;

        // static variable, true if there is already one van taht follow player
        static bool     AlreadyFollowing = false;
        bool            isFollowing;

        /// <summary>
        /// Speed when all passengers died
        /// </summary>
        float           driveAwaySpeed;

        Vector3         targetVelocity;
        Vector3         velocity;
        Transform       currentTarget;
        bool            doorsOpened;

        VanDoor[]       doors;

        protected override void InitEnemy()
        {
            doors = GetComponentsInChildren<VanDoor>(true);

            foreach (var d in doors)
            {
                d.Init(this);
            }
        }

        protected override void Activate()
        {
            VehicleRigidbody.isKinematic = true;
            velocity = transform.forward * Data.Speed;

            isFollowing = false;

            // find target
            var player = FindObjectOfType<PlayerLogic.Player>();
            currentTarget = player.transform;

            if (player.Vehicle != null)
            {
                float targetSpeed = player.Vehicle.DefaultSpeed;
                targetVelocity = transform.forward * targetSpeed;

                const float diff = 0.0f;
                driveAwaySpeed = targetSpeed - diff;
            }

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
            if (State == EnemyVehicleState.Active)
            {
                // distance between van and target along target forward vector
                float projLength = Vector3.Dot(VehicleRigidbody.position - currentTarget.position, currentTarget.forward);

                // if nobody follows or this van follows
                bool canFollow = !AlreadyFollowing || isFollowing;

                // clamp position, if close to target
                if (Mathf.Abs(projLength) <= AttackDistance && canFollow)
                {
                    float x = Mathf.Lerp(VehicleRigidbody.position.x, currentTarget.position.x, SideSpeed * Time.fixedDeltaTime);

                    Vector3 pos = currentTarget.position + currentTarget.forward * AttackDistance;
                    pos.x = x;

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
                // drive away
                VehicleRigidbody.position += velocity * Time.fixedDeltaTime;
            }
        }


        // when all passengers died, 
        // just drive away;
        protected override void DoPassengerDeath()
        {
            velocity = transform.forward * driveAwaySpeed;

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
    }
}
