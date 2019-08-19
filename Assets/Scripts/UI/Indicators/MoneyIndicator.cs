using UnityEngine;
using UnityEngine.UI;
using SD.PlayerLogic;

namespace SD.UI.Indicators
{
    class MoneyIndicator : MonoBehaviour
    {
        [SerializeField]
        Text moneyAmountText;

        IInventory inventory;

        void Start()
        {
            Init(GameController.Instance.Inventory);
        }

        void OnDestroy()
        {
            if (inventory != null)
            {
                inventory.OnBalanceChange -= SetMoneyAmount;
            }
        }

        void Init(IInventory inventory)
        {
            this.inventory = inventory;
            SetMoneyAmount(inventory.Money);

            inventory.OnBalanceChange += SetMoneyAmount;
        }

        void SetMoneyAmount(int oldBalance, int newBalance)
        {
            SetMoneyAmount(newBalance);
        }

        void SetMoneyAmount(int amount)
        {
            moneyAmountText.text = MoneyFormatter.FormatMoney(amount);
        }
    }
}
