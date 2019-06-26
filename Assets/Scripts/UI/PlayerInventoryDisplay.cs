using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SD.Player;
using SD.Weapons;

namespace SD.UI
{
    class PlayerInventoryDisplay : MonoBehaviour
    {
        [SerializeField]
        private Transform ammoTextsParent;
        [SerializeField]
        private Transform itemTextsParent;

        Dictionary<AmmoType, Text> ammoTexts;
        Dictionary<ItemType, Text> itemTexts;

        void Awake()
        {
            ammoTexts = new Dictionary<AmmoType, Text>();
            itemTexts = new Dictionary<ItemType, Text>();

            Init(ammoTexts, ammoTextsParent);
            Init(itemTexts, itemTextsParent);
        }

        void OnEnable()
        {
            UpdateText();
        }

        void UpdateText()
        {
            var inv = Player.Player.Instance.Inventory;

            foreach (AmmoType a in Enum.GetValues(typeof(AmmoType)))
            {
                SetText(a, inv.Ammo[a].ToString());
            }

            foreach (ItemType a in Enum.GetValues(typeof(ItemType)))
            {
                SetText(a, inv.Items[a].ToString());
            }
        }

        /// <summary>
        /// Parse game objects and add them to dictionary
        /// </summary>
        /// <param name="ts">parent of transforms</param>
        void Init<T>(Dictionary<T, Text> texts, Transform ts) where T : Enum
        {
            foreach (Transform t in ts)
            {
                T i = (T)Enum.Parse(typeof(T), t.name);
                texts.Add(i, t.GetComponentInChildren<Text>(true));
            }

            Debug.Assert(texts.Keys.Count == Enum.GetValues(typeof(T)).Length);
        }

        /// <summary>
        /// Set text of Text component for given ammotype
        /// </summary>
        void SetText(AmmoType t, string text)
        {
            ammoTexts[t].text = text;
        }
        /// <summary>
        /// Set text of Text component for given item
        /// </summary>
        void SetText(ItemType t, string text)
        {
            itemTexts[t].text = text;
        }
    }
}
