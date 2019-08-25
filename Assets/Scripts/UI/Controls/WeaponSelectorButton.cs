using UnityEngine;
using UnityEngine.EventSystems;

namespace SD.UI.Controls
{
    class WeaponSelectorButton : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField]
        InputController inputController;

        public void OnPointerDown(PointerEventData eventData)
        {
            inputController.OnWeaponSelectorDown();
        }
    }
}
