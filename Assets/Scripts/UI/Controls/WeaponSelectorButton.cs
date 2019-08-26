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
            //// check if player have any weapons
            //if (GameController.Instance != null && GameController.Instance.Inventory != null)
            //{
            //    GameController.Instance.Inventory.Weapons.Get
            //}

            inputController.OnWeaponSelectorDown();
        }
    }
}
