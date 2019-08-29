using UnityEngine;
using UnityEngine.UI;

namespace SD.UI.Shop
{
    /// <summary>
    /// Displays info about ammo item in a shop.
    /// Buttons 'Buy' and 'Buy All' must contain 'Text' and 'TranslatedText' components,
    /// other serialized elements mustn't, their text will be overriten automatically
    /// </summary>
    class ShopItemAmmo : MonoBehaviour
    {
        [SerializeField]
        Text nameText;

        [SerializeField]
        Image ammoImage;

        [SerializeField]
        Text priceText;
        [SerializeField]
        Text priceAllText;

        /// <summary>
        /// Must contain 'TranslatedText' component
        /// </summary>
        [SerializeField]
        Text buyText;
        /// <summary>
        /// Must contain 'TranslatedText' component
        /// </summary>
        [SerializeField]
        Text buyAllText;

        [SerializeField]
        Button buyButton;
        [SerializeField]
        Button buyAllButton;

        TranslatedText buyTranslation;
        TranslatedText buyAllTranslation;

        [SerializeField]
        Text currentAmountText;
        [SerializeField]
        Image currentAmountIndicator;
        [SerializeField]
        float maxImageWidth;

        IShop shop;
        IAmmoItem ammoItem;

        /// <summary>
        /// Set info about ammo item
        /// </summary>
        public void SetInfo(IShop shop, IAmmoItem item)
        {
            this.shop = shop;
            this.ammoItem = item;

            nameText.text = GetTranslation(ammoItem.TranslationKey);
            ammoImage.sprite = ammoItem.Icon;

            UpdateInfo();
        }

        void UpdateInfo()
        {
            int diff = ammoItem.MaxAmount - ammoItem.CurrentAmount;
            if (diff > ammoItem.AmountToBuy)
            {
                diff = ammoItem.AmountToBuy;
            }

            int diffAll = ammoItem.MaxAmount - ammoItem.CurrentAmount;


            buyText.text = GetBuyText(diff);
            buyAllText.text = GetBuyAllText(diffAll);

            priceText.text =
                MoneyFormatter.FormatMoney(shop.GetAmmoPrice(ammoItem, diff));
            priceAllText.text =
                MoneyFormatter.FormatMoney(shop.GetAmmoPrice(ammoItem, diffAll));

            currentAmountText.text = GetAmountText(ammoItem.CurrentAmount, ammoItem.MaxAmount);
            SetAmountPercentage((float)ammoItem.CurrentAmount / ammoItem.MaxAmount);

            buyButton.interactable = diff != 0 && shop.EnoughMoneyToBuy(ammoItem, diff);
            buyAllButton.interactable = diffAll != 0 && shop.EnoughMoneyToBuy(ammoItem, diffAll);
        }


        public void BuyThis()
        {
            if (shop == null)
            {
                return;
            }

            shop.BuyAmmo(ammoItem.This, false);

            // update this
            UpdateInfo();
        }

        public void BuyThisAll()
        {
            if (shop == null)
            {
                return;
            }

            shop.BuyAmmo(ammoItem.This, true);

            // update this
            UpdateInfo();
        }

        #region hud
        /// <summary>
        /// Set width of current amount image
        /// </summary>
        /// <param name="amountPercentage">amount percentage in [0..1]</param>
        void SetAmountPercentage(float amountPercentage)
        {
            Vector2 d = currentAmountIndicator.rectTransform.sizeDelta;
            d.x = amountPercentage * maxImageWidth;

            currentAmountIndicator.rectTransform.sizeDelta = d;
        }

        /// <summary>
        /// Get translated text for buying button
        /// </summary>
        /// <param name="amount">how many </param>
        /// <returns>        
        /// Example result: "Buy x14"
        /// </returns>
        string GetBuyText(int amount)
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
                return string.Format("Buy x{0}", amount);
            }

            return string.Format(translated, amount);
        }

        string GetBuyAllText(int amount)
        {
            if (buyAllTranslation == null)
            {
                buyAllTranslation = buyAllText.GetComponent<TranslatedText>();
            }

            try
            {
                return buyTranslation.GetValue();
            }
            catch
            {
                return "Buy All";
            }
        }

        string GetAmountText(int currentAmount, int maxAmount)
        {
            const string format = "{0} / {1}";
            return string.Format(format, currentAmount, maxAmount);
        }

        string GetTranslation(string key)
        {
            return GameController.Instance.Localization.GetValue(
                GameController.Instance.Settings.GameLanguage, key);
        }
        #endregion
    }
}
