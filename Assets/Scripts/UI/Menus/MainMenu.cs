using System.Collections;
using UnityEngine;

namespace SD.UI.Controls
{
    class MainMenu : MonoBehaviour, IMenu
    {
        MenuController  menuController;

        [SerializeField]
        Animation       menuAnimation;
        [SerializeField]
        string          enablingAnimation;
        [SerializeField]
        string          hidingAnimation;


        public void Init(MenuController menuController)
        {
            this.menuController = menuController;
            GameController.OnMainMenuActivate += ShowThisMenu;
        }

        void OnDestroy()
        {
            GameController.OnMainMenuActivate -= ShowThisMenu;
        }

        void ShowThisMenu()
        {
            menuController.EnableMenu(gameObject.name);
        }

        public void Activate()
        {
            gameObject.SetActive(true);
            menuAnimation.Play(enablingAnimation, PlayMode.StopAll);
        }

        public void Deactivate()
        {
            menuAnimation.Play(hidingAnimation, PlayMode.StopAll);
        }

        IEnumerator WaitForDisable()
        {
            yield return new WaitForSeconds(menuAnimation[hidingAnimation].length);
            gameObject.SetActive(false);
        }
    }
}
