using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SD.UI.Shop
{
    class WeaponEquipItem : MonoBehaviour, IPointerClickHandler
    {
        Image mainPanel;
        Image weaponIcon;
        Text weaponNameText;
        Color defaultColor;
        Color selectedColor;
        bool isSelected;

        Action select;
        Action remove;

        public void Set(IWeaponItem item, Action<WeaponIndex> select, Action<WeaponIndex> remove)
        {
            this.select = () => select(item.Index);
            this.remove = () => remove(item.Index);

            if (weaponIcon == null)
            {
                weaponIcon = transform.GetChild(0).GetComponent<Image>();
            }

            if (weaponNameText == null)
            {
                weaponNameText = transform.GetChild(1).GetComponent<Text>();
            }

            if (mainPanel == null)
            {
                mainPanel = GetComponent<Image>();
            }

            weaponIcon.sprite = item.Icon; 

            string weaponName = GameController.Instance.Localization.GetValue(
                GameController.Instance.Settings.GameLanguage, item.TranslationKey);
            weaponNameText.text = weaponName;
        }

        public void SetColors(Color defaultColor, Color selectedColor)
        {
            this.defaultColor = defaultColor;
            this.selectedColor = selectedColor;
        }

        public void SetSelection(bool isSelected)
        {
            this.isSelected = isSelected;
            mainPanel.color = isSelected ? selectedColor : defaultColor;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SetSelection(!isSelected);

            if (isSelected)
            {
                select();
            }
            else
            {
                remove();
            }
        }
    }
}
