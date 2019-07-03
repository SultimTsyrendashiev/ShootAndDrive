using UnityEngine;

namespace SD.Background
{
    class BackgroundBlock : MonoBehaviour, IBackgroundBlock
    {
        [SerializeField]
        float width = 10;
        [SerializeField]
        float length = 20;

        public float Length => length;
        public Vector3 Center => transform.position;
        public GameObject CurrentOject => gameObject;

        public bool Contains(Vector3 position)
        {
            return (position.x - Center.x) <= width / 2
                && (position.z - Center.z) <= length / 2;
        }

        public Vector2 GetHorizontalBounds()
        {
            float center = transform.position.x;

            float left = center - width / 2;
            float right = center + width / 2;

            return new Vector2(left, right);
        }
    }
}
