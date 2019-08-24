using System;
using System.Collections.Generic;
using UnityEngine;

namespace SD.UI.Shop
{
    class UIWeaponContainer : MonoBehaviour
    {
        Dictionary<WeaponIndex, GameObject> items;
        GameObject current;

        void Init()
        {
            var aitems = GetComponentsInChildren<UIWeaponModel>(true);

            items = new Dictionary<WeaponIndex, GameObject>();

            foreach (var a in aitems)
            {
                if (Enum.TryParse(a.name, true, out WeaponIndex w))
                {
                    items.Add(w, a.gameObject);
                }
            }
        }

        /// <summary>
        /// Activate weapon item, if exist
        /// </summary>
        public void Select(WeaponIndex w)
        {
            if (items == null)
            {
                Init();
            }

            var values = items.Values;

            foreach (var v in values)
            {
                v.SetActive(false);
            }

            if (items.ContainsKey(w))
            {
                GameObject item = items[w];
                item.SetActive(true);

                current = item;
            }
        }

        /// <summary>
        /// Rotate current
        /// </summary>
        /// <param name="deltax"></param>
        /// <param name="deltay"></param>
        public void Rotate(float deltax, float deltay)
        {
            current?.transform.Rotate(deltax, deltay, 0);
        }
    }
}
