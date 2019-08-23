using System.Collections;
using UnityEngine;

namespace SD.UI.Menus
{
    class MainMenu : AAnimatedMenu
    {
        protected override void DoInit()
        {
            GameController.OnMainMenuActivate += ShowThisMenu;
        }

        protected override void DoDestroy()
        {
            GameController.OnMainMenuActivate -= ShowThisMenu;
        }
    }
}
