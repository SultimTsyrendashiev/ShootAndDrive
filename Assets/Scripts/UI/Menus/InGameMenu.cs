using UnityEngine;
using System.Collections;

namespace SD.UI.Menus
{
    class InGameMenu : AAnimatedMenu
    {
        protected override void DoInit()
        {
            GameController.OnGameplayActivate += ShowThisMenu;
            GameController.OnGameUnpause += ShowThisMenu;
        }

        protected override void DoDestroy()
        {
            GameController.OnGameplayActivate -= ShowThisMenu;
            GameController.OnGameUnpause -= ShowThisMenu;
        }
    }
}
