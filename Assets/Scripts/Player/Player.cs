using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SD.Weapons;
using SD.UI;

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

        // steering wheel is used for controlling a vehicle
        [SerializeField]
        private SteeringWheel       steeringWheel; 

        private Camera              playerCamera;

        private static Player       instance;
        public static Player        Instance => instance;
        public Camera               MainCamera => playerCamera;
        public PlayerInventory      Inventory => PlayerInventory.Instance;
        public PlayerState          State => state;

        void Awake()
        {
            Application.targetFrameRate = 30;

            Debug.Assert(instance == null, "Several players in a scene");
            Debug.Assert(steeringWheel != null);
          
            instance = this;
            playerCamera = GetComponentInChildren<Camera>();

            // load items from player prefs
            Inventory.Load();
            Inventory.GiveAll();

            state = PlayerState.Ready;
        }

        void Update()
        {
            float x = InputController.MovementHorizontal;
            steeringWheel.Steer(x);
        }

#region health management
        void Die()
        {
            state = PlayerState.Dead;

            // TODO:
            // play anim, sound

            // hide weapon
            WeaponsController.Instance.HideWeapon();
        }

        public void RegenerateHealth()
        {            
            // if player is busy
            if (state != PlayerState.Ready)
            {
                return;
            }

            // if weapons controller is busy
            if (WeaponsController.Instance.IsBusy())
            {
                return;
            }

            // regenerate if health is not max
            if (health < MaxHealth)
            {
                StartCoroutine(WaitForRegeneration());
            }
        }

        IEnumerator WaitForRegeneration()
        {
            state = PlayerState.Regenerating;

            // hide weapon and wait
            WeaponsController.Instance.HideWeapon();
            while (WeaponsController.Instance.IsBusy())
            {
                yield return null;
            }

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

            // add health
            if (health < MinHealthForRegeneration)
            {
                health += HealthToRegenerate;

                if (health < MinRegeneratedHealth)
                {
                    health = MinRegeneratedHealth;
                }
            }
            else
            {
                health = HealthAfterMedkit;
            }

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
