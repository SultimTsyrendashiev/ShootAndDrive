using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SD.Game.Settings;

namespace SD.UI.Controls
{
    // This class must be attached to movement field
    // joystick circle and dot are just images 
    // that appears on startPostion and tracks position from OnDrag()
    public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField]
        Canvas canvas;
        [SerializeField]
        InputController inputController;

        [SerializeField]
        RectTransform joystickBase;
        [SerializeField]
        RectTransform joystickDot;
        
        Image baseImage;
        Image dotImage;

        Vector3 startPosition;
        float movementRange;

        bool drawImages;

        void Start()
        {
            // range is a half of parent's width
            movementRange = joystickBase.rect.width / 2;

            baseImage = joystickBase.GetComponent<Image>();
            dotImage = joystickDot.GetComponent<Image>();

            baseImage.enabled = false;
            dotImage.enabled = false;

            InitHandler();
        }

        void UpdateAxis(float delta)
        {
            // correct range according to canvas scale
            float actualRange = movementRange * canvas.scaleFactor;

            delta /= actualRange;
            inputController.UpdateMovementInput(delta);
        }

        void OnEnable()
        {
            if (baseImage != null && dotImage != null)
            {
                // when joystick field activates, return to default
                OnPointerUp(null);
            }
        }

        public void OnDrag(PointerEventData data)
        {
            // correct range according to canvas scale
            float actualRange = movementRange * canvas.scaleFactor;

            float delta = data.position.x - startPosition.x;
            delta = Mathf.Clamp(delta, -actualRange, actualRange);
            
            joystickDot.position = new Vector3(startPosition.x + delta, startPosition.y, startPosition.z);
            UpdateAxis(delta);
        }

        public void OnPointerUp(PointerEventData data)
        {
            joystickBase.position = startPosition;
            joystickDot.position = startPosition;
            UpdateAxis(0.0f);

            baseImage.enabled = false;
            dotImage.enabled = false;
        }

        public void OnPointerDown(PointerEventData data)
        {
            startPosition = data.position;

            joystickBase.position = startPosition;
            joystickDot.position = startPosition;

            baseImage.enabled = true && drawImages;
            dotImage.enabled = true && drawImages;
        }

        #region settings handler
        void InitHandler()
        {
            GameController.Instance.SettingsSystem.Subscribe(SettingsList.Setting_Key_HUD_Hide, Hide);
            drawImages = !GameController.Instance.Settings.HUDHide;

            baseImage.enabled = false;
            dotImage.enabled = false;
        }

        void Hide(GlobalSettings settings)
        {
            drawImages = !settings.HUDHide;
        }
        #endregion
    }
}