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

        TranslatedText buyTranslation;
        TranslatedText buyAllTranslation;

        [SerializeField]
        Text currentAmountText;
        [SerializeField]
        Image currentAmountIndicator;
        [SerializeField]
        float maxImageWidth;


        /// <summary>
        /// Set info about ammo item
        /// </summary>
        /// <param name="price">price of 'amount' ammo</param>
        /// <param name="buyAmount">ammo to buy for 'price'</param>
        /// <param name="priceAll">price to buy all ammo</param>
        public void SetInfo(string name, Sprite sprite, int price, int buyAmount, int currentAmount, int maxAmount, int priceAll)
        {
            nameText.text = name;

            ammoImage.sprite = sprite;

            buyText.text = GetBuyText(buyAmount);
            buyAllText.text = GetBuyAllText();

            priceText.text = MoneyFormatter.FormatMoney(price);
            priceAllText.text = MoneyFormatter.FormatMoney(priceAll);

            currentAmountText.text = GetAmountText(currentAmount, maxAmount);
            SetAmountPercentage((float)currentAmount / maxAmount);
        }

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
                translated = "Buy x{0}";
            }

            return string.Format(translated, amount);
        }

        string GetBuyAllText()
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
    }
}
