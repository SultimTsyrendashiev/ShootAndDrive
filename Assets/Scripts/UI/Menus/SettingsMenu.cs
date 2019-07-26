using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SD.UI.Menus
{
    class SettingsMenu : MonoBehaviour
    {
        [SerializeField]
        GameObject[] submenuList;
        [SerializeField]
        string startSubmenu = "Game";

        Dictionary<string, GameObject> submenus;

        void Awake()
        {
            Init();
        }

        void Init()
        {
            submenus = new Dictionary<string, GameObject>();

            foreach (var g in submenuList)
            {
                submenus.Add(g.name, g);
                g.SetActive(g.name == startSubmenu);
            }
        }

        public void EnableSubmenu(string submenu)
        {
            // disable all other menus
            foreach (var m in submenus.Keys)
            {
                submenus[m].SetActive(m == submenu);
            }
        }
    }
}
