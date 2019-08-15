using UnityEngine;
using System.Collections;

namespace SD.UI.Menus
{
    abstract class AAnimatedMenu : MonoBehaviour, IMenu
    {
        Animator animator;
        [SerializeField]
        string          enablingAnimation;
        [SerializeField]
        string          hidingAnimation;

        protected MenuController MenuController { get; private set; }

        public void Init(MenuController menuController)
        {
            MenuController = menuController;
            animator = GetComponent<Animator>();

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
            StopAllCoroutines();

            gameObject.SetActive(true);
            animator?.Play(enablingAnimation);
        }

        public void Deactivate()
        {
            if (animator != null)
            {
                StopAllCoroutines();
                StartCoroutine(WaitForHide());
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        IEnumerator WaitForHide()
        {
            animator.Play(hidingAnimation);
            yield return new WaitForSecondsRealtime(animator.GetCurrentAnimatorStateInfo(0).length);

            gameObject.SetActive(false);
        }
    }
}
