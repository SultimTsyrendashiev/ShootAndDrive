﻿using System.Collections;
using UnityEngine;
using SD.Weapons;

namespace SD.PlayerLogic
{
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
        public const float                  MinHealthForRegeneration = 25;

        /// <summary>
        /// Min health there must be after regeneration
        /// </summary>
        const float                         MinRegeneratedHealth = MinHealthForRegeneration;
        /// <summary>
        /// How many health points will be regenerated without medkit
        /// </summary>
        const float                         HealthToRegenerate = 20;
        const float                         HealthAfterMedkit = MaxHealth;
        public const float                  MaxHealth = 100;
        #endregion

        [SerializeField]
        ParticleSystem                      bloodParticles;

        ISteeringWheel                      steeringWheel;
        WeaponsController                   weaponsController;
        GameScore                           currentScore;

        float                               health;
        public float                        Health
        {
            get
            {
                return health;
            }
            private set
            {
                health = value;

                try
                {
                    OnHealthChange(value);
                }
                catch { }
            }
        }

        public Camera                       MainCamera { get; private set; }
        public PlayerInventory              Inventory { get; private set; }
        public PlayerState                  State { get; private set; }
        public GameScore                    CurrentScore => currentScore;
        public PlayerVehicle                Vehicle { get; private set; }

        #region events
        public event FloatChange            OnHealthChange;
        public event ScoreChange            OnScoreChange;

        /// <summary>
        /// Called when player dies, contains calculated score
        /// </summary>
        public event PlayerDeath            OnPlayerDeath;
        public event PlayerStateChange      OnPlayerStateChange;
        public static event PlayerSpawn     OnPlayerSpawn;
        public event PlayerKills            OnKill;
        public event PlayerKills            OnDestroyVehicle;
        #endregion

        #region init / destroy
        /// <summary>
        /// Init player, must be called before 'Start'
        /// </summary>
        public void Init()
        {            
            MainCamera = GetComponentInChildren<Camera>();


            // player must be faced to world forward
            transform.forward = Vector3.forward;


            // init vehicle
            Vehicle = GetComponentInChildren<PlayerVehicle>(true);
            Debug.Assert(Vehicle != null, "There must be a 'PlayerVehicle' as child object", this);

            Vehicle.Init(this);
            steeringWheel = Vehicle.SteeringWheel;

            // init weapons
            weaponsController = GetComponentInChildren<WeaponsController>();
            weaponsController.SetOwner(this);

            SignToEvents();
        }

        /// <summary>
        /// Reinit player
        /// </summary>
        /// <param name="position">where to spawn player</param>
        /// <param name="defaultVehicleSpeed">if false, speed of player's vehicle will be zero</param>
        public void Reinit(Vector3 position, bool defaultVehicleSpeed = true)
        {
            gameObject.SetActive(true);
            transform.position = position;

            Health = MaxHealth;

            // reset score
            currentScore = new GameScore(Vehicle.MaxHealth);

            // reset vehicle
            Vehicle.Reinit(!defaultVehicleSpeed);

            CameraShaker.Instance?.ResetAnimation();

            State = PlayerState.Ready;
            OnPlayerStateChange(State);

            OnScoreChange?.Invoke(currentScore);
        }

        void Start()
        {
            OnPlayerSpawn(this);
        }

        /// <summary>
        /// Inits inventory and weapons
        /// </summary>
        public void InitInventory()
        {
            Inventory = new PlayerInventory();
            Inventory.Init(); // default values
        }

        void SignToEvents()
        {
            Enemies.EnemyVehicle.OnEnemyDeath += AddEnemyScore;
            Enemies.EnemyVehicle.OnVehicleDestroy += AddEnemyVehicleScore;
            UI.InputController.OnHealthRegenerate += RegenerateHealth;
            UI.InputController.OnMovementHorizontal += UpdateInput;
            Vehicle.OnVehicleCollision += CollideVehicle;
            GameController.OnGamePause += Pause;
        }

        /// <summary>
        /// To enable GC
        /// </summary>
        void UnsignFromEvents()
        {
            Enemies.EnemyVehicle.OnEnemyDeath -= AddEnemyScore;
            Enemies.EnemyVehicle.OnVehicleDestroy -= AddEnemyVehicleScore;
            UI.InputController.OnHealthRegenerate -= RegenerateHealth;
            UI.InputController.OnMovementHorizontal -= UpdateInput;
            Vehicle.OnVehicleCollision -= CollideVehicle;
            GameController.OnGamePause -= Pause;
        }

        void OnDestroy()
        {
            UnsignFromEvents();
        }
        #endregion

        public void UpdateInput(float horizonalAxis)
        {
            // if regenerating, still can steer
            if (State == PlayerState.Dead || State == PlayerState.Nothing)
            {
                return;
            }

            // player must call steering wheel methods
            // to control vehicle
            steeringWheel.Steer(horizonalAxis);
        }

        #region score
        void AddEnemyScore(Enemies.EnemyData data, Transform enemyPosition, GameObject initiator)
        {
            // if initiator is player, count score
            if (initiator == gameObject)
            {
                currentScore.KillsAmount++;
                currentScore.ScorePoints += data.Score;

                OnScoreChange?.Invoke(currentScore);
                OnKill?.Invoke(enemyPosition);
            }
        }

        void AddEnemyVehicleScore(Enemies.EnemyVehicleData data, Transform enemyPosition, GameObject initiator)
        {
            // if initiator is player, count score
            // if (initiator == gameObject)
            {
                currentScore.DestroyedVehiclesAmount++;
                currentScore.ScorePoints += data.Score;

                OnScoreChange?.Invoke(currentScore);
                OnDestroyVehicle?.Invoke(enemyPosition);
            }
        }
        #endregion

        #region health management
        void Die()
        {
            // ignore if already died
            if (State == PlayerState.Dead)
            {
                return;
            }

            State = PlayerState.Dead;
            OnPlayerStateChange(State);

            // send player's score
            currentScore.VehicleHealth = (int)Vehicle.Health;
            currentScore.TravelledDistance = Vehicle.TravelledDistance;

            currentScore.Calculate();
            OnPlayerDeath(currentScore);

            // TODO:
            // sound
            CameraShaker.Instance.PlayAnimation(CameraShaker.CameraAnimation.Death);
        }

        public bool RegenerateHealth(int healthPoints)
        {
            int maxHealth = (int)MaxHealth;

            if (Health < maxHealth)
            {
                int newHealth = (int)Health + healthPoints;
                newHealth = newHealth < maxHealth ? newHealth : maxHealth;

                Health = newHealth;
                return true;
            }

            return false;
        }

        #region self health regeneration
        public bool RegenerateHealth()
        {
            Debug.Log("Self health regeneration is not supported");
            return false;

            //// if player is busy
            //if (State != PlayerState.Ready)
            //{
            //    return false;
            //}

            //// if weapons controller is busy
            //if (weaponsController.IsBusy())
            //{
            //    return false;
            //}

            //// regenerate if health is not max
            //if (Health < MaxHealth)
            //{
            //    StartCoroutine(WaitForRegeneration());
            //}

            //return true;
        }
        
        //IEnumerator WaitForRegeneration()
        //{
        //    State = PlayerState.Regenerating;

        //    // weapons must be hidden,
        //    // so wait
        //    while (weaponsController.IsBusy())
        //    {
        //        yield return null;
        //    }

        //    // TODO:
        //    // start anim and sound

        //    Debug.Log("Regenerating health");

        //    float animLength = 1.0f;
        //    float waited = 0.0f;

        //    do
        //    {
        //        yield return null;
        //        waited += Time.deltaTime;

        //        // if died while regenerating
        //        if (State == PlayerState.Dead || State == PlayerState.Nothing)
        //        {
        //            yield break;
        //        }

        //    } while (waited < animLength);

        //    // add health
        //    if (Health < MinHealthForRegeneration)
        //    {
        //        Health += HealthToRegenerate;

        //        if (Health < MinRegeneratedHealth)
        //        {
        //            Health = MinRegeneratedHealth;
        //        }
        //    }
        //    else
        //    {
        //        Health = HealthAfterMedkit;
        //    }

        //    OnHealthChange(Health);

        //    State = PlayerState.Ready;
        //}
        #endregion

        /// <summary>
        /// Note: must be called only by 'PlayerDamageReceiver'
        /// </summary>
        public void ReceiveDamage(Damage damage)
        {           
            // always play blood particle system
            bloodParticles.Play();

            if (Health <= 0)
            {
                return;
            }

            float damageValue = damage.CalculateDamageValue(transform.position);

            if (damageValue > 0)
            {
                float newHealth = Health - damageValue;
                newHealth = newHealth < 0 ? 0 : newHealth;

                Health = newHealth;

                if (Health <= 0)
                {
                    Die();
                }
                else
                {
                    if (damage.Type == DamageType.Explosion)
                    {
                        CameraShaker.Instance.PlayAnimation(CameraShaker.CameraAnimation.Explosion);
                    }
                    else
                    {
                        CameraShaker.Instance.PlayAnimation(CameraShaker.CameraAnimation.Damage);
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// This method must be called when there is 
        /// collision between player vehicle and some other one
        /// </summary>
        void CollideVehicle(IVehicle other, float damage)
        {
            // reduce full damage
            float damageMultiplier = 0.5f * MaxHealth / Vehicle.MaxHealth;

            damage *= damageMultiplier;

            if (damage > 0)
            {
                // receive damage
                ReceiveDamage(Damage.CreateBulletDamage(
                    damage, -transform.forward, transform.position, transform.forward, null));

                if (Health > 0)
                {
                    // if still alive, play default animation
                    CameraShaker.Instance.PlayAnimation(CameraShaker.CameraAnimation.Collision);
                }
                // otherwise death animation will be played (in receive damage)
            }
            else
            {
                // just play animation
                CameraShaker.Instance.PlayAnimation(CameraShaker.CameraAnimation.Collision);
            }
        }

        /// <summary>
        /// Kill the player
        /// </summary>
        public void Kill()
        {
            if (State == PlayerState.Nothing)
            {
                return;
            }

            if (Health <= 0)
            {
                return;
            }

            Damage fatalDamage = Damage.CreateBulletDamage(Health,
                    transform.forward, transform.position, transform.forward, null);

            ReceiveDamage(fatalDamage);
        }

        /// <summary>
        /// This method must be called on game pause
        /// </summary>
        void Pause()
        {
            /// send event that score is changed, 
            /// so it may be presented in pause menu, for example
            OnScoreChange?.Invoke(currentScore);
        }
    }
}
