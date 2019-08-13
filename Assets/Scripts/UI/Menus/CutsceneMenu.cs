using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SD.UI.Menus
{
    public class CutsceneMenu : MonoBehaviour, IMenu
    {
        [SerializeField]
        Animation   blackLinesAnimation;
        [SerializeField]
        string      enablingAnimation;
        [SerializeField]
        string      hidingAnimation;

        public void Init()
        { }

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
            yield return new WaitForSeconds(blackLinesAnimation.clip.length);
            gameObject.SetActive(false);
        }
    }
}
