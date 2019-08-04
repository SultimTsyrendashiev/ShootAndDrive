using UnityEngine;
using UnityEngine.EventSystems;

namespace SD.UI.Controls
{
    class ContinueButton : MonoBehaviour, IPointerClickHandler
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

        public void OnPointerClick(PointerEventData eventData)
        {
            inputController.Unpause();
        }
    }
}
