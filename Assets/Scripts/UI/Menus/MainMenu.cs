using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SD.UI.Menus
{
    class MainMenu : MonoBehaviour, IMenu
    {
        public const string PlayerPrefs_FirstTimePlay = "FirstTimePlay";

        public void Play()
        {
            GameController.Instance.Play();
        }

        public void Init()
        { }

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}