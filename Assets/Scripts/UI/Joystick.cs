using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SD.UI
{
    // TODO: this class must be attached to movement field
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

        void Start()
        {
            // range is a half of parent's width
            movementRange = joystickBase.rect.width / 2;

            baseImage = joystickBase.GetComponent<Image>();
            dotImage = joystickDot.GetComponent<Image>();

            baseImage.enabled = false;
            dotImage.enabled = false;
        }

        void UpdateAxis(float delta)
        {
            // correct range according to canvas scale
            float actualRange = movementRange * canvas.scaleFactor;

            delta /= actualRange;
            inputController.UpdateMovementInput(delta);
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

            baseImage.enabled = true;
            dotImage.enabled = true;
        }
    }
}