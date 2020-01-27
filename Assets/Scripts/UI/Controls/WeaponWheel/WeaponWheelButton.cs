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
        const float SectorImageLargeMult = 1.05f;

        readonly Color DefaultImageColor = Color.white;
        readonly Color DisabledImageColor = new Color(1, 1, 1, 0.7f);

        Image weaponIcon;

        Action select;
        Action<bool> highlight;
        Action unhighlight;

        bool isInteractable;
        bool isPoiterIn;

        Image sectorImage;
        Vector2 defaultSectorImageSize;
        Vector2 largeSectorImageSize;

        RectTransform imageTransform;
        Image image;
        Vector2 defaultImageSize;
        Vector2 smallImageSize;
        Vector2 largeImageSize;

        Color defaultColor;
        Color highlitedColor;
        Color disabledColor;

        void Check()
        {
            if (imageTransform == null || image == null)
            {
                imageTransform = (RectTransform)transform.GetChild(0);
                image = imageTransform.GetComponent<Image>();
                defaultImageSize = imageTransform.sizeDelta;

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

                defaultSectorImageSize = sectorImage.rectTransform.sizeDelta;
                largeSectorImageSize = defaultSectorImageSize * SectorImageLargeMult;
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

            imageTransform.gameObject.SetActive(true);

            isInteractable = (item.Health > 0 || item.IsAmmo) && ammo.CurrentAmount > 0;

            if (isInteractable)
            {
                image.color = DefaultImageColor;
                imageTransform.sizeDelta = defaultImageSize;
                sectorImage.color = defaultColor;
            }
            else
            {
                image.color = DisabledImageColor;
                imageTransform.sizeDelta = smallImageSize;
                sectorImage.color = disabledColor;
            }

            sectorImage.rectTransform.sizeDelta = defaultSectorImageSize;
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

            imageTransform.gameObject.SetActive(false);
            sectorImage.color = disabledColor;
            sectorImage.rectTransform.sizeDelta = defaultSectorImageSize;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isInteractable)
            {
                sectorImage.color = highlitedColor;
                sectorImage.rectTransform.sizeDelta = largeSectorImageSize;
                imageTransform.sizeDelta = largeImageSize;
            }
            else
            {
                sectorImage.rectTransform.sizeDelta = defaultSectorImageSize;
                imageTransform.sizeDelta = defaultImageSize;
            }

            isPoiterIn = true;

            highlight?.Invoke(isInteractable);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isInteractable)
            {
                sectorImage.color = defaultColor;
                imageTransform.sizeDelta = defaultImageSize;
            }
            else
            {
                imageTransform.sizeDelta = smallImageSize;
            }

            sectorImage.rectTransform.sizeDelta = defaultSectorImageSize;

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
