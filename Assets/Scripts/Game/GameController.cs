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

namespace SD
{
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
        GameObject                      mainMenuBackground;



        Vector3                         defaultPlayerPosition;


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
            if (Instance != null)
            {
                Debug.Log("Several game controllers. Destroying: ", this);

                // deactivate
                Destroy(this);
            }

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
            
            InputController.OnPause += PauseGame;
            InputController.OnUnpause += UnpauseGame;
            InputController.OnPlayButton += Play;
            InputController.OnPlayWithInventoryButton += PlayWithInventoryMenu;
            InputController.OnMainMenuButton += StopGame;
            InputController.OnInventoryButton += ShowInventory;
            InputController.OnWeaponSelectionButton += EnableWeaponsSelection;
            InputController.OnWeaponSelectionDisableButton += DisableWeaponsSelection;
        }

        void UnsignFromEvents()
        {
            CurrentPlayer.OnPlayerDeath -= ProcessPlayerDeath;

            InputController.OnPause -= PauseGame;
            InputController.OnUnpause -= UnpauseGame;
            InputController.OnPlayButton -= Play;
            InputController.OnPlayWithInventoryButton -= PlayWithInventoryMenu;
            InputController.OnMainMenuButton -= StopGame;
            InputController.OnInventoryButton -= ShowInventory;
            InputController.OnWeaponSelectionButton -= EnableWeaponsSelection;
            InputController.OnWeaponSelectionDisableButton -= DisableWeaponsSelection;
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


            // find objects
            Background              = FindObjectOfType<BackgroundController>();
            cutsceneManager         = FindObjectOfType<CutsceneManager>();
            tutorialManager         = FindObjectOfType<TutorialManager>();
            audioManager            = FindObjectOfType<AudioManager>();

            WeaponsStats            = new AllWeaponsStats(weaponsList.Data);
            AmmoStats               = new AllAmmoStats(ammoList.Data);

            spawnersController      = new SpawnersController();
            SettingsSystem          = new SettingsSystem(Settings);
            Shop                    = new ShopSystem();
            audioSettingsHandler    = new AudioSettingsHandler();
            timeController          = new TimeController(Time.fixedDeltaTime);


            // check all systems
            Debug.Assert(WeaponsStats != null,                              "Can't find AllWeaponsStats",           this);
            Debug.Assert(Background != null,                                "Can't find BackgroundController",      this);
            Debug.Assert(spawnersController != null,                        "Can't find SpawnersController",        this);
            Debug.Assert(cutsceneManager != null,                           "Can't find CutsceneManager",           this);
            Debug.Assert(tutorialManager != null,                           "Can't find TutorialManager",           this);
            Debug.Assert(FindObjectOfType<ObjectPool>() != null,            "Can't find ObjectPool",                this);
            Debug.Assert(FindObjectOfType<ParticlesPool>() != null,         "Can't find ParticlesPool",             this);
            Debug.Assert(audioManager != null,                              "Can't find AudioManager");


            SettingsInitializer.Init(SettingsSystem, Settings, audioManager, audioSettingsHandler, timeController);

            InitPlayer();

            // inventory is loaded, init shop system
            Shop.Init(Inventory);
            
            // all systems initialized, sign up to events
            SignToEvents();
            
            // at last, init object and particle pools
            InitPools();
        }

        void InitPools()
        {
            FindObjectOfType<ObjectPool>().Init();
            FindObjectOfType<ParticlesPool>().Init();
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

            DataSystem.LoadInventory(CurrentPlayer.Inventory);

            Inventory = CurrentPlayer.Inventory;

            Inventory.Money = 200000;

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
        }

        void Update()
        {
            spawnersController.Update();
        }

        void SaveInventory()
        {
            DataSystem.SaveInventory(CurrentPlayer.Inventory);
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
            SaveInventory();

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
            if (State != GameState.Game)
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
            if (State == GameState.Menu)
            {
                return;
            }

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

            // and save
            SaveInventory();

            // scale down time
            StartCoroutine(WaitForGameEnd());

            OnPlayerDeath?.Invoke(CurrentPlayer);
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

        void Dummy(string c)
        { }
    }
}
