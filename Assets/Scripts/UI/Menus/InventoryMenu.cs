using SD.Game;

namespace SD.UI.Menus
{
    class InventoryMenu : AAnimatedMenu
    {
        protected override void DoInit()
        {
            GameController.OnInventoryOpen += ShowThisMenu;
        }

        protected override void DoDestroy()
        {
            GameController.OnInventoryOpen -= ShowThisMenu;
        }
    }
}
