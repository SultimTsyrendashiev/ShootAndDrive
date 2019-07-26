using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SD.UI
{
    /// <summary>
    /// Sets child items' positions on a circle
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class ItemsPosition : MonoBehaviour
    {
        /// <summary>
        /// Start angle in degrees
        /// </summary>
        public float StartAngle;
        /// <summary>
        /// End angle in degrees
        /// </summary>
        public float EndAngle;

        public Vector2 Offset;
        public float Radius;

        void Start()
        {
            SetPositions();
        }

        public void SetPositions()
        {
            SetPositions(GetComponent<RectTransform>().childCount);
        }

        public void SetPositions(int count)
        {
            var thisTransform = GetComponent<RectTransform>();

            count = Mathf.Min(thisTransform.childCount, count);

            float deltaAngle = Mathf.Abs(EndAngle - StartAngle) / (count - 1);

            for (int i = 0; i < count; i++)
            {
                float angle = (StartAngle + i * deltaAngle) * Mathf.Deg2Rad;

                float x = Mathf.Cos(angle) * Radius;
                float y = Mathf.Sin(angle) * Radius;

                RectTransform child = (RectTransform)thisTransform.GetChild(i);
                child.anchoredPosition = new Vector2(x + Offset.x, y + Offset.y);
            }

            // deactivate other
            for (int i = thisTransform.childCount; i < count; i++)
            {
                thisTransform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
