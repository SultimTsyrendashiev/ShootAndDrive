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

            DoInit();
        }

        void OnDestroy()
        {
            DoDestroy();
        }

        /// <summary>
        /// Called on init
        /// </summary>
        protected virtual void DoInit() { }
        /// <summary>
        /// Called on destroy
        /// </summary>
        protected virtual void DoDestroy() { }

        /// <summary>
        /// Called on menu activation, after starting an animation
        /// </summary>
        protected virtual void DoActivate() { }
        /// <summary>
        /// Called on menu deactivation, after the end of animation
        /// </summary>
        protected virtual void DoDeactivate() { }

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

            DoActivate();
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
                DoDeactivate();
            }
        }

        IEnumerator WaitForHide()
        {
            Animator.Play(hidingAnimation);
            yield return new WaitForSecondsRealtime(Animator.GetCurrentAnimatorStateInfo(0).length);

            gameObject.SetActive(false);
            DoDeactivate();
        }
    }
}
