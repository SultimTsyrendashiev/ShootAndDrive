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
        IWeaponItem item;

        Action select;
        Action remove;

        public bool IsSelected => item.IsSelected;

        public void Init(IWeaponItem item, 
            Action<WeaponIndex> select, Action<WeaponIndex> remove,
            Color defaultColor, Color selectedColor)
        {
            this.item = item;
            this.select = () => select(item.Index);
            this.remove = () => remove(item.Index);
            this.defaultColor = defaultColor;
            this.selectedColor = selectedColor;

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

            UpdateColor();
        }

        void OnEnable()
        {
            UpdateColor();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            bool toSelect = !IsSelected;

            if (toSelect)
            {
                select();
            }
            else
            {
                remove();
            }

            UpdateColor();
        }

        public void UpdateColor()
        {
            if (mainPanel != null)
            {
                mainPanel.color = IsSelected ? selectedColor : defaultColor;
            }
        }
    }
}
