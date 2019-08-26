using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SD.UI.Controls
{
    class WeaponButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        Button button;
        Text weaponName;

        Action onAmmoSelection;
        Action onAmmoDeselection;

        public void Set(IWeaponItem item, IAmmoItem ammo, Action<WeaponIndex> onWeaponSelection, Action<AmmunitionType> onAmmoSelection, Action onAmmoDeselection)
        {
            // this.onWeaponSelection = () => onWeaponSelection(item.Index);
            this.onAmmoSelection = () => onAmmoSelection(item.AmmoType);
            this.onAmmoDeselection = onAmmoDeselection;

            if (weaponName == null)
            {
                weaponName = GetComponentInChildren<Text>();
            }

            weaponName.text = GameController.Instance.Languages.GetValue(
                GameController.Instance.Settings.GameLanguage, item.TranslationKey);

            if (button == null)
            {
                button = GetComponentInChildren<Button>();
            }

            button.onClick.AddListener(() => onWeaponSelection(item.Index));
            button.interactable = (item.Health > 0 || item.IsAmmo) && ammo.CurrentAmount > 0;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onAmmoSelection();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onAmmoDeselection();
        }
    }
}
