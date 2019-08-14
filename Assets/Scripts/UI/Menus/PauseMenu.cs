using UnityEngine;
using UnityEngine.EventSystems;

namespace SD.UI.Controls
{
    class PauseMenu : MonoBehaviour, IMenu
    {
        MenuController menuController;

        public void Init(MenuController menuController)
        {
            this.menuController = menuController;
            GameController.OnGamePause += ShowThisMenu;
        }

        void OnDestroy()
        {
            GameController.OnGameplayActivate -= ShowThisMenu;
        }

        void ShowThisMenu()
        {
            menuController.EnableMenu(gameObject.name);
        }

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}
