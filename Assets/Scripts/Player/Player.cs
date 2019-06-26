﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SD.Weapons;

namespace SD.Player
{
    class Player : MonoBehaviour, IDamageable
    {
        #region constants
        /// <summary>
        /// Min health when regeneration without medkit can be applied 
        /// </summary>
        const float                 MinHealthForRegeneration = 20;
        /// <summary>
        /// Min health there must be after regeneration
        /// </summary>
        const float                 MinRegeneratedHealth = 20;
        /// <summary>
        /// How many health points will be regenerated without medkit
        /// </summary>
        const float                 HealthToRegenerate = 10;
        const float                 HealthAfterMedkit = 100;
        const float                 MaxHealth = 100;
        #endregion

        PlayerState                 state;
        private float               health;

        private Camera              playerCamera;

        [SerializeField]
        private PlayerInventory     inventory;

        private static Player       instance;

        public static Player        Instance => instance;
        public Camera               MainCamera => playerCamera;
        public PlayerInventory      Inventory => inventory;
        public PlayerState          State => state;

        void Awake()
        {
            Debug.Assert(instance == null, "Several players in a scene");
          
            instance = this;

            playerCamera = GetComponentInChildren<Camera>();

            inventory = new PlayerInventory();
            inventory.Load();

            // for testing
            inventory.GiveAll();

            state = PlayerState.Ready;
        }

        #region health management
        void Die()
        {
            state = PlayerState.Dead;

            // TODO:
            // play anim, sound

            // hide weapon
            WeaponsController.Instance.HideWeapon();

            // 

        }

        public void RegenerateHealth()
        {            
            // check if weapon state is Ready
            if (WeaponsController.Instance.GetCurrentWeaponState() != WeaponState.Ready)
            {
                return;
            }

            state = PlayerState.Regenerating;

            // hide weapon
            WeaponsController.Instance.HideWeapon();

            // regenerate
            if (health < MinHealthForRegeneration)
            {
                // regeneration without medkit
                health += HealthToRegenerate;

                if (health < MinRegeneratedHealth)
                {
                    health = MinRegeneratedHealth;
                }

                StartCoroutine(WaitForRegeneration(false));
            }
            else if (health < MaxHealth)
            {
                // use medkit
                health = MaxHealth;

                StartCoroutine(WaitForRegeneration(true));
            }
        }

        IEnumerator WaitForRegeneration(bool usingMedkit)
        {
            // TODO:
            // anim, sound

            do
            {
                yield return null;

                // if died while regenerating
                if (state == PlayerState.Dead || state == PlayerState.Nothing)
                {
                    yield break;
                }

            } while (state != PlayerState.Ready);

            // finally, take out weapon
            WeaponsController.Instance.TakeOutWeapon();

            state = PlayerState.Ready;
        }
        #endregion

        #region inherited
        float IDamageable.Health
        {
            get
            {
                return health;
            }
        }

        void IDamageable.ReceiveDamage(Damage damage)
        {
            float damageValue = damage.Value;

            if (damage.Type == DamageType.Explosion)
            {
                float length = (transform.position - damage.Point).magnitude;
                
                if (length < 1.0f)
                {
                    length = 1.0f;
                }

                damageValue /= length;
            }

            health -= damageValue;

            if (health <= 0)
            {
                Die();
            }
        }
        #endregion
    }
}
