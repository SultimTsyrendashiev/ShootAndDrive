using UnityEngine;
using UnityEngine.UI;

namespace SD.UI.Shop
{
    /// <summary>
    /// Displays info about weapon item in a shop.
    /// Buttons 'Buy' and 'Repair' must contain 'Text' and 'TranslatedText' components,
    /// other serialized elements mustn't, their text will be overriten automatically
    /// </summary>
    class ShopItemWeapon : MonoBehaviour
    {
        const float IndicatorMaxDamage = 50.0f;
        const float IndicatorMaxDurability = 4000.0f;
        const float IndicatorMaxFireRate = 1200.0f;
        const float IndicatorMaxAccuracy = 1.0f;

        [SerializeField]
        Text nameText;

        [SerializeField]
        RawImage weaponImage;

        [SerializeField]
        Text ammoText;
        [SerializeField]
        Image ammoImage;

        [SerializeField]
        Button buyButton;
        [SerializeField]
        Text buyText;
        TranslatedText buyTranslation;

        [SerializeField]
        Button repairButton;
        [SerializeField]
        Text repairText;
        TranslatedText repairTranslation;

        [SerializeField]
        Image damageIndicatorImage;
        [SerializeField]
        Image durabilityIndicatorImage;
        [SerializeField]
        Image fireRateIndicatorImage;
        [SerializeField]
        Image accuracyIndicatorImage;

        [SerializeField]
        GameObject health;
        [SerializeField]
        Image healthIndicatorImage;

        [SerializeField]
        float maxIndicatorsWidth;

        IShop shop;
        IWeaponItem weaponItem;

        public void SetInfo(IShop shop, IWeaponItem weaponItem, IAmmoItem ammoItem, Texture image)
        {
            this.shop = shop;
            this.weaponItem = weaponItem;

            // firstly, set const info
            nameText.text = GetTranslation(weaponItem.TranslationKey);

            ammoImage.sprite = ammoItem.Icon;
            ammoText.text = GetTranslation(ammoItem.TranslationKey);

            SetPercentage(damageIndicatorImage, maxIndicatorsWidth, 
                Mathf.Clamp(weaponItem.Damage / IndicatorMaxDamage, 0, 1));

            SetPercentage(durabilityIndicatorImage, maxIndicatorsWidth, 
                Mathf.Clamp(weaponItem.Durability / IndicatorMaxDurability, 0, 1));

            SetPercentage(fireRateIndicatorImage, maxIndicatorsWidth,
                Mathf.Clamp(weaponItem.RoundsPerMinute / IndicatorMaxFireRate, 0, 1));

            SetPercentage(accuracyIndicatorImage, maxIndicatorsWidth,
                Mathf.Clamp(weaponItem.Accuracy / IndicatorMaxAccuracy, 0, 1));

            // then updateable info
            UpdateInfo();

            SetImage(image);
        }

        /// <summary>
        /// Update info that can change
        /// </summary>
        void UpdateInfo()
        {
            if (!weaponItem.IsBought && weaponItem.Health > 0)
            {
                buyButton.gameObject.SetActive(true);
                repairButton.gameObject.SetActive(false);
                health.SetActive(false);

                buyText.text = GetBuyText(weaponItem.Price);
            }
            else
            {
                buyButton.gameObject.SetActive(false);
                repairButton.gameObject.SetActive(true);
                health.SetActive(true);

                float healthPercentage = Mathf.Clamp((float)weaponItem.Health / weaponItem.Durability, 0, 1);

                SetPercentage(healthIndicatorImage, maxIndicatorsWidth, healthPercentage);

                int repairCost = shop.GetRepairCost(weaponItem);
                repairText.text = GetRepairText(repairCost);

                // disable button, if full health
                repairButton.interactable = healthPercentage != 1;
            }
        }

        void SetImage(Texture image)
        {
            weaponImage.texture = image;
        }

        public void BuyThis()
        {
            if (shop == null)
            {
                return;
            }

            shop.BuyWeapon(weaponItem.Index);

            UpdateInfo();
        }

        public void RepairThis()
        {
            if (shop == null)
            {
                return;
            }

            shop.RepairWeapon(weaponItem.Index);

            UpdateInfo();
        }

        #region hud
        string GetTranslation(string key)
        {
            return GameController.Instance.Languages.GetValue(
                GameController.Instance.Settings.GameLanguage, key);
        }

        /// <summary>
        /// Set width of the image
        /// </summary>
        /// <param name="percentage">amount percentage in [0..1]</param>
        void SetPercentage(Image image, float maxWidth, float percentage)
        {
            Vector2 d = image.rectTransform.sizeDelta;
            d.x = percentage * maxWidth;

            image.rectTransform.sizeDelta = d;
        }

        string GetBuyText(int price)
        {
            if (buyTranslation == null)
            {
                buyTranslation = buyText.GetComponent<TranslatedText>();
            }

            string translated;

            try
            {
                translated = buyTranslation.GetValue();
            }
            catch
            {
                translated = "Buy {0}";
            }

            return string.Format(translated, MoneyFormatter.FormatMoney(price));
        }

        string GetRepairText(int price)
        {
            if (repairTranslation == null)
            {
                repairTranslation = repairText.GetComponent<TranslatedText>();
            }

            string translated;

            try
            {
                translated = repairTranslation.GetValue();
            }
            catch
            {
                translated = "Repair {0}";
            }

            return string.Format(translated, MoneyFormatter.FormatMoney(price));
        }
        #endregion
    }
}
