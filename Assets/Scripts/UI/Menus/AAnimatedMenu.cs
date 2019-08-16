using UnityEngine;
using System.Collections;

namespace SD.UI.Menus
{
    /// <summary>
    /// Has enabling and hiding animation
    /// </summary>
    abstract class AAnimatedMenu : MonoBehaviour, IMenu
    {
        [SerializeField]
        string          enablingAnimation;
        [SerializeField]
        string          hidingAnimation;

        Coroutine       hidingCoroutine;

        protected MenuController MenuController { get; private set; }
        protected Animator Animator { get; private set; }

        public void Init(MenuController menuController)
        {
            MenuController = menuController;
            Animator = GetComponent<Animator>();

            SignToEvents();
        }

        void OnDestroy()
        {
            UnsignFromEvents();
        }

        protected virtual void SignToEvents() { }
        protected virtual void UnsignFromEvents() { }

        public void ShowThisMenu()
        {
            MenuController.EnableMenu(gameObject.name);
        }

        public void Activate()
        {
            if (hidingCoroutine != null)
            {
                StopCoroutine(hidingCoroutine);
                hidingCoroutine = null;
            }

            gameObject.SetActive(true);

            if (!string.IsNullOrEmpty(enablingAnimation))
            {
                PlayActivationAnimation(enablingAnimation);
            }
        }

        protected virtual void PlayActivationAnimation(string animName)
        {
            Animator?.Play(animName);
        }

        public void Deactivate()
        {
            if (Animator != null && !string.IsNullOrEmpty(enablingAnimation))
            {
                if (hidingCoroutine != null)
                {
                    StopCoroutine(hidingCoroutine);
                    hidingCoroutine = null;
                }

                hidingCoroutine = StartCoroutine(WaitForHide());
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        IEnumerator WaitForHide()
        {
            Animator.Play(hidingAnimation);
            yield return new WaitForSecondsRealtime(Animator.GetCurrentAnimatorStateInfo(0).length);

            gameObject.SetActive(false);
        }
    }
}
