using UnityEngine;

namespace SD.UI.Shop
{
    /// <summary>
    /// Attach this script to the camera
    /// that renders weapon model in a shop, weapon switch menu, etc
    /// </summary>
    [RequireComponent(typeof(Camera))]
    class WeaponItemCamera : MonoBehaviour
    {
        const int CameraDepth = 128;
        const int RenderTextureMaxSize = 1024;

        Camera thisCamera;

        //public void Deactivate()
        //{
        //    gameObject.SetActive(false);
        //}

        //public void Activate()
        //{
        //    gameObject.SetActive(true);
        //}

        //public bool TryGetRenderTexture(out RenderTexture renderTexture)
        //{
        //    if (!gameObject.activeInHierarchy)
        //    {
        //        return false;
        //    }
        //}

        /// <summary>
        /// Get render texture of weapon item
        /// </summary>
        public RenderTexture GetImage(WeaponIndex w, int width, int height)
        {
            if (width > RenderTextureMaxSize)
            {
                width = RenderTextureMaxSize;
            }

            if (height > RenderTextureMaxSize)
            {
                height = RenderTextureMaxSize;
            }

            RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 16);
            renderTexture.antiAliasing = 4;
            renderTexture.autoGenerateMips = true;

            if (thisCamera == null)
            {
                thisCamera = GetComponent<Camera>();
            }

            // render to texture
            thisCamera.targetTexture = renderTexture;
            thisCamera.Render();

            thisCamera.targetTexture = null;

            return renderTexture;
        }

        /// <summary>
        /// Set this camera to render on main display
        /// </summary>
        public void SetAsMain(bool active)
        {
            gameObject.SetActive(active);
            thisCamera.targetTexture = null;

            if (active)
            {
                thisCamera.depth = CameraDepth;
            }
            else
            {
                return;
            }
        }
    }
}
