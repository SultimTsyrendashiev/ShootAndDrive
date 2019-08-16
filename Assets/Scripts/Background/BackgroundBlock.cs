using UnityEngine;

namespace SD.Background
{
    class BackgroundBlock : MonoBehaviour, IBackgroundBlock, IPooledObject
    {
        const float Epsilon = 1.5f;

        [SerializeField]
        float width = 10;
        [SerializeField]
        float length = 20;

        public float Length => length;
        public GameObject CurrentObject => gameObject;

        public Vector3 Center { get => transform.position; set => transform.position = value; }

        public GameObject ThisObject => gameObject;
        public PooledObjectType Type => PooledObjectType.Important;
        public int AmountInPool => 4;

        public void Init()
        { }

        public void Reinit()
        { }

        public bool Contains(Vector3 position)
        {
            return (position.x - Center.x) <= (width / 2 + Epsilon)
                && (position.z - Center.z) <= (length / 2 + Epsilon);
        }

        public Vector2 GetHorizontalBounds()
        {
            float center = transform.position.x;

            float left = center - width / 2;
            float right = center + width / 2;

            return new Vector2(left, right);
        }

        public float GetMinZ()
        {
            float center = transform.position.z;
            return center - length / 2;
        }

        public float GetMaxZ()
        {
            float center = transform.position.z;
            return center + length / 2;
        }
    }
}
