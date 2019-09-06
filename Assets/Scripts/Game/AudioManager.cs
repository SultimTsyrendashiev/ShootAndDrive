using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using SD.Game.Settings;

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

        const float GameSfxVolumeMult = 1.0f;
        const float MenuSfxVolumeMult = 0.0f;
        float currentSfxVolumeMult;
        float targetSfxVolumeMult;
        float currentTransitionSpeed;
        bool isTransited;
        float soundVolume;

        [SerializeField]
        string soundVolumeParamName = "SoundVolume";
        [SerializeField]
        string uiVolumeParamName = "UIVolume";
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

        void SetMenuSnapshot(float transitionTime = 0)
        {
            if (transitionTime == 0)
            {
                currentSfxVolumeMult = targetSfxVolumeMult = MenuSfxVolumeMult;
                return;
            }

            targetSfxVolumeMult = MenuSfxVolumeMult;
            currentTransitionSpeed = 1.0f / transitionTime;

            //menu.TransitionTo(transitionTime);
        }

        void SetGameSnapshot(float transitionTime = 0)
        {
            if (transitionTime == 0)
            {
                currentSfxVolumeMult = targetSfxVolumeMult = GameSfxVolumeMult;
                return;
            }

            targetSfxVolumeMult = GameSfxVolumeMult;
            currentTransitionSpeed = 1.0f / transitionTime;

            //game.TransitionTo(transitionTime);
        }

        /// <summary>
        /// Process volume transition
        /// </summary>
        void Update()
        {
            if (!isTransited)
            {
                return;
            }

            const float Epsilon = 0.01f;

            if (Mathf.Abs(targetSfxVolumeMult - currentSfxVolumeMult) < Epsilon)
            {
                currentSfxVolumeMult = Mathf.Lerp(currentSfxVolumeMult, targetSfxVolumeMult, Time.unscaledDeltaTime * currentTransitionSpeed);
            }
            else
            {
                currentSfxVolumeMult = targetSfxVolumeMult;
                isTransited = false;
            }
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
        /// Set UI volume
        /// </summary>
        /// <param name="value">ui volume in [0..1]</param>
        void SetUIVolume(float value)
        {
            audioMixer.SetFloat(uiVolumeParamName, GetVolume(value));
        }

        /// <summary>
        /// Set sound volume
        /// </summary>
        /// <param name="value">sound volume in [0..1]</param>
        public void SetSoundVolume(float value)
        {
            soundVolume = value;

            SetSoundVolumeRaw(value);
            SetUIVolume(value);
        }

        void SetSoundVolumeRaw(float value)
        {
            audioMixer.SetFloat(soundVolumeParamName, GetVolume(value));
        }

        /// <summary>
        /// Set sound pitch
        /// </summary>
        /// <param name="pitch">pitch in [0..1]</param>
        void SetSoundPitch(float pitch)
        {
            audioMixer.SetFloat(soundPitchParamName, pitch);
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

                SetSoundPitch(scale);
            }
            else
            {
                AudioListener.pause = true;
            }
        }
    }
}
