using UnityEngine;
using UnityEngine.UI;
using SD.PlayerLogic;

namespace SD.UI.Indicators
{
    class MoneyIndicator : MonoBehaviour
    {
        Player player;

        [SerializeField]
        Text moneyAmountText;

        void Awake()
        {
            SetMoneyAmount(0);
            Player.OnPlayerSpawn += Init;
        }

        void Init(Player player)
        {
            this.player = player;
            Player.OnPlayerSpawn -= Init;
        }

        void OnEnable()
        {
            if (player != null)
            {
                SetMoneyAmount(player.Inventory.Money);
            }
        }

        void SetMoneyAmount(int amount)
        {
            moneyAmountText.text = PlayerInventory.MoneySymbol + amount.ToString();
        }
    }
}
