using UnityEngine;
using UnityEngine.Audio;
using SD.Game.Settings;
using System;

namespace SD.Game
{
    class AudioManager : MonoBehaviour
    {
        [SerializeField]
        AudioMixer audioMixer;

        //[SerializeField]
        //AudioMixerSnapshot game;
        //[SerializeField]
        //AudioMixerSnapshot menu;

        [SerializeField]
        string soundVolumeParamName = "SoundVolume";
        [SerializeField]
        string musicVolumeParamName = "MusicVolume";

        [SerializeField]
        string soundPitchParamName = "SoundPitch";

        TimeController timeController;

        void Start()
        {
            // default
            EnableMenu();

            GameController.OnGamePause += EnableMenu;
            GameController.OnMainMenuActivate += EnableMenu;

            GameController.OnGameUnpause += EnableGame;
            GameController.OnGameplayActivate += EnableGame;

            GameController.OnPlayerDeath += PlayerDied;
        }

        public void SetTimeContoller(TimeController timeController)
        {
            this.timeController = timeController;
            timeController.OnTimeScaleChange += ProcessTimeScale;
        }

        void OnDestroy()
        {
            GameController.OnGamePause -= EnableMenu;
            GameController.OnMainMenuActivate -= EnableMenu;

            GameController.OnGameUnpause -= EnableGame;
            GameController.OnGameplayActivate -= EnableGame;

            GameController.OnPlayerDeath -= PlayerDied;

            if (timeController != null)
            {
                timeController.OnTimeScaleChange -= ProcessTimeScale;
            }
        }

        void EnableMenu() => SetMenuSnapshot();
        void EnableGame() => SetGameSnapshot();
        void PlayerDied(PlayerLogic.Player p) => SetMenuSnapshot(1);

        void SetMenuSnapshot(float transitionTime = 0.01f)
        {
            //menu.TransitionTo(transitionTime);
        }

        void SetGameSnapshot(float transitionTime = 0.01f)
        {
            //game.TransitionTo(transitionTime);
        }

        /// <summary>
        /// Set music volume
        /// </summary>
        /// <param name="value">music volume in [0..1]</param>
        public void SetMusicVolume(float value)
        {
            audioMixer.SetFloat(musicVolumeParamName, GetVolume(value));
        }

        /// <summary>
        /// Set sound volume
        /// </summary>
        /// <param name="value">sound volume in [0..1]</param>
        public void SetSoundVolume(float value)
        {
            audioMixer.SetFloat(soundVolumeParamName, GetVolume(value));
        }

        /// <summary>
        /// Set sound pitch
        /// </summary>
        /// <param name="pitch">pitch in [0..1]</param>
        void SetSoundPitch(float pitch)
        {
            audioMixer.SetFloat(soundPitchParamName, pitch * 100);
        }

        /// <summary>
        /// Volume from [0..1] to db
        /// </summary>
        float GetVolume(float value)
        {
            const float MinDB = -80;

            return value > 0 ? 20 * Mathf.Log(value) : MinDB;
        }

        /// <summary>
        /// Scales audio sources' pitch
        /// </summary>
        /// <param name="scale"></param>
        void ProcessTimeScale(float scale)
        {
            if (scale != 0)
            {
                if (AudioListener.pause)
                {
                    AudioListener.pause = false;
                }

                audioMixer.SetFloat(soundPitchParamName, scale);
            }
            else
            {
                AudioListener.pause = true;
            }
        }
    }
}
