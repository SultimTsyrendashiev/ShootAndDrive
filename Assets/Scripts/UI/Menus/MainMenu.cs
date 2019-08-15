using System.Collections;
using UnityEngine;

namespace SD.UI.Menus
{
    class MainMenu : AAnimatedMenu
    {
        protected override void SignToEvents()
        {
            GameController.OnMainMenuActivate += ShowThisMenu;
        }

        protected override void UnsignFromEvents()
        {
            GameController.OnMainMenuActivate -= ShowThisMenu;
        }
    }
}
