using UnityEngine;
using UnityEngine.UI;
using SD.PlayerLogic;

namespace SD.UI.Indicators
{
    class OverallDestroyedIndicator : MonoBehaviour
    {
        // use this field ONLY for events
        Player player;

        [SerializeField]
        Text killsAmountText;

        [SerializeField]
        bool updateOnlyOnEnable;

        void Awake()
        {
            Player.OnPlayerSpawn += Init;
        }

        void Init(Player player)
        {
            this.player = player;
            // default
            SetDestroyedAmount(player.CurrentScore);

            if (!updateOnlyOnEnable)
            {
                player.OnScoreChange += SetDestroyedAmount;
            }
        }

        void OnDestroy()
        {
            if (!updateOnlyOnEnable)
            {
                player.OnScoreChange -= SetDestroyedAmount;
            }

            Player.OnPlayerSpawn -= Init;
        }

        void OnEnable()
        {
            if (player == null)
            {
                player = GameController.Instance.CurrentPlayer;
            }

            if (updateOnlyOnEnable)
            {
                SetDestroyedAmount(player.CurrentScore);
            }
        }

        void SetDestroyedAmount(GameScore score)
        {
            int amount = score.KillsAmount + score.DestroyedVehiclesAmount;
            killsAmountText.text = amount.ToString();
        }
    }
}
