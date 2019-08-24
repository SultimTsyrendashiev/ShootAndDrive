using System;
using UnityEngine;

namespace SD.UI.Shop
{
    /// <summary>
    /// Attach this script to weapon model that must be
    /// rendered by WeaponItemCamera
    /// </summary>
    class UIWeaponModel : MonoBehaviour
    {
        void Start()
        {
            // check name to parse
            if (!Enum.TryParse(gameObject.name, true, out WeaponIndex w))
            {
                Debug.Log("Name of game object with UIWeaponModel script can't be parsed", this);
            }
        }
    }
}
