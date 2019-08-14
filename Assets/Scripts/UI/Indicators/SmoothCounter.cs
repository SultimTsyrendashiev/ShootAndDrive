using UnityEngine;
using UnityEngine.UI;

namespace SD.UI.Indicators
{
    [RequireComponent(typeof(Text))]
    class SmoothCounter : MonoBehaviour
    {
        const float     DefaultCountTime = 1.5f;
        Text            text;

        int             target;
        float           current;

        bool            toCount;


        public float    CountTime { get; private set; }


        public void Set(int target, int from = 0, float countTime = DefaultCountTime)
        {
            this.target = target;
            this.current = from;
            this.CountTime = countTime;

            if (text == null)
            {
                text = GetComponent<Text>();
            }

            text.text = from.ToString();
        }

        public void StartCounting()
        {
            toCount = true;
        }

        void Update()
        {
            if (toCount)
            {
                float add = target * Time.unscaledDeltaTime / CountTime;
                current += add;

                if (current >= target)
                {
                    current = target;
                    toCount = false;
                }

                text.text = ((int)current).ToString();
            }
        }
    }
}
