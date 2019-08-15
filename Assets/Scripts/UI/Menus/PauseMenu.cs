using UnityEngine;
using UnityEngine.EventSystems;

namespace SD.UI.Menus
{
    class PauseMenu : AAnimatedMenu
    {
        protected override void SignToEvents()
        {
            GameController.OnGamePause += ShowThisMenu;
        }

        protected override void UnsignFromEvents()
        {
            GameController.OnGameplayActivate -= ShowThisMenu;
        }
    }
}
