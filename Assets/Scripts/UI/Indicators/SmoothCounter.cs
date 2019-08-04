using UnityEngine;
using UnityEngine.UI;

namespace SD.UI.Indicators
{
    [RequireComponent(typeof(Text))]
    class SmoothCounter : MonoBehaviour
    {
        const float TimeForCount = 1.5f;

        Text    text;

        int     target;
        float   current;
        bool    toCount;

        void Start()
        {
            text = GetComponent<Text>();
        }

        public void Set(int target)
        {
            this.target = target;
            current = 0;
        }

        public void StartCounting()
        {
            toCount = true;
        }

        void Update()
        {
            if (toCount)
            {
                float add = target * Time.unscaledDeltaTime / TimeForCount;
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
