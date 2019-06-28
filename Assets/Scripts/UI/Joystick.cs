using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SD.UI
{
    // TODO: this class must be attached to movement field
    // joystick circle and dot are just images 
    // that appears on startPostion and tracks position from OnDrag()
    public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField]
        InputController inputController;

        Vector3 startPosition;
        float movementRange;

        void Start()
        {
            // range is a half of parent's width
            movementRange = transform.parent.GetComponent<RectTransform>().rect.width / 2;
        }

        void OnEnable()
        {
            startPosition = transform.position;
        }

        void UpdateAxis(float delta)
        {
            delta /= movementRange;
            inputController.UpdateMovementInput(delta);
        }

        public void OnDrag(PointerEventData data)
        {
            float delta = data.position.x - startPosition.x;
            delta = Mathf.Clamp(delta, -movementRange, movementRange);

            transform.position = new Vector3(startPosition.x + delta, startPosition.y, startPosition.z);
            UpdateAxis(delta);
        }

        public void OnPointerUp(PointerEventData data)
        {
            transform.position = startPosition;
            UpdateAxis(0.0f);
        }

        public void OnPointerDown(PointerEventData data) { }

        ///// <summary>
        ///// Called on pointer down on movement field
        ///// </summary>
        //public void OnMovementDown(BaseEventData data)
        //{
        //    // only joystick
        //    Debug.Assert(UIController.Instance.MovementInputType == MovementInputType.Joystick);

        //    PointerEventData pointerData = (PointerEventData)data;
        //    UIController.Instance.ActivateJoystick(pointerData.position);
        //}

        ///// <summary>
        ///// Called on pointer up from movement field
        ///// </summary>
        //public void OnMovementUp()
        //{
        //    UIController.Instance.DeactivateJoystick();
        //}

    }
}