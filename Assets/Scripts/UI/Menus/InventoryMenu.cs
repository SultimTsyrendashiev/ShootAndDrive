using SD.Game;

namespace SD.UI.Menus
{
    class InventoryMenu : AAnimatedMenu
    {
        protected override void SignToEvents()
        {
            GameController.OnInventoryOpen += ShowThisMenu;
        }

        protected override void UnsignFromEvents()
        {
            GameController.OnInventoryOpen -= ShowThisMenu;
        }
    }
}
