using UnityEngine;
using UnityEngine.UI;

namespace SD.UI.Shop
{
    class WeaponsWorldUI : MonoBehaviour
    {
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

            // TODO: from settings
            int width = 192;
            int height = 192;

            // render it
            itemCamera.enabled = true;
            return itemCamera.GetImage(w, width, height);
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
