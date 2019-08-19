using UnityEngine;
using UnityEngine.EventSystems;

namespace SD.UI.Shop
{
    class BuyAmmoButton : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            try
            {
                GetComponentInParent<ShopItemAmmo>().BuyThis();
            }
            catch
            {
                Debug.Log("ShopItemAmmo must be a parent of this component", this);
            }
        }
    }
}
