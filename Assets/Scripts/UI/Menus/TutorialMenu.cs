using UnityEngine;
using SD.Game;

namespace SD.UI.Menus
{
    class TutorialMenu : MonoBehaviour, IMenu
    {
        public void Init(MenuController menuController)
        {
            TutorialManager.OnTutorialStart += Activate;
        }

        public void Activate()
        {
            throw new System.NotImplementedException();
        }

        public void Deactivate()
        {
            throw new System.NotImplementedException();
        }

    }
}
