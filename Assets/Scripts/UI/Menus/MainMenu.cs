using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SD.UI.Menus
{
    class MainMenu : MonoBehaviour
    {
        public const string PlayerPrefs_FirstTimePlay = "FirstTimePlay";

        [SerializeField]
        GameObject playWithCutsceneBtn;
        [SerializeField]
        GameObject playBtn;

        void Start()
        {
            bool firstTimePlay = PlayerPrefs.GetInt(PlayerPrefs_FirstTimePlay, 1) != 0;

            playWithCutsceneBtn.SetActive(firstTimePlay);
            playBtn.SetActive(!firstTimePlay);
        }

        public void Play()
        {
            // TODO: start the game
        }

        public void PlayCutscene()
        {
            // TODO: start playing cutscene
        }
    }
}