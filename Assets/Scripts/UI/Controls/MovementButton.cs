using UnityEngine;
using UnityEngine.EventSystems;

namespace SD.UI.Controls
{
    class MovementButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        InputController inputController;

        [SerializeField]
        float movement;

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
            inputController.UpdateMovementInput(movement);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            inputController.UpdateMovementInput(0);
        }
    }
}
