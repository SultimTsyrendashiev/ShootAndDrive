using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SD.UI.Menus
{
    class InventoryMenu : MonoBehaviour, IMenu
    {
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

        public void Play()
        {

        }
    }
}
