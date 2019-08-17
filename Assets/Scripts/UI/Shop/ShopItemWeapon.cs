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
        Text ammoText;
        [SerializeField]
        Image ammoImage;

        [SerializeField]
        GameObject buyButton;
        [SerializeField]
        Text buyText;
        TranslatedText buyTranslation;

        [SerializeField]
        GameObject repairButton;
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
        
        public void Set(string name, IWeaponItem weaponItem, IAmmoItem ammoItem)
        {
            nameText.text = GetTranslation(weaponItem.TranslationKey);

            ammoImage.sprite = ammoItem.Icon;
            ammoText.text = GetTranslation(ammoItem.TranslationKey);

            if (!weaponItem.IsBought && weaponItem.Health > 0)
            {
                buyButton.SetActive(true);
                repairButton.SetActive(false);
                health.SetActive(false);

                buyText.text = GetBuyText(weaponItem.Cost);
            }
            else
            {
                buyButton.SetActive(false);
                repairButton.SetActive(true);
                health.SetActive(true);

                float healthPercentage = Mathf.Clamp((float)weaponItem.Health / weaponItem.Durability, 0, 1);

                SetPercentage(healthIndicatorImage, maxIndicatorsWidth, healthPercentage);

                // todo: add full repair cost
                int fullRepairCost = weaponItem.Cost / 4;

                int repairCost = (int)(fullRepairCost * healthPercentage);
                repairText.text = GetRepairText(repairCost);
            }

            SetPercentage(damageIndicatorImage, maxIndicatorsWidth, 
                Mathf.Clamp(weaponItem.Damage / IndicatorMaxDamage, 0, 1));

            SetPercentage(durabilityIndicatorImage, maxIndicatorsWidth, 
                Mathf.Clamp(weaponItem.Durability / IndicatorMaxDurability, 0, 1));

            SetPercentage(fireRateIndicatorImage, maxIndicatorsWidth,
                Mathf.Clamp(weaponItem.RoundsPerMinute / IndicatorMaxFireRate, 0, 1));

            SetPercentage(accuracyIndicatorImage, maxIndicatorsWidth,
                Mathf.Clamp(weaponItem.Accuracy / IndicatorMaxAccuracy, 0, 1));
        }

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
                translated = "Repair {0}";
            }

            return string.Format(translated, MoneyFormatter.FormatMoney(price));
        }
    }
}
