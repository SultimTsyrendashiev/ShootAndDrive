﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace SD.UI
{
    class MenuController : MonoBehaviour
    {
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

                var menu = g.GetComponent<IMenu>();
                if (menu != null)
                {
                    menu.Init(this);
                }
                else
                {
                    // to call Awake on all objects, even on deactivated
                    g.SetActive(true);
                }
            }

            EnableMenu(startMenu);
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
                // call if there is IMenu component
                if (m == previousMenu)
                {
                    var menuComp = menus[m].GetComponent<IMenu>();

                    if (menuComp != null)
                    {
                        menuComp.Deactivate();
                        continue;
                    }
                }
                else if (m == currentMenu)
                {
                    var menuComp = menus[m].GetComponent<IMenu>();

                    if (menuComp != null)
                    {
                        menuComp.Activate();
                        continue;
                    }
                }

                // if there is no special component,
                // just disable it
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
