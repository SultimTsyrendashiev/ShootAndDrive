using System;
using System.Collections;
using UnityEngine.Playables;
using UnityEngine;
using SD.UI.Controls;

namespace SD.Game
{
    /// <summary>
    /// All cutscene objects must be children of this one
    /// </summary>
    class CutsceneManager : MonoBehaviour
    {
        /// <summary>
        /// Called when cutscene started
        /// </summary>
        public static event Void OnCutsceneStart;

        [SerializeField]
        PlayableDirector    cutscene;

        /// <summary>
        /// Action after cutscene
        /// </summary>
        Action              onCutsceneEnd;

        float               endTime = -1;
        bool                isPlaying = false;


        public void Play(Action onCutsceneEnd)
        {
            ActivateCutsceneObjects(true);

            cutscene.Play();
            isPlaying = true;

            endTime = Time.time + (float)cutscene.duration;
            this.onCutsceneEnd = onCutsceneEnd;

            // sign to event to process forced skip
            CutsceneSkipper.OnCutsceneSkip += Stop;

            OnCutsceneStart();
        }

        void OnDestroy()
        {
            CutsceneSkipper.OnCutsceneSkip -= Stop;
        }

        void Update()
        {
            // if not playing or ended
            if (!isPlaying || Time.time < endTime)
            {
                return;
            }

            Stop();
        }

        void Stop()
        {
            isPlaying = false;

            // stop cutscene
            cutscene.Stop();

            // deactivate all objects assocated with cutscene
            ActivateCutsceneObjects(false);

            onCutsceneEnd?.Invoke();
        }

        void ActivateCutsceneObjects(bool active)
        {
            cutscene.gameObject.SetActive(active);
        }
    }
}
