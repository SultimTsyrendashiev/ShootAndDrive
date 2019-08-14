using UnityEngine;

namespace SD.UI.Controls
{
    class CutsceneSkipper : MonoBehaviour
    {
        public static event Void OnCutsceneSkip;

        [SerializeField]
        GameObject          skip;
        Animation           skipAnimation;
        float               skipHoldTime;
        float               neededTime;

        bool                isHolding;

        void Start()
        {
            skipAnimation = skip.GetComponent<Animation>();
            neededTime = skipAnimation.clip.length;

            PointerUp();
        }

        public void PointerDown()
        {
            isHolding = true;

            skip.SetActive(true);
            skipAnimation.Play(PlayMode.StopAll);
        }

        public void PointerUp()
        {
            isHolding = false;

            skip.SetActive(false);
            skipAnimation.Stop();

            skipHoldTime = 0;
        }

        void Update()
        {
            if (isHolding)
            {
                skipHoldTime += Time.unscaledDeltaTime;

                if (skipHoldTime > neededTime)
                {
                    SkipCutscene();
                }
            }
        }

        void SkipCutscene()
        {
            OnCutsceneSkip();
        }
    }
}
