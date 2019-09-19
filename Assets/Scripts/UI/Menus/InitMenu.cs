using UnityEngine;

namespace SD.UI.Menus
{
    /// <summary>
    /// Initialization menu. When game is not loaded, this menu will be active.
    /// When game loads, init settings will appear (if game was started for first time).
    /// </summary>
    class InitMenu : MonoBehaviour, IMenu
    {
        [SerializeField]
        string              nextMenu = "MainMenu";

        [SerializeField]
        GameObject          loadingScreen;

        /// <summary>
        /// Game object that contains start settings:
        /// f.e. language setup when game is started at first time
        /// </summary>
        [SerializeField]
        GameObject          initSettings;

        bool                gameIsInited;

        MenuController      menuController;

        public void Init(MenuController menuController)
        {
            this.menuController = menuController;

            loadingScreen.SetActive(true);
            initSettings.SetActive(false);

            gameIsInited = false;
            GameController.OnGameInit += DisableOnGameInit;
        }

        void OnDestroy()
        {
            GameController.OnGameInit -= DisableOnGameInit;
        }

        void DisableOnGameInit(GameController obj)
        {
            gameIsInited = true;

            // if first start, show init settings
            if (obj.Settings.FirstStart)
            {
                loadingScreen.SetActive(false);
                initSettings.SetActive(true);
            }
            else
            {
                Deactivate();
            }
        }

        /// <summary>
        /// Call this method when init settings are set
        /// </summary>
        public void OnInitSettingsSet()
        {
            if (!gameIsInited)
            {
                return;
            }

            Deactivate();
        }

        public void Activate()
        {
            // don't activate, if already inited
            if (gameIsInited && !initSettings.activeSelf)
            {
                return;
            }

            gameObject.SetActive(true);
        }

        /// <summary>
        /// Deactivates object and enables next
        /// </summary>
        public void Deactivate()
        {
            gameObject.SetActive(false);
            menuController.EnableMenu(nextMenu);
        }
    }
}
