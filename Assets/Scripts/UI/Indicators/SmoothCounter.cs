using UnityEngine;
using UnityEngine.UI;

namespace SD.UI.Indicators
{
    [RequireComponent(typeof(Text))]
    class SmoothCounter : MonoBehaviour
    {
        const float     DefaultCountTime = 1.5f;
        Text            text;

        int             from;
        int             target;
        float           current;

        bool            toCount;

        string          format;

        public float    CountTime { get; private set; }


        public void Set(int target, int from = 0, string format = null, float countTime = DefaultCountTime)
        {
            this.target = target;
            this.current = from;
            this.from = from;
            this.CountTime = countTime;
            this.format = format;

            if (text == null)
            {
                text = GetComponent<Text>();
            }

            UpdateValue(from);
        }

        public void StartCounting()
        {
            toCount = true;
        }

        void Update()
        {
            if (toCount)
            {
                float add = (target - from) * Time.unscaledDeltaTime / CountTime;
                current += add;

                if (current >= target)
                {
                    current = target;
                    toCount = false;
                }

                UpdateValue((int)current);
            }
        }

        void UpdateValue(int value)
        {
            if (string.IsNullOrEmpty(format))
            {
                text.text = value.ToString();
            }
            else
            {
                text.text = string.Format(format, value);
            }
        }
    }
}
