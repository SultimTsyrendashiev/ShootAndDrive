using UnityEngine;
using UnityEngine.EventSystems;

namespace SD.UI.Controls
{
    class PauseButton : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField]
        InputController inputController;

        void Start()
        {
            // try to find if not set
            if (!inputController)
            {
                inputController = GetComponentInParent<InputController>();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            inputController.Pause();
        }
    }
}
