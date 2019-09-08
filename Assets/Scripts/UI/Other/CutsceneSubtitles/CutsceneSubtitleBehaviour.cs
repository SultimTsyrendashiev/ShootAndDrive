using System;
using UnityEngine;
using UnityEngine.Playables;

namespace SD.UI.CutsceneSubtitles
{
    [Serializable]
    class CutsceneSubtitleBehaviour : PlayableBehaviour
    {
        [SerializeField]
        string subtitleTranslationKey;

        CutsceneSubtitleText currentSubtitle;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            currentSubtitle = (CutsceneSubtitleText)playerData;

            if (currentSubtitle == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(subtitleTranslationKey))
            {
                Debug.Log("CutsceneSubtitleBehaviour::Empty translation key. Frame ID: " + info.frameId);
                return;
            }

            currentSubtitle.SetTranslationKey(subtitleTranslationKey);

            base.ProcessFrame(playable, info, playerData);
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            currentSubtitle?.Clear();
        }
    }
}
