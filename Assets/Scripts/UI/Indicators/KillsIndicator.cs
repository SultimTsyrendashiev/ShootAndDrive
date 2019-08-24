using UnityEngine;
using UnityEngine.UI;
using SD.PlayerLogic;

namespace SD.UI.Indicators
{
    class KillsIndicator : MonoBehaviour
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
            SetKillsAmount(player.CurrentScore);

            if (!updateOnlyOnEnable)
            {
                player.OnScoreChange += SetKillsAmount;
            }
        }

        void OnDestroy()
        {
            if (!updateOnlyOnEnable)
            {
                player.OnScoreChange -= SetKillsAmount;
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
                SetKillsAmount(player.CurrentScore);
            }
        }

        void SetKillsAmount(GameScore score)
        {
            killsAmountText.text = score.KillsAmount.ToString();
        }
    }
}
