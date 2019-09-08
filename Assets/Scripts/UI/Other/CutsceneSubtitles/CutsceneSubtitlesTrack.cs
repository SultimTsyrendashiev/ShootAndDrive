using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SD.UI.CutsceneSubtitles
{
    [TrackBindingType(typeof(CutsceneSubtitleText))]
    [TrackClipType(typeof(CutsceneSubtitlesClip))]
    class CutsceneSubtitlesTrack : TrackAsset
    {

    }
}
