using System;
using System.Collections.Generic;
using UnityEngine;
using SD.UI.Controls;

namespace SD.UI.Indicators
{
    class WeaponsList : MonoBehaviour
    {
        [SerializeField]
        InputController inputController;

        [SerializeField]
        AmmoList ammoList;

        [SerializeField]
        GameObject weaponBtnPrefab;

        Dictionary<WeaponIndex, WeaponButton> buttons;
        IInventory inventory;

        void Start()
        {
            inventory = GameController.Instance.Inventory;
            buttons = new Dictionary<WeaponIndex, WeaponButton>();

            var inScene = GetComponentsInChildren<WeaponButton>();
            foreach (var i in inScene)
            {
                Destroy(i.gameObject);
            }

            foreach (WeaponIndex a in Enum.GetValues(typeof(WeaponIndex)))
            {
                GameObject o = Instantiate(weaponBtnPrefab, transform);
                buttons.Add(a, o.GetComponent<WeaponButton>());
            }

            UpdateList();
        }

        void OnEnable()
        {
            if (buttons == null)
            {
                return;
            }

            UpdateList();
        }

        void UpdateList()
        {
            IWeaponsHolder weapons = inventory.Weapons;
            IAmmoHolder ammo = inventory.Ammo;

            List<WeaponIndex> available = weapons.GetAvailableWeaponsInGame();
            int availableAmount = available.Count;

            int counter = 0;

            foreach (WeaponButton b in buttons.Values)
            {
                if (counter < availableAmount)
                {
                    IWeaponItem w = weapons.Get(available[counter]);
                    IAmmoItem a = ammo.Get(w.AmmoType);

                    if (!w.IsAmmo || (w.IsAmmo && a.CurrentAmount > 0))
                    {
                        b.Set(w, a, SelectWeapon, ammoList.HighlightAmmo, ammoList.UnhighlightAll);
                    }
                    else
                    {
                        b.gameObject.SetActive(false);
                    }
                }
                else
                {
                    // disable other buttons
                    b.gameObject.SetActive(false);
                }

                counter++;
            }
        }

        void SelectWeapon(WeaponIndex w)
        {
            inputController.SelectWeapon(w);
            inputController.OnWeaponSelectorUp();
        }
    }
}
