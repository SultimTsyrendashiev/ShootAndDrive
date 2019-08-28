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
        const float WeaponsSelectionMultiplier = 0.05f;

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

        float                           defaultTimeScale;
        float                           defaultFixedDelta;


        // services
        public Player                   CurrentPlayer { get; private set; }

        public IInventory               Inventory { get; private set; }
        public IShop                    Shop { get; private set; }

        public IBackgroundController    Background { get; private set; }
        public IEnemyTarget             EnemyTarget { get; private set; }

        public SettingsSystem           SettingsSystem { get; private set; }

        SpawnersController              spawnersController;
        CutsceneManager                 cutsceneManager;
        TutorialManager                 tutorialManager;

        // stats
        public AllWeaponsStats          WeaponsStats { get; private set; }
        public AllAmmoStats             AmmoStats { get; private set; }

        // settings
        public GlobalSettings           Settings { get; private set; }
        public LanguageTable            Languages => csvLanguageTable.Languages;
        CSVLanguageTable                csvLanguageTable;

        #region events
        public static event Void                    OnGameplayActivate;
        public static event Void                    OnGamePause;
        public static event Void                    OnGameUnpause;
        public static event Void                    OnMainMenuActivate;
        public static event Void                    OnInventoryOpen;

        public static event Void                    OnWeaponSelectionEnable;
        public static event Void                    OnWeaponSelectionDisable;

        public static event PlayerDeath             OnPlayerDeath;

        /// <summary>
        /// Called when game ends, i.e. after some time from player death
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

            GlobalSettings.OnLanguageChange -= Dummy;
        }

        /// <summary>
        /// Init all systems
        /// </summary>
        void Init()
        {
            defaultTimeScale = Time.timeScale;
            defaultFixedDelta = Time.fixedDeltaTime;

            // to sure that there is at least one listener
            GlobalSettings.OnLanguageChange += Dummy;

            // load data from previous sessions
            LoadSettings();

            // init localization
            InitLanguages();


            // find objects
            Background              = FindObjectOfType<BackgroundController>();
            cutsceneManager         = FindObjectOfType<CutsceneManager>();
            tutorialManager         = FindObjectOfType<TutorialManager>();

            WeaponsStats            = new AllWeaponsStats(weaponsList.Data);
            AmmoStats               = new AllAmmoStats(ammoList.Data);

            spawnersController      = new SpawnersController();
            SettingsSystem          = new SettingsSystem(Settings);

            // check all systems
            Debug.Assert(WeaponsStats != null,                              "Can't find AllWeaponsStats", this);
            Debug.Assert(Background != null,                                "Can't find BackgroundController", this);
            Debug.Assert(spawnersController != null,                        "Can't find SpawnersController", this);
            Debug.Assert(cutsceneManager != null,                           "Can't find CutsceneManager", this);
            Debug.Assert(tutorialManager != null,                           "Can't find TutorialManager", this);
            Debug.Assert(FindObjectOfType<ObjectPool>() != null,            "Can't find ObjectPool", this);
            Debug.Assert(FindObjectOfType<ParticlesPool>() != null,         "Can't find ParticlesPool", this);

            InitPlayer();

            // inventory is loaded, create shop system
            Shop = new ShopSystem(Inventory);
            
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

        void InitLanguages()
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
            // disable player object
            CurrentPlayer.gameObject.SetActive(false);

            // don't play cutscene next time
            Settings.GameShowCutscene = false;

            cutsceneManager.Play(onCutsceneEnd);
        }

        void ShowTutorial()
        {
            // don't show tutorial next time
            Settings.GameShowTutorial = false;

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
        /// <param name="defaultVehicleSpeed">if false, player's vehicle will accelerate from zero speed</param>
        void ActivateGameplay(bool defaultVehicleSpeed, bool defaultPosition, bool activateSpawners = true)
        {
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

            Background.Reinit();

            OnGameplayActivate();
        }
        #endregion

        void PauseGame()
        {
            Time.timeScale = 0;
            OnGamePause();
        }

        void UnpauseGame()
        {
            Time.timeScale = 1;
            OnGameUnpause();
        }

        void EnableWeaponsSelection()
        {
            if (Inventory.Weapons.ContainsAtLeastOne())
            {
                Time.timeScale = defaultTimeScale * WeaponsSelectionMultiplier;
                Time.fixedDeltaTime = defaultFixedDelta * WeaponsSelectionMultiplier;

                OnWeaponSelectionEnable();
            }
        }

        void DisableWeaponsSelection()
        {
            Time.timeScale = defaultTimeScale;
            Time.fixedDeltaTime = defaultFixedDelta;

            OnWeaponSelectionDisable();
        }

        /// <summary>
        /// When main menu must be activated
        /// </summary>
        void StopGame()
        {
            Time.timeScale = defaultTimeScale;
            Time.fixedDeltaTime = defaultFixedDelta;

            mainMenuBackground.SetActive(true);
            CurrentPlayer.gameObject.SetActive(false);

            OnMainMenuActivate();
        }

        void ProcessPlayerDeath(GameScore score)
        {
            OnPlayerDeath(score);

            // add money
            CurrentPlayer.Inventory.Money += score.Money;

            // and save
            SaveInventory();

            // scale down time
            StartCoroutine(WaitForGameEnd());
        }

        /// <summary>
        /// Scales down time scale and then reverts it, calls 'OnGameEnd' event
        /// </summary>
        IEnumerator WaitForGameEnd()
        {
            const float scale = 0.2f;
            const float toWait = 0.3f;

            Time.timeScale = defaultTimeScale * scale;
            Time.fixedDeltaTime = defaultFixedDelta * scale;

            yield return new WaitForSeconds(toWait);

            Time.timeScale = defaultTimeScale;
            Time.fixedDeltaTime = defaultFixedDelta;

            OnGameEnd();

            //    float startScale = 0.15f;
            //    Time.timeScale = defaultTimeScale * startScale;
            //    Time.fixedDeltaTime = defaultFixedDelta * startScale;

            //    yield return new WaitForSeconds(1.0f);

            //    const float timeToWait = 0.25f;
            //    float waited = 0;

            //    while (waited < timeToWait)
            //    {
            //        yield return null;
            //        waited += Time.unscaledDeltaTime;

            //        if (waited > timeToWait)
            //        {
            //            waited = timeToWait;
            //        }

            //        float scale = 1 - waited / timeToWait;
            //        scale *= startScale;

            //        Time.timeScale = scale * defaultTimeScale;
            //        Time.fixedDeltaTime = scale * defaultFixedDelta;
            //    }

            //    Time.timeScale = 0;
            //    Time.fixedDeltaTime = 0;
            //}
        }

        public void AddEnemyTarget(IEnemyTarget target)
        {
            EnemyTarget = target;
        }

        void Dummy(string c)
        { }
    }
}
