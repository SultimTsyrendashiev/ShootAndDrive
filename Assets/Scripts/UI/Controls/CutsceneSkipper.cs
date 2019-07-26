using UnityEngine;

namespace SD.UI.Controls
{
    class CutsceneSkipper
    {
        [SerializeField]
        MenuController mainMenuController;
        [SerializeField]
        string nextMenuName = "InGame";

        [SerializeField]
        GameObject  skip;
        Animation   skipAnimation;
        float       skipHoldTime;
        float       neededTime;

        void Start()
        {
            skipAnimation = skip.GetComponent<Animation>();
            neededTime = skipAnimation.clip.length;
        }

        void OnEnable()
        {
            skipHoldTime = 0;
            skip.SetActive(false);
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (skipHoldTime == 0)
                {
                    skip.SetActive(true);
                    skipAnimation.Play(PlayMode.StopAll);
                }

                skipHoldTime += Time.unscaledDeltaTime;

                if (skipHoldTime > neededTime)
                {
                    SkipCutscene();
                }
            }
            else
            {
                if (skipHoldTime == 0)
                {
                    skip.SetActive(false);
                    skipAnimation.Stop();
                }

                skipHoldTime = 0;
            }
        }

        void SkipCutscene()
        {
            mainMenuController.EnableMenu(nextMenuName);
        }
    }
}
