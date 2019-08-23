using SD.Game;

namespace SD.UI.Menus
{
    class CutsceneMenu : AAnimatedMenu
    {
        protected override void DoInit()
        {
            CutsceneManager.OnCutsceneStart += ShowThisMenu;
        }

        protected override void DoDestroy()
        {
            CutsceneManager.OnCutsceneStart -= ShowThisMenu;
        }
    }
}
