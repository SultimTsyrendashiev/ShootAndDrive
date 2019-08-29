using SD.Game.Settings;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SD.UI.Controls
{
    class MovementButtons : MonoBehaviour
    {
        [SerializeField]
        InputController inputController;

        [SerializeField]
        RectTransform leftButton;
        [SerializeField]
        RectTransform rightButton;

        [SerializeField]
        float defaultDistance;
        [SerializeField]
        float defaultSize;

        bool isInitialized;

        void Start()
        {
            isInitialized = true;

            GameController.Instance.SettingsSystem.Subscribe(SettingsList.Setting_Key_Input_MoveBtnsSize, SetMovementBtnsSize);
            GameController.Instance.SettingsSystem.Subscribe(SettingsList.Setting_Key_Input_MoveBtnsDistance, SetMovementBtnsDistance);
            GameController.Instance.SettingsSystem.Subscribe(SettingsList.Setting_Key_HUD_Hide, Hide);
        }

        void OnEnable()
        {
            if (!isInitialized)
            {
                var settings = GameController.Instance.Settings;

                SetMovementBtnsSize(settings);
                SetMovementBtnsDistance(settings);
                Hide(settings);
            }
        }

        void OnDestroy()
        {
            GameController.Instance.SettingsSystem.Unsubscribe(SettingsList.Setting_Key_Input_MoveBtnsSize, SetMovementBtnsSize);
            GameController.Instance.SettingsSystem.Unsubscribe(SettingsList.Setting_Key_Input_MoveBtnsDistance, SetMovementBtnsDistance);
            GameController.Instance.SettingsSystem.Unsubscribe(SettingsList.Setting_Key_HUD_Hide, Hide);
        }

        void SetMovementBtnsSize(GlobalSettings settings)
        {
            float s = settings.InputMovementBtnsSize * defaultSize;
            Vector2 size = new Vector2(s, s);

            leftButton.sizeDelta = size;
            rightButton.sizeDelta = size;
        }

        void SetMovementBtnsDistance(GlobalSettings settings)
        {
            var t = GetComponent<RectTransform>();
            Vector2 size = t.sizeDelta;

            size.x = settings.InputMovementBtnsDistance * defaultDistance;

            t.sizeDelta = size;
        }

        void Hide(GlobalSettings settings)
        {
            leftButton.GetComponent<Image>().enabled = !settings.HUDHide;
            rightButton.GetComponent<Image>().enabled = !settings.HUDHide;
        }

        public void PressLeft()
        {
            inputController.UpdateMovementInput(-1);
        }

        public void PressRight()
        {
            inputController.UpdateMovementInput(1);
        }

        public void Unpress()
        {
            inputController.UpdateMovementInput(0);
        }
    }
}
