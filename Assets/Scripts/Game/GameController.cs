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

namespace SD
{
    class GameController : MonoBehaviour
    {
        [SerializeField]
        LanguageList                    languageList;

        [SerializeField]
        GameObject                      playerPrefab;

        float                           defaultTimeScale;
        float                           defaultFixedDelta;


        // instances
        public Player                   CurrentPlayer { get; private set; }
        public IBackgroundController    Background { get; private set; }
        public IEnemyTarget             EnemyTarget { get; private set; }
        SpawnersController              spawnersController;
        CutsceneManager                 cutsceneManager;

        // settings, stats
        public AllWeaponsStats          WeaponsStats { get; private set; }
        public GlobalSettings           Settings { get; private set; }
        public LanguageTable            Languages => csvLanguageTable.Languages;
        CSVLanguageTable                csvLanguageTable;

        // events
        public static event Void        OnGamePause;
        public static event Void        OnGameUnpause;

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

        void OnDestroy()
        {
            SaveData();
            UnsignFromEvents();
        }

        void SignToEvents()
        {
            CurrentPlayer.OnPlayerDeath += ProcessPlayerDeath;
            InputController.OnPause += PauseGame;
            InputController.OnUnpause += UnpauseGame;

            // to sure that there is at least one listener
            GlobalSettings.OnLanguageChange += Dummy;
        }

        void UnsignFromEvents()
        {
            CurrentPlayer.OnPlayerDeath -= ProcessPlayerDeath;
            InputController.OnPause -= PauseGame;
            InputController.OnUnpause -= UnpauseGame;

            GlobalSettings.OnLanguageChange -= Dummy;
        }

        /// <summary>
        /// Init all systems
        /// </summary>
        void Init()
        {
            defaultTimeScale = Time.timeScale;
            defaultFixedDelta = Time.fixedDeltaTime;

            // load data from previous sessions
            LoadData();

            // init multilingual
            InitLanguages();

            // find object with stats
            WeaponsStats            = FindObjectOfType<AllWeaponsStats>();
            Background              = FindObjectOfType<BackgroundController>();
            spawnersController      = FindObjectOfType<SpawnersController>();
            cutsceneManager         = FindObjectOfType<CutsceneManager>();

            // check all systems
            Debug.Assert(WeaponsStats != null,                              "Can't find AllWeaponsStats", this);
            Debug.Assert(Background != null,                                "Can't find BackgroundController", this);
            Debug.Assert(spawnersController != null,                        "Can't find SpawnersController", this);
            Debug.Assert(cutsceneManager != null,                           "Can't find CutsceneManager", this);
            Debug.Assert(FindObjectOfType<ObjectPool>() != null,            "Can't find ObjectPool", this);
            Debug.Assert(FindObjectOfType<ParticlesPool>() != null,         "Can't find ParticlesPool", this);

            InitPlayer();

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

            // player must be faced to world forward
            CurrentPlayer.transform.forward = Vector3.forward;

            // independent
            // init player, player's vehicle, weapons
            CurrentPlayer.Init();

            // inventory;
            // depends on player and weapons stats
            CurrentPlayer.InitInventory();
            DataSystem.LoadInventory(CurrentPlayer.Inventory);
        }
        #endregion

        void SaveData()
        {
            DataSystem.SaveInventory(CurrentPlayer.Inventory);
            DataSystem.SaveSettings(Settings);
        }

        void LoadData()
        {
            Settings = DataSystem.LoadSettings();
        }

        void Start()
        {
            Play();
        }

        void StartEnemySpawn()
        {
            const float startSpawnerDistance = 200;
            spawnersController.StartSpawn(CurrentPlayer.transform.position + CurrentPlayer.transform.forward * startSpawnerDistance);
        }

        public void Play()
        {
            // temp
            CurrentPlayer.Inventory.GiveAll();

            if (Settings.GameShowCutscene)
            {
                // don't play cutscene next time
                Settings.GameShowCutscene = false;

                // cutscene and tutorial
                PlayWithCutscene();
            }
            else if (Settings.GameShowTutorial)
            {
                ShowTutorial();
            }

            StartEnemySpawn();
        }

        /// <summary>
        /// Play cutscene and tutorial (if needed)
        /// </summary>
        void PlayWithCutscene()
        {
            // if tutorial must be shown, send tutorial method
            if (Settings.GameShowTutorial)
            {
                cutsceneManager.Play(ShowTutorial);
            }
            else
            {
                cutsceneManager.Play(null);
            }
        }

        void ShowTutorial()
        {

        }

        void PauseGame()
        {
            Time.timeScale = 0;
            OnGamePause();
        }

        void UnpauseGame()
        {
            Time.timeScale = 1;
            // OnGameUnpause();
        }

        void ProcessPlayerDeath(GameScore score)
        {
            // (no) death screen, wait 1 sec;
            // (no) on tap - skip waiting;

            // add money
            CurrentPlayer.Inventory.Money += score.Money;
            // and save
            SaveData();

            // scale down time
            StartCoroutine(WaitForScaleTime());
        }

        /// <summary>
        /// Scales down time scale and then reverts it
        /// </summary>
        IEnumerator WaitForScaleTime()
        {
            const float scale = 0.2f;
            const float toWait = 0.3f;

            Time.timeScale = defaultTimeScale * scale;
            Time.fixedDeltaTime = defaultFixedDelta * scale;

            yield return new WaitForSeconds(toWait);

            Time.timeScale = defaultTimeScale;
            Time.fixedDeltaTime = defaultFixedDelta;

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
