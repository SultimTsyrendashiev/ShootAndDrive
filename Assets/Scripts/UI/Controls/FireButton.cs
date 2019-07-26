using UnityEngine;
using UnityEngine.EventSystems;

namespace SD.UI.Controls
{
    class FireButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
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
            inputController.FireDown();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            inputController.FireUp();
        }
    }
}
