using System;
using System.Collections;
using UnityEngine.Playables;
using UnityEngine;

namespace SD.Game
{
    /// <summary>
    /// All cutscene objects must be children of this one
    /// </summary>
    class CutsceneManager : MonoBehaviour
    {
        [SerializeField]
        PlayableDirector cutscene;

        Action onCutsceneEnd;
        float endTime;

        public void Play(Action onCutsceneEnd)
        {
            ActivateCutsceneObjects(true);

            cutscene.Play();

            endTime = Time.time + (float)cutscene.duration;
            this.onCutsceneEnd = onCutsceneEnd;
        }

        void Update()
        {
            if (Time.time < endTime)
            {
                return;
            }

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
