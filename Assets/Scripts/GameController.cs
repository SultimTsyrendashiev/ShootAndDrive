using UnityEngine;
using SD.PlayerLogic;

namespace SD
{
    class GameController : MonoBehaviour
    {
        [SerializeField]
        GameObject playerPrefab;

        Player playerInstance;

        public Player CurrentPlayer => playerInstance;

        void Awake()
        {
            Application.targetFrameRate = 60;

            playerInstance = FindObjectOfType<Player>();

            // if not exist create
            if (playerInstance == null)
            {
                playerInstance = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity)
                    .GetComponent<Player>();
            }

            playerInstance.Init();
            
            // load items from player prefs
            playerInstance.Inventory.Load();

#if UNITY_EDITOR
            playerInstance.Inventory.GiveAll();
#endif
        }

        void Update()
        {
            UpdatePlayerControls();
            UpdateBackground();
        }

        void UpdatePlayerControls()
        {
            float x = UI.InputController.MovementHorizontal;
            playerInstance.UpdateInput(x);
        }

        void UpdateBackground()
        {
            Background.BackgroundController.Instance.UpdateCameraPosition(playerInstance.MainCamera.transform.position);
        }
    }
}
