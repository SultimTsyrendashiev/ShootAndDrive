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
        const float IndicatorMaxFireRate = 1000.0f;
        const float IndicatorMaxAccuracy = 1.0f;

        [SerializeField]
        Text nameText;

        [SerializeField]
        RawImage weaponImage;

        [SerializeField]
        Text ammoText;
        [SerializeField]
        Image ammoImage;

        #region buttons
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
        #endregion

        #region indicators
        [SerializeField]
        Image damageIndicatorImage;
        [SerializeField]
        Image durabilityIndicatorImage;
        [SerializeField]
        Image fireRateIndicatorImage;
        [SerializeField]
        Image accuracyIndicatorImage;
        #endregion

        #region health
        [SerializeField]
        GameObject health;
        [SerializeField]
        Image healthIndicatorImage;
        #endregion

        /// <summary>
        /// Max width of indicator image
        /// </summary>
        [SerializeField]
        float maxIndicatorsWidth;

        #region stats that can be disabled
        [SerializeField]
        GameObject durabilityIndicator;

        [SerializeField]
        GameObject accuracyIndicator;
        #endregion

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
            if (weaponItem == null)
            {
                return;
            }

            if (!weaponItem.IsBought)
            {
                // if not bought, show buy button 
                buyButton.gameObject.SetActive(ShowBuyButton(weaponItem));
                buyText.text = GetBuyText(weaponItem.Price);

                // don't show repair button and variable stats
                repairButton.gameObject.SetActive(false);
                health.SetActive(false);
            }
            else
            {
                bool canBeRepaired = CanBeRepaired(weaponItem);

                // show, if necessary
                repairButton.gameObject.SetActive(canBeRepaired);
                health.SetActive(canBeRepaired);

                if (canBeRepaired)
                {
                    float healthPercentage = Mathf.Clamp((float)weaponItem.Health / weaponItem.Durability, 0, 1);

                    SetPercentage(healthIndicatorImage, maxIndicatorsWidth, healthPercentage);
                
                    // disable button, if full health
                    repairButton.interactable = healthPercentage != 1;

                    int repairCost = shop.GetRepairCost(weaponItem);
                    repairText.text = GetRepairText(repairCost);
                }

                // disable buy button
                buyButton.gameObject.SetActive(false);
            }

            // disable indicators that are not necessary
            durabilityIndicator.SetActive(ShowDurabilityIndicator(weaponItem));
            accuracyIndicator.SetActive(ShowAccuracyIndicator(weaponItem));
        }

        static bool CanBeRepaired(IWeaponItem weaponItem)
        {
            return weaponItem.CanBeDamaged && !weaponItem.IsAmmo;
        }

        static bool ShowBuyButton(IWeaponItem weaponItem)
        {
            return !weaponItem.IsAmmo;
        }

        static bool ShowDurabilityIndicator(IWeaponItem weaponItem)
        {
            return weaponItem.CanBeDamaged;
        }

        static bool ShowAccuracyIndicator(IWeaponItem weaponItem)
        {
           return !(weaponItem.AmmoType == AmmunitionType.FireBottles ||
                weaponItem.AmmoType == AmmunitionType.Grenades ||
                weaponItem.AmmoType == AmmunitionType.Cannonballs ||
                weaponItem.AmmoType == AmmunitionType.Rockets);
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
