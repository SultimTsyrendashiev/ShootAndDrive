using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SD.UI.Controls
{
    class WeaponWheelButton : MonoBehaviour, IPointerEnterHandler, IPointerUpHandler, IPointerExitHandler, IPointerDownHandler
    {
        const string WeaponIconName = "Image";
        const float ImageSmallMult = 0.7f;
        const float ImageLargeMult = 1.3f;

        Image weaponIcon;

        Action select;
        Action<bool> highlight;
        Action unhighlight;

        bool isInteractable;
        bool isPoiterIn;

        Image sectorImage;

        RectTransform image;
        Vector2 defaultImageSize;
        Vector2 smallImageSize;
        Vector2 largeImageSize;

        Color defaultColor;
        Color highlitedColor;
        Color disabledColor;

        void Check()
        {
            if (image == null)
            {
                image = (RectTransform)transform.GetChild(0);
                defaultImageSize = image.sizeDelta;

                smallImageSize = defaultImageSize * ImageSmallMult;
                largeImageSize = defaultImageSize * ImageLargeMult;
            }

            if (weaponIcon == null)
            {
                var images = transform.GetComponentsInChildren<Image>(true);
                foreach (Image i in images)
                {
                    if (i.name == WeaponIconName)
                    {
                        weaponIcon = i;
                        break;
                    }
                }
            }

            if (sectorImage == null)
            {
                sectorImage = GetComponent<Image>();
            }
        }

        public void Set(IWeaponItem item, IAmmoItem ammo, 
            Action<WeaponIndex> select, Action<WeaponIndex, bool> highlight, Action unhighlight)
        {
            Check();

            this.select = () => select(item.Index);
            this.highlight = (canBeSelected) => highlight(item.Index, canBeSelected);
            this.unhighlight = unhighlight;

            weaponIcon.rectTransform.eulerAngles = new Vector3(0, 0, 0);
            weaponIcon.sprite = item.Icon;

            image.gameObject.SetActive(true);

            isInteractable = (item.Health > 0 || item.IsAmmo) && ammo.CurrentAmount > 0;

            if (isInteractable)
            {
                image.sizeDelta = defaultImageSize;
                sectorImage.color = defaultColor;
            }
            else
            {
                image.sizeDelta = smallImageSize;
                sectorImage.color = disabledColor;
            }
        }

        public void SetColors(Color defaultColor, Color highlitedColor, Color disabledColor)
        {
            this.defaultColor = defaultColor;
            this.highlitedColor = highlitedColor;
            this.disabledColor = disabledColor;
        }

        public void Disable()
        {
            Check();

            select = null;
            highlight = null;
            unhighlight = null;

            image.gameObject.SetActive(false);
            sectorImage.color = disabledColor;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isInteractable)
            {
                sectorImage.color = highlitedColor;
                image.sizeDelta = largeImageSize;
            }
            else
            {
                image.sizeDelta = largeImageSize;
            }

            isPoiterIn = true;

            highlight?.Invoke(isInteractable);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isInteractable)
            {
                sectorImage.color = defaultColor;
                image.sizeDelta = defaultImageSize;
            }
            else
            {
                image.sizeDelta = smallImageSize;
            }

            isPoiterIn = false;

            unhighlight?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isInteractable || !isPoiterIn)
            {
                return;
            }

            select?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // to make OnPointerUp work
        }
    }
}
