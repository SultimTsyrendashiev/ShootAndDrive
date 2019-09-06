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
        public static event Action OnCutsceneStart;

        /// <summary>
        /// Called when cutscene ended
        /// </summary>
        public static event Action OnCutsceneEnd;


        [SerializeField]
        PlayableDirector[] cutscenes;

        /// <summary>
        /// All UI objects that are in cutscene,
        /// they will be enabled on start and disabled on end
        /// </summary>
        [SerializeField]
        GameObject[] cutsceneUiObjects;

        int currentCutscene = -1;
        int cutscenesAmount;

        /// <summary>
        /// Action after cutscene
        /// </summary>
        Action              afterCutscene;
        float               endTimeOfCurrentCutscene;

        float               endTime = -1;

        bool                isPlaying = false;


        public void Play(Action onCutsceneEnd)
        {
            isPlaying = true;

            endTime = Time.time + GetOverallDuration();
            cutscenesAmount = cutscenes.Length;

            // enable ui objects
            foreach (GameObject uiObj in cutsceneUiObjects)
            {
                uiObj.SetActive(true);
            }

            PlayCutscene(0);

            this.afterCutscene = onCutsceneEnd;

            // sign to event to process forced skip
            CutsceneSkipper.OnCutsceneSkip += Stop;

            OnCutsceneStart?.Invoke();
        }

        void PlayCutscene(int index)
        {
            if (index < 0 || index >= cutscenesAmount)
            {
                return;
            }

            currentCutscene = index;

            for (int i = 0; i < cutscenes.Length; i++)
            {
                if (i != index)
                {
                    // stop cutscene
                    cutscenes[i].Stop();

                    // deactivate all objects assocated with cutscene
                    ActivateCutsceneObjects(cutscenes[i], false);
                }
            }

            // activate current
            ActivateCutsceneObjects(cutscenes[index], true);
            cutscenes[index].Play();

            endTimeOfCurrentCutscene = Time.time + (float)cutscenes[index].duration;
        }

        void OnDestroy()
        {
            CutsceneSkipper.OnCutsceneSkip -= Stop;
        }

        void Update()
        {
            if (!isPlaying)
            {
                return;
            }

            // if previous cutscene ended and there is next,
            // start next
            if (Time.time > endTimeOfCurrentCutscene
                && currentCutscene < cutscenesAmount
                && currentCutscene >= 0)
            {
                PlayCutscene(currentCutscene + 1);
            }

            if (Time.time < endTime)
            {
                return;
            }

            Stop();
        }

        void Stop()
        {
            isPlaying = false;
            currentCutscene = -1;

            // disable ui objects
            foreach (GameObject uiObj in cutsceneUiObjects)
            {
                uiObj.SetActive(false);
            }

            foreach (PlayableDirector c in cutscenes)
            {
                // deactivate all objects assocated with cutscene
                ActivateCutsceneObjects(c, false);

                c.time = 0;

                // stop cutscene
                c.Stop();

                // evaluate 1 frame
                c.Evaluate();
            }

            afterCutscene?.Invoke();

            OnCutsceneEnd?.Invoke();
        }

        static void ActivateCutsceneObjects(PlayableDirector cutsceneObject, bool active)
        {
            cutsceneObject.gameObject.SetActive(active);
        }

        float GetOverallDuration()
        {
            float overallDuration = 0;

            foreach (var c in cutscenes)
            {
                overallDuration += (float)c.duration;
            }

            return overallDuration;
        }
    }
}
