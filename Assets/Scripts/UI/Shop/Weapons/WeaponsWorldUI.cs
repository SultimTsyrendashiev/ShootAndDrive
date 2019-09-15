using UnityEngine;
using UnityEngine.UI;

namespace SD.UI.Shop
{
    class WeaponsWorldUI : MonoBehaviour
    {
        [SerializeField]
        int size = 256;

        [SerializeField]
        WeaponItemCamera itemCamera;

        [SerializeField]
        UIWeaponContainer container;

        public RenderTexture GetImage(WeaponIndex w)
        {
            if (container == null)
            {
                return null;
            }

            // select weapon
            container.Select(w);

            // render it
            itemCamera.enabled = true;
            return itemCamera.GetImage(w, size, size);
        }

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}
