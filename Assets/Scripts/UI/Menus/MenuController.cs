using System;
using System.Collections.Generic;
using UnityEngine;

namespace SD.UI
{
    class MenuController : MonoBehaviour
    {
        //public enum Menu
        //{
        //    Main,
        //    Settings,
        //    Pause,
        //    Score,
        //    Inventory,
        //    Curtscene,
        //    InGame
        //}

        [SerializeField]
        GameObject[] menuList;
        [SerializeField]
        string startMenu = "MainMenu";

        Dictionary<string, GameObject> menus;
        string currentMenu;
        string previousMenu;

        void Awake()
        {
            Init();
        }

        void Init()
        {
            menus = new Dictionary<string, GameObject>();

            foreach (var g in menuList)
            {
                menus.Add(g.name, g);
                g.SetActive(g.name == startMenu);
            }
        }

        /// <summary>
        /// Enable new menu and disable other ones
        /// </summary>
        public void EnableMenu(string newMenu)
        {
            if (currentMenu == newMenu)
            {
                return;
            }

            previousMenu = currentMenu;
            currentMenu = newMenu;

            // disable all other menus
            foreach (var m in menus.Keys)
            {
                menus[m].SetActive(m == newMenu);
            }
        }

        public void EnablePreviousMenu()
        {
            Debug.Assert(!string.IsNullOrEmpty(previousMenu), "There was no previous menu. Current menu: " + currentMenu, this);
            EnableMenu(previousMenu);
        }
    }
}
