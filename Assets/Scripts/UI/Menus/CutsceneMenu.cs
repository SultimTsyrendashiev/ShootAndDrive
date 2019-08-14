using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SD.Game;

namespace SD.UI.Menus
{
    class CutsceneMenu : MonoBehaviour, IMenu
    {
        MenuController menuController;

        [SerializeField]
        Animation   blackLinesAnimation;
        [SerializeField]
        string      enablingAnimation;
        [SerializeField]
        string      hidingAnimation;

        public void Init(MenuController menuController)
        {
            this.menuController = menuController;

            // when cutscene starts, enable this menu
            CutsceneManager.OnCutsceneStart += EnableCutscene;
        }

        void OnDestroy()
        {
            CutsceneManager.OnCutsceneStart -= EnableCutscene;
        }

        void EnableCutscene()
        {
            menuController.EnableMenu(gameObject.name);
        }

        public void Activate()
        {
            gameObject.SetActive(true);
            blackLinesAnimation.Play(enablingAnimation, PlayMode.StopAll);
        }

        public void Deactivate()
        {
            blackLinesAnimation.Play(hidingAnimation, PlayMode.StopAll);
            
            // wait until animation ends,
            // and only then disable game object
            StartCoroutine(WaitForDisable());
        }

        IEnumerator WaitForDisable()
        {
            yield return new WaitForSeconds(blackLinesAnimation[hidingAnimation].length);
            gameObject.SetActive(false);
        }
    }
}
