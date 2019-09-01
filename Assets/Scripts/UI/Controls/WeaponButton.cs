using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using SD.UI.Indicators;

namespace SD.UI.Controls
{
    class WeaponButton : MonoBehaviour , IPointerEnterHandler // , IPointerExitHandler
    {
        Button button;

        [SerializeField]
        Text weaponNameText;
        [SerializeField]
        Text healthText;

        [SerializeField]
        Color greenHealthColor;
        [SerializeField]
        Color yellowHealthColor;
        [SerializeField]
        Color redHealthColor;


        // [SerializeField]
        // RectTransform healthImage;
        // [SerializeField]
        // float healthImageMaxWidth;

        Action onAmmoSelection;
        // Action onAmmoDeselection;

        public void Set(IWeaponItem item, IAmmoItem ammo, Action<WeaponIndex> onWeaponSelection, Action<AmmunitionType> onAmmoSelection) // , Action onAmmoDeselection)
        {
            gameObject.SetActive(true);

            // this.onWeaponSelection = () => onWeaponSelection(item.Index);

            this.onAmmoSelection = () => onAmmoSelection(item.AmmoType);
            // this.onAmmoDeselection = onAmmoDeselection;

            SetName(item);
            SetHealth(item);

            if (button == null)
            {
                button = GetComponentInChildren<Button>();
            }

            button.onClick.AddListener(() => onWeaponSelection(item.Index));
            button.interactable = (item.Health > 0 || item.IsAmmo) && ammo.CurrentAmount > 0;

            // Vector2 size = healthImage.sizeDelta;
            // healthImage.sizeDelta = new Vector2((float)item.Health / item.Durability * healthImageMaxWidth, size.y);
        }

        void SetName(IWeaponItem item)
        {
            string weaponName = GameController.Instance.Localization.GetValue(
                GameController.Instance.Settings.GameLanguage, item.TranslationKey);

            if (weaponNameText == null)
            {
                weaponNameText = GetComponentInChildren<Text>();
            }

            weaponNameText.text = weaponName;
        }

        void SetHealth(IWeaponItem item)
        {
            if (item.IsAmmo)
            {
                healthText.text = string.Empty;
                return;
            }

            float health = (float)item.Health / item.Durability * 100;

            healthText.text = health.ToString("F1") + "%";

            const float Yellow = 15;
            const float Red = 5;

            if (health > Yellow)
            {
                healthText.color = greenHealthColor;
            }
            else if (health > Red)
            {
                healthText.color = yellowHealthColor;
            }
            else
            {
                healthText.color = redHealthColor;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onAmmoSelection?.Invoke();
        }

        //public void OnPointerExit(PointerEventData eventData)
        //{
        //    onAmmoDeselection();
        //}
    }
}
