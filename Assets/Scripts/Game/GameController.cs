using System;
using System.Collections;
using UnityEngine;
using SD.PlayerLogic;
using SD.Weapons;
using SD.Enemies.Spawner;
using SD.Background;
using SD.UI;
using SD.Game.Data;
using SD.Game;
using SD.Game.Shop;
using SD.Game.Settings;
using SD.Online;
using SD.ObjectPooling;

namespace SD
{
    // TODO: refactoring
    class GameController : MonoBehaviour
    {
        [SerializeField]
        LanguageList                    languageList;

        [SerializeField]
        WeaponsList                     weaponsList;
        [SerializeField]
        AmmoList                        ammoList;

        [SerializeField]
        GameObject                      playerPrefab;

        [SerializeField]
        ObjectPoolPrefabs               objectPoolPrefabs;
        [SerializeField]
        ParticlesPoolPrefabs            particlesPoolPrefabs;


        [SerializeField]
        GameObject                      mainMenuBackground;



        Vector3                         defaultPlayerPosition;

        InventoryData                   lastLoadedInventoryData;


        // services
        public Player                   CurrentPlayer { get; private set; }

        public IInventory               Inventory { get; private set; }
        public IShop                    Shop { get; private set; }

        public IBackgroundController    Background { get; private set; }
        public IEnemyTarget             EnemyTarget { get; private set; }

        public SettingsSystem           SettingsSystem { get; private set; }

        AudioManager                    audioManager;

        SpawnersController              spawnersController;
        CutsceneManager                 cutsceneManager;
        TutorialManager                 tutorialManager;
        TimeController                  timeController;

        public ObjectPool               ObjectPool { get; private set; }
        public ParticlesPool            ParticlesPool { get; private set; }

        public IOnlineService           OnlineService { get; private set; }

        public GameState                State { get; private set; }

        // stats
        public AllWeaponsStats          WeaponsStats { get; private set; }
        public AllAmmoStats             AmmoStats { get; private set; }

        // settings
        public GlobalSettings           Settings { get; private set; }
        public LanguageTable            Localization => csvLanguageTable.Languages;
        CSVLanguageTable                csvLanguageTable;

        AudioSettingsHandler            audioSettingsHandler;


        #region events
        public static event Action<IOnlineService>  OnSignIn;
        public static event Action<GameController>  OnGameInit;

        public static event Void                    OnGameplayActivate;
        public static event Void                    OnGamePause;
        public static event Void                    OnGameUnpause;
        public static event Void                    OnMainMenuActivate;
        public static event Void                    OnInventoryOpen;
        public static event Action<Player>          OnPlayerDeath;

        public static event Void                    OnWeaponSelectionEnable;
        public static event Void                    OnWeaponSelectionDisable;

        public static event ScoreSet                OnScoreSet;

        /// <summary>
        /// Called when game ends, i.e. when gameplay elements can be destroyed :
        /// when goes to main menu, when player dies etc
        /// </summary>
        public static event Void                    OnGameEnd;
        #endregion

        // this class is always alive
        public static GameController    Instance { get; private set; }

        #region init / destroy
        void Awake()
        {
            Debug.Assert(Instance == null, "Several game controllers.", this);
            Instance = this;

            Application.targetFrameRate = 60;

            Init();
        }

        void OnApplicationQuit()
        {
            SaveSettings();
            UnsignFromEvents();
        }

        void SignToEvents()
        {
            CurrentPlayer.OnPlayerDeath += ProcessPlayerDeath;
            Inventory.OnBalanceChange += SaveInventoryOnBalanceChange;
            
            InputController.OnPause += PauseGame;
            InputController.OnUnpause += UnpauseGame;

            InputController.OnPlayButton += Play;
            InputController.OnPlayWithInventoryButton += PlayWithInventoryMenu;

            InputController.OnMainMenuButton += StopGame;
            InputController.OnInventoryButton += ShowInventory;

            InputController.OnInventoryRevert += RevertInventory;

            InputController.OnWeaponSelectionButton += EnableWeaponsSelection;
            InputController.OnWeaponSelectionDisableButton += DisableWeaponsSelection;

            InputController.OnSettingsApply += ApplySettings;
        }

        void UnsignFromEvents()
        {
            if (CurrentPlayer != null)
            {
                CurrentPlayer.OnPlayerDeath -= ProcessPlayerDeath;
            }

            if (Inventory != null)
            {
                Inventory.OnBalanceChange -= SaveInventoryOnBalanceChange;
            }

            InputController.OnPause -= PauseGame;
            InputController.OnUnpause -= UnpauseGame;

            InputController.OnPlayButton -= Play;
            InputController.OnPlayWithInventoryButton -= PlayWithInventoryMenu;

            InputController.OnMainMenuButton -= StopGame;
            InputController.OnInventoryButton -= ShowInventory;

            InputController.OnInventoryRevert -= RevertInventory;

            InputController.OnWeaponSelectionButton -= EnableWeaponsSelection;
            InputController.OnWeaponSelectionDisableButton -= DisableWeaponsSelection;

            InputController.OnSettingsApply -= ApplySettings;
        }

        void ApplySettings()
        {
            // currently, settings applied instantly,
            // so just save them

            SaveSettings();
        }

        /// <summary>
        /// Init all systems
        /// </summary>
        void Init()
        {
            State = GameState.Menu;

            // load data from previous sessions
            LoadSettings();

            // init localization
            InitLocalization();

            CreateSystems();
            CheckSystems();

            InitSystems();
        }

        /// <summary>
        /// Create or find systems
        /// </summary>
        void CreateSystems()
        {
            // find objects
            Background                  = FindObjectOfType<BackgroundController>();
            cutsceneManager             = FindObjectOfType<CutsceneManager>();
            tutorialManager             = FindObjectOfType<TutorialManager>();
            audioManager                = FindObjectOfType<AudioManager>();

            WeaponsStats                = new AllWeaponsStats(weaponsList.Data);
            AmmoStats                   = new AllAmmoStats(ammoList.Data);

            spawnersController          = new SpawnersController();
            SettingsSystem              = new SettingsSystem(Settings);
            Shop                        = new ShopSystem();
            audioSettingsHandler        = new AudioSettingsHandler();
            timeController              = new TimeController(Time.fixedDeltaTime);

            ObjectPool                  = new ObjectPool(objectPoolPrefabs, transform);
            ParticlesPool               = new ParticlesPool(particlesPoolPrefabs, transform);

#if UNITY_ANDROID && !UNITY_EDITOR
            OnlineService               = new PlayGamesService();
#endif
        }

        void CheckSystems()
        {
            // check all systems
            Debug.Assert(WeaponsStats != null, "Can't find AllWeaponsStats", this);
            Debug.Assert(Background != null, "Can't find BackgroundController", this);
            Debug.Assert(spawnersController != null, "Can't find SpawnersController", this);
            Debug.Assert(cutsceneManager != null, "Can't find CutsceneManager", this);
            Debug.Assert(tutorialManager != null, "Can't find TutorialManager", this);
            Debug.Assert(ObjectPool != null, "Can't find ObjectPool", this);
            Debug.Assert(ParticlesPool != null, "Can't find ParticlesPool", this);
            Debug.Assert(audioManager != null, "Can't find AudioManager");
        }

        void InitSystems()
        {
            SettingsInitializer.Init(SettingsSystem, Settings, audioManager, audioSettingsHandler, timeController);

            InitPlayer();

            // inventory is loaded, init shop system
            Shop.Init(Inventory);

            // all systems initialized, sign up to events
            SignToEvents();

            // at last, init object and particle pools
            InitPools();

            if (OnlineService != null)
            {
                OnlineService.Activate();
                OnlineService.SignIn();

                StartCoroutine(WaitForOnlineServiceInit());
            }
        }

        IEnumerator WaitForOnlineServiceInit()
        {
            // wait for authenticating,
            // always wait 1 frame
            do
            {
                yield return null;

            } while (!OnlineService.IsAuthenicated);

            OnSignIn?.Invoke(OnlineService);

            // authenticated, now load inventory
            OnlineService.Load();

            // wait for load
            do
            {
                yield return null;

            } while (!OnlineService.IsLoaded);

            // load data to inventory
            DataSystem.LoadInventory(CurrentPlayer.Inventory, OnlineService.LoadedData, out lastLoadedInventoryData);

            OnGameInit?.Invoke(this);
        }

        void InitPools()
        {
            ObjectPool.Init();
            ParticlesPool.Init();
        }

        void InitLocalization()
        {
            csvLanguageTable = new CSVLanguageTable();
            csvLanguageTable.Parse(languageList.CSVLanguageTable);
        }

        /// <summary>
        /// Must be called after initialization 'AllWeaponsStats'
        /// </summary>
        void InitPlayer()
        {
            FindPlayer();

            // independent
            // init player, player's vehicle, weapons
            CurrentPlayer.Init();
            CurrentPlayer.gameObject.SetActive(false);

            // inventory;
            // depends on player and weapons stats
            CurrentPlayer.InitInventory();

            Inventory = CurrentPlayer.Inventory;

            defaultPlayerPosition = CurrentPlayer.transform.position;
        }

        void FindPlayer()
        {
            var players = FindObjectsOfType<Player>();
            Debug.Assert(players.Length <= 1, "Player class must be one on the scene", this);

            // if not exist create
            if (players.Length == 0)
            {
                CurrentPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity)
                    .GetComponent<Player>();
            }
            else
            {
                CurrentPlayer = players[0];
            }
        }
        #endregion

        void Start()
        {
            mainMenuBackground.SetActive(true);
            Background.CreateCutsceneBackground(Vector3.zero);

            if (OnlineService == null)
            {           
                // load data to inventory from file
                DataSystem.LoadInventory(CurrentPlayer.Inventory, out lastLoadedInventoryData);

                // if there is no service, everything is initialized
                OnGameInit?.Invoke(this);
            }

            Shop.OnAmmoBuy += Shop_OnAmmoBuy;
            Shop.OnWeaponBuy += Shop_OnWeaponBuy;
            Shop.OnWeaponRepair += Shop_OnWeaponRepair;
        }

        private void Shop_OnWeaponRepair(WeaponIndex index, int price)
        {
            OnlineService?.ReportProgress(GPGSIds.achievement_first_purchase, 100);
        }

        private void Shop_OnWeaponBuy(WeaponIndex index, int price)
        {
            OnlineService?.ReportProgress(GPGSIds.achievement_first_purchase, 100);
        }

        private void Shop_OnAmmoBuy(AmmunitionType index, int price)
        {
            OnlineService?.ReportProgress(GPGSIds.achievement_first_purchase, 100);
        }

        void Update()
        {
            spawnersController.Update();
        }

        void SaveInventoryOnBalanceChange(int oldBalance, int newBalance)
        {
            SaveInventory();
        }

        void SaveInventory()
        {
            if (OnlineService != null)
            {
                DataSystem.SaveInventory(CurrentPlayer.Inventory, OnlineService);
            }
            else
            {
                DataSystem.SaveInventory(CurrentPlayer.Inventory);
            }
        }

        /// <summary>
        /// Revert inventory to last loaded
        /// </summary>
        void RevertInventory()
        {
            if (Inventory == null || lastLoadedInventoryData == null)
            {
                return;
            }

            print("Reverting inventory");

            lastLoadedInventoryData.SaveTo((PlayerInventory)Inventory);
        }

        void SaveSettings()
        {
            DataSystem.SaveSettings(Settings);
        }

        void LoadSettings()
        {
            Settings = DataSystem.LoadSettings();
        }

        #region game start: cutscene / tutorial / gameplay
        /// <summary>
        /// Same method as Play(), but shows inventory menu,
        /// if cutscene and tutorial are disabled
        /// </summary>
        public void PlayWithInventoryMenu()
        {
            if (!Settings.GameShowCutscene && !Settings.GameShowTutorial)
            {
                ShowInventory();
            }
            else
            {
                Play();
            }
        }

        /// <summary>
        /// Starts / restarts gameplay. Processes cutscene and tutorial show,
        /// as specified in game settings
        /// </summary>
        public void Play()
        {
            mainMenuBackground.SetActive(false);

            if (Settings.GameShowCutscene)
            {
                Background.CreateCutsceneBackground(Vector3.zero);

                if (Settings.GameShowTutorial)
                {
                    // show cutscene after tutorial
                    ShowCutscene(ShowTutorial);
                }
                else
                {
                    // after cutscene:
                    // vehicle has zero speed,
                    // player has default position
                    ShowCutscene(() =>
                        {
                            ActivateGameplay(false, true);
                            CurrentPlayer.Vehicle.Accelerate();
                        });
                }
            }
            else
            {
                Background.Reinit();

                if (Settings.GameShowTutorial)
                {
                    ShowTutorial();
                }
                else
                {
                    // otherwise:
                    // default speed and default position
                    ActivateGameplay(true, true);
                }
            }
        }

        /// <summary>
        /// Play cutscene and tutorial (if needed)
        /// </summary>
        void ShowCutscene(Action onCutsceneEnd)
        {
            State = GameState.Cutscene;

            // disable player object
            CurrentPlayer.gameObject.SetActive(false);

            // don't play cutscene next time
            if (Settings.GameShowCutscene)
            {
                SettingsSystem.ChangeSetting(SettingsList.Setting_Key_Game_ShowCutscene);
            }

            cutsceneManager.Play(onCutsceneEnd);
        }

        void ShowTutorial()
        {
            State = GameState.Tutorial;

            // don't show tutorial next time
            if (Settings.GameShowTutorial)
            {
                SettingsSystem.ChangeSetting(SettingsList.Setting_Key_Game_ShowTutorial);
            }

            // at start of tutorial: player has zero speed and default position
            ActivateGameplay(false, false, false);

            // on the end of tutorial: enable spawners
            tutorialManager.StartTutorial(CurrentPlayer, spawnersController.RestartSpawn);
        }

        void ShowInventory()
        {
            OnInventoryOpen();
        }

        /// <summary>
        /// Activate player object, start enemy spawn
        /// </summary>
        /// <param name="defaultPosition">use default player position</param>
        /// <param name="defaultVehicleSpeed">if false, speed of player's vehicle will be zero</param>
        void ActivateGameplay(bool defaultVehicleSpeed, bool defaultPosition, bool activateSpawners = true)
        {
            State = GameState.Game;

            // restart player
            if (defaultPosition)
            {
                CurrentPlayer.Reinit(defaultPlayerPosition, defaultVehicleSpeed);
            }
            else
            {
                CurrentPlayer.Reinit(CurrentPlayer.transform.position, defaultVehicleSpeed);
            }

            if (activateSpawners)
            {
                spawnersController.RestartSpawn();
            }

            OnGameplayActivate();
        }
        #endregion

        void PauseGame()
        {
            if (State == GameState.Paused || State == GameState.Menu)
            {
                return;
            }

            State = GameState.Paused;
            OnGamePause();
        }

        void UnpauseGame()
        {
            if (State != GameState.Paused)
            {
                return;
            }

            State = GameState.Game;
            OnGameUnpause();
        }

        void EnableWeaponsSelection()
        {
            // if (Inventory.Weapons.ContainsAtLeastOne())
            {
                OnWeaponSelectionEnable();
            }
        }

        void DisableWeaponsSelection()
        {
            OnWeaponSelectionDisable();
        }

        /// <summary>
        /// When main menu must be activated
        /// </summary>
        void StopGame()
        {
            State = GameState.Menu;

            mainMenuBackground.SetActive(true);
            Background.CreateCutsceneBackground(Vector3.zero);
            
            CurrentPlayer.gameObject.SetActive(false);

            OnMainMenuActivate?.Invoke();
            OnGameEnd?.Invoke();
        }

        void ProcessPlayerDeath(GameScore score)
        {
            State = GameState.Menu;

            OnScoreSet?.Invoke(score, Inventory.PlayerStats.Score_Best);

            // add money
            CurrentPlayer.Inventory.Money += score.Money;

            OnlineService?.ReportProgress(GPGSIds.leaderboard_score, score.ActualScorePoints);
            OnlineService?.ReportProgress(GPGSIds.leaderboard_cash, score.Money);

            UpdateScoreAchievements();
            
            // scale down time
            StartCoroutine(WaitForGameEnd());

            OnPlayerDeath?.Invoke(CurrentPlayer);
        }

        void UpdateScoreAchievements()
        {
            OnlineService?.ReportProgress(GPGSIds.achievement_ten_thoudsand, Inventory.PlayerStats.Combat_TotalShots);
        }

        /// <summary>
        /// Scales down time scale and then reverts it, calls 'OnGameEnd' event
        /// </summary>
        IEnumerator WaitForGameEnd()
        {
            const float scale = 0.2f;
            const float toWait = 0.3f;

            timeController.SetTimeScale(scale);
            yield return new WaitForSeconds(toWait);

            timeController.SetDefault();

            OnGameEnd();
        }

        public void AddEnemyTarget(IEnemyTarget target)
        {
            EnemyTarget = target;
        }
    }
}
