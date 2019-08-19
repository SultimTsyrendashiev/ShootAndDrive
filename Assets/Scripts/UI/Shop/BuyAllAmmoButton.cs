using UnityEngine;
using UnityEngine.EventSystems;

namespace SD.UI.Shop
{
    class BuyAllAmmoButton : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            try
            {
                GetComponentInParent<ShopItemAmmo>().BuyThisAll();
            }
            catch
            {
                Debug.Log("ShopItemAmmo must be a parent of this component", this);
            }
        }
    }
}
