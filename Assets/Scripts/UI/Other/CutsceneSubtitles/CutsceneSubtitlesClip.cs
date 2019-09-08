using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SD.UI.CutsceneSubtitles
{
    [Serializable]
    class CutsceneSubtitlesClip : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField]
        CutsceneSubtitleBehaviour template = new CutsceneSubtitleBehaviour();

        public ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<CutsceneSubtitleBehaviour>.Create(graph, template);
        }
    }
}
