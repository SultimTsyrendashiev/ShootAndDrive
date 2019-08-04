using System.Collections;
using UnityEngine;
using SD.PlayerLogic;
using SD.Weapons;
using SD.Enemies.Spawner;
using SD.Background;
using SD.UI;

namespace SD
{
    class GameController : MonoBehaviour
    {
        // position of player's spawn
        readonly Vector3            PlayerStartPoint = new Vector3(0,0,0);

        [SerializeField]
        GameObject                  playerPrefab;

        SpawnersController          spawnersController;

        float defaultTimeScale;
        float defaultFixedDelta;

        public Player CurrentPlayer { get; private set; }
        public AllWeaponsStats WeaponsStats { get; private set; }
        public BackgroundController Background { get; private set; }

        // events
        public static event Void    OnGamePause;
        public static event Void    OnGameUnpause;

        void Awake()
        {
            Application.targetFrameRate = 60;

            Init();
            SignToEvents();
        }

        void OnDestroy()
        {
            // save data from player's inventory
            CurrentPlayer.Inventory.Save();

            UnsignFromEvents();
        }

        void SignToEvents()
        {
            CurrentPlayer.OnPlayerDeath += ProcessPlayerDeath;
            InputController.OnPause += PauseGame;
            InputController.OnUnpause += UnpauseGame;
        }

        void UnsignFromEvents()
        {
            CurrentPlayer.OnPlayerDeath -= ProcessPlayerDeath;
            InputController.OnPause -= PauseGame;
            InputController.OnUnpause -= UnpauseGame;
        }

        /// <summary>
        /// Init all systems
        /// </summary>
        void Init()
        {
            defaultTimeScale = Time.timeScale;
            defaultFixedDelta = Time.fixedDeltaTime;

            // TODO:
            // in 'Init' method of each IPooledObject must not be references to next systems
            InitPools();

            // independent
            InitWeaponsStats();
            // independent (depends on ObjectPool)
            InitBackground();
            // independent
            InitEnemySpawners();
            // independent
            InitPlayer();

            // depends on player and weapons
            //InitUI();

            // depends on player, weapons and its stats
            InitPlayerInventory();
        }

        void InitPools()
        {
            ObjectPool objectPool = FindObjectOfType<ObjectPool>();
            ParticlesPool particlesPool = FindObjectOfType<ParticlesPool>();

            objectPool.Init();
            particlesPool.Init();
        }

        void Start()
        {
            const float startSpawnerDistance = 100;

            spawnersController.StartSpawn(
                CurrentPlayer.transform.position + CurrentPlayer.transform.forward * startSpawnerDistance,
                CurrentPlayer.transform);
        }

        private void InitBackground()
        {
            Background = FindObjectOfType<BackgroundController>();
            Debug.Assert(Background != null, "Can't find BackgroundController", this);

            // independent
            Background.Init();
        }

        private void InitEnemySpawners()
        {
            spawnersController = FindObjectOfType<SpawnersController>();
            Debug.Assert(spawnersController != null, "Can't find SpawnersController", this);

            // independent
            spawnersController.Init();
        }

        private void InitPlayerInventory()
        {
            // depends on player and weapons stats
            CurrentPlayer.InitInventory();
            CurrentPlayer.Inventory.Load();

            CurrentPlayer.Inventory.GiveAll();
        }

        //void InitUI()
        //{
        //    ui = FindObjectOfType<UIController>();
        //    Debug.Assert(ui != null, "Can't find UIController", this);

        //    // depends on player and weapons
        //    ui.Init(CurrentPlayer);
        //}

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

            CurrentPlayer.transform.position = PlayerStartPoint;

            // independent
            // init player, player's vehicle, weapons
            CurrentPlayer.Init(Background);
        }

        void InitWeaponsStats()
        {
            WeaponsStats = GetComponent<AllWeaponsStats>();

            // don't assert as weapon system can be unused
            if (WeaponsStats == null)
            {
                Debug.Log("Can't find AllWeaponsStats class", this);
                return;
            }

            // independent
            WeaponsStats.Init();
        }

        void Update()
        {
            UpdateBackground();
        }

        void UpdateBackground()
        {
            Background.UpdateCameraPosition(CurrentPlayer.MainCamera.transform.position);
        }

        public void PauseGame()
        {
            Time.timeScale = 0;
            OnGamePause();
        }

        public void UnpauseGame()
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
            // and save inventory
            CurrentPlayer.Inventory.Save();

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
    }
}
