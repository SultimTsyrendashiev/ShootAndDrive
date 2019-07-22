using UnityEngine;
using SD.PlayerLogic;
using SD.Weapons;
using SD.Enemies.Spawner;
using SD.Background;
using System;

namespace SD
{
    class GameController : MonoBehaviour
    {
        readonly Vector3 PlayerStartPoint = new Vector3(0,0,0);

        [SerializeField]
        GameObject playerPrefab;

        SpawnersController spawnersController;

        public Player               CurrentPlayer { get; private set; }
        public AllWeaponsStats      WeaponsStats { get; private set; }
        public BackgroundController Background { get; private set; }

        void Awake()
        {
            Application.targetFrameRate = 60;

            Init();
            SignToEvents();
        }

        void SignToEvents()
        {
            CurrentPlayer.OnPlayerDeath += ProcessPlayerDeath;
        }

        void ProcessPlayerDeath(GameScore score)
        {
            // TODO: 
            // 1) death screen, wait 1 sec;
            // on tap - skip waiting;
            // 2) show menu: score, menu, restart
        }


        /// <summary>
        /// Init all systems
        /// </summary>
        void Init()
        {
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
            InitUI();

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

            spawnersController.StartCoroutine(spawnersController.StartSpawn(
                CurrentPlayer.transform.position + CurrentPlayer.transform.forward * startSpawnerDistance,
                CurrentPlayer.transform));
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

            // load items from player prefs
            CurrentPlayer.Inventory.Load();

#if UNITY_EDITOR
            CurrentPlayer.Inventory.GiveAll();
#endif
        }

        void InitUI()
        {
            var ui = FindObjectOfType<UI.UIController>();
            Debug.Assert(ui != null, "Can't find UIController", this);

            // depends on player and weapons
            ui.Init(CurrentPlayer);
        }

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
            UpdatePlayerControls();
            UpdateBackground();
        }

        void UpdatePlayerControls()
        {
            float x = UI.InputController.MovementHorizontal;
            CurrentPlayer.UpdateInput(x);
        }

        void UpdateBackground()
        {
            Background.UpdateCameraPosition(CurrentPlayer.MainCamera.transform.position);
        }

        void OnDestroy()
        {
            // save data from player's inventory
            CurrentPlayer.Inventory.Save();
        }
    }
}
