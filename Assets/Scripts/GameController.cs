using UnityEngine;
using SD.PlayerLogic;
using SD.Weapons;

namespace SD
{
    class GameController : MonoBehaviour
    {
        [SerializeField]
        GameObject playerPrefab;

        public Player CurrentPlayer { get; private set; }
        public AllWeaponsStats WeaponsStats { get; private set; }

        void Awake()
        {
            Application.targetFrameRate = 60;

            InitPlayer();
            InitUI();

            InitWeaponsStats();
            InitPlayerInventory();
        }

        private void InitPlayerInventory()
        {
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

            // init player, player's vehicle, weapons
            CurrentPlayer.Init();
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
            Background.BackgroundController.Instance.UpdateCameraPosition(CurrentPlayer.MainCamera.transform.position);
        }

        void OnDestroy()
        {
            // save data from player's inventory
            CurrentPlayer.Inventory.Save();
        }
    }
}
