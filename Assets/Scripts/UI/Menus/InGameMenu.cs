using UnityEngine;
using System.Collections;

namespace SD.UI.Menus
{
    class InGameMenu : AAnimatedMenu
    {
        protected override void SignToEvents()
        {
            GameController.OnGameplayActivate += ShowThisMenu;
            GameController.OnGameUnpause += ShowThisMenu;
        }

        protected override void UnsignFromEvents()
        {
            GameController.OnGameplayActivate -= ShowThisMenu;
            GameController.OnGameUnpause -= ShowThisMenu;
        }
    }
}
