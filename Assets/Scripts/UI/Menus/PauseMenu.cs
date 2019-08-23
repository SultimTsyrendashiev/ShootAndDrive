using UnityEngine;
using UnityEngine.EventSystems;

namespace SD.UI.Menus
{
    class PauseMenu : AAnimatedMenu
    {
        protected override void DoInit()
        {
            GameController.OnGamePause += ShowThisMenu;
        }

        protected override void DoDestroy()
        {
            GameController.OnGamePause -= ShowThisMenu;
        }
    }
}
