using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SD.Weapons;
using SD.UI;

namespace SD.PlayerLogic
{
    public delegate void FloatChange(float f);

    /// <summary>
    /// Player class. There must be 
    /// 'PlayerVehicle' and 'PlayerDamageReceiver'
    /// as child objects
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
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

        PlayerVehicle       playerVehicle;
        ISteeringWheel      steeringWheel;

        public event FloatChange OnHealthChange;

        public static Player    Instance { get; private set; }
        public Camera           MainCamera { get; private set; }
        public PlayerInventory  Inventory => PlayerInventory.Instance;
        public PlayerState      State { get; private set; }
        public float            Health { get; private set; } = MaxHealth;

        void Awake()
        {
            Debug.Assert(Instance == null, "Several players in a scene", this);
            Instance = this;

            #region TODO: remove from this class
            Application.targetFrameRate = 60;

            // TODO: must be not here
            // load items from player prefs
            Inventory.Load();
            Inventory.GiveAll();
            #endregion
        }

        void Start()
        {
            MainCamera = GetComponentInChildren<Camera>();
            playerVehicle = GetComponentInChildren<PlayerVehicle>(true);

            Debug.Assert(playerVehicle != null, "There must be a 'PlayerVehicle' as child object", this);

            steeringWheel = playerVehicle.SteeringWheel;
            State = PlayerState.Ready;
        }

        void Update()
        {
            // player must call steering wheel methods
            // to control vehicle
            float x = InputController.MovementHorizontal;
            steeringWheel.Steer(x);

            Background.BackgroundController.Instance.UpdateCameraPosition(MainCamera.transform.position);
        }

#region health management
        void Die()
        {
            State = PlayerState.Dead;

            // TODO:
            // play anim, sound

            // hide weapon
            WeaponsController.Instance.HideWeapon();
        }

        public void RegenerateHealth()
        {            
            // if player is busy
            if (State != PlayerState.Ready)
            {
                return;
            }

            // if weapons controller is busy
            if (WeaponsController.Instance.IsBusy())
            {
                return;
            }

            // regenerate if health is not max
            if (Health < MaxHealth)
            {
                StartCoroutine(WaitForRegeneration());
            }
        }

        IEnumerator WaitForRegeneration()
        {
            State = PlayerState.Regenerating;

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
                if (State == PlayerState.Dead || State == PlayerState.Nothing)
                {
                    yield break;
                }

            } while (State != PlayerState.Ready);

            // add health
            if (Health < MinHealthForRegeneration)
            {
                Health += HealthToRegenerate;

                if (Health < MinRegeneratedHealth)
                {
                    Health = MinRegeneratedHealth;
                }
            }
            else
            {
                Health = HealthAfterMedkit;
            }

            // finally, take out weapon
            WeaponsController.Instance.TakeOutWeapon();

            State = PlayerState.Ready;
        }
#endregion

#region inherited
        /// <summary>
        /// Note: must be called only by 'PlayerDamageReceiver'
        /// </summary>
        public void ReceiveDamage(Damage damage)
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

            Health -= damageValue;
            OnHealthChange(Health);

            if (Health <= 0)
            {
                Die();
            }
        }
#endregion
    }
}
