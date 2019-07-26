using UnityEngine;
using UnityEngine.UI;
using SD.PlayerLogic;

namespace SD.UI.Indicators
{
    class VehiclesIndicator : MonoBehaviour
    {
        // use this field ONLY for events
        Player player;

        [SerializeField]
        Text vehiclesAmountText;

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
            SetVehiclesAmount(player.CurrentScore);

            if (!updateOnlyOnEnable)
            {
                player.OnScoreChange += SetVehiclesAmount;
            }
        }

        void OnDestroy()
        {
            if (!updateOnlyOnEnable)
            {
                player.OnScoreChange -= SetVehiclesAmount;
            }

            Player.OnPlayerSpawn -= Init;
        }

        void OnEnable()
        {
            if (updateOnlyOnEnable && player)
            {
                SetVehiclesAmount(player.CurrentScore);
            }
        }

        void SetVehiclesAmount(GameScore score)
        {
            vehiclesAmountText.text = score.DestroyedVehiclesAmount.ToString();
        }
    }
}
