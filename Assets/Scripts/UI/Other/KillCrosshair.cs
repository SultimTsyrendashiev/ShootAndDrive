using UnityEngine;
using UnityEngine.UI;
using SD.PlayerLogic;

namespace SD.UI
{
    /// <summary>
    /// Shows crosshair when enemy dies
    /// </summary>
    [RequireComponent(typeof(Image))]
    class KillCrosshair : MonoBehaviour
    {
        const float Duration = 0.3f;

        Image crosshairImage;

        Camera playerCamera;
        Transform target;

        float hideTime;

        void Awake()
        {
            crosshairImage = GetComponent<Image>();
            playerCamera = GameController.Instance.CurrentPlayer.MainCamera;
            gameObject.SetActive(false);

            GameController.Instance.CurrentPlayer.OnKill += ShowCrosshair;
        }

        void OnDestroy()
        {
            GameController.Instance.CurrentPlayer.OnKill -= ShowCrosshair;
        }

        void ShowCrosshair(Transform enemyTransform)
        {
            gameObject.SetActive(true);
            target = enemyTransform;
            hideTime = Time.time + Duration;
        }

        void Update()
        {
            if (Time.time < hideTime && target != null)
            {
                // update crosshair position
                transform.position = playerCamera.WorldToScreenPoint(target.position);
            }
            else if (gameObject.activeSelf)
            {
                // otherwise, hide
                gameObject.SetActive(false);
                target = null;
            }
        }
    }
}
