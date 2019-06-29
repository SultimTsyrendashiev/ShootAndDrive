using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SD.UI
{
    /// <summary>
    /// Sets child items' positions on a circle
    /// </summary>
    public class ItemsPosition : MonoBehaviour
    {
        /// <summary>
        /// Start angle in degrees
        /// </summary>
        [SerializeField]
        private float startAngle;
        /// <summary>
        /// End angle in degrees
        /// </summary>
        [SerializeField]
        private float endAngle;

        [SerializeField]
        private Vector2 offset;

        [SerializeField]
        private float radius;

        void Start()
        {
            SetPositions();
        }

        void SetPositions()
        {
            var thisTransform = GetComponent<RectTransform>();

            float deltaAngle = Mathf.Abs(endAngle - startAngle) / (thisTransform.childCount - 1);

            for (int i = 0; i < thisTransform.childCount; i++)
            {
                float angle = (startAngle + i * deltaAngle) * Mathf.Deg2Rad;

                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;

                RectTransform child = (RectTransform)thisTransform.GetChild(i);
                child.anchoredPosition = new Vector2(x + offset.x, y + offset.y);
            }
        }
    }
}
