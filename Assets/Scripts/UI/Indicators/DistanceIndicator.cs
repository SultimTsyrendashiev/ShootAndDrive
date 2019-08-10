using UnityEngine;
using UnityEngine.UI;
using SD.PlayerLogic;

namespace SD.UI.Indicators
{
    class DistanceIndicator : MonoBehaviour
    {
        // use this field ONLY for events
        PlayerVehicle vehicle;

        [SerializeField]
        Text distanceText;

        [SerializeField]
        bool updateOnlyOnEnable;

        void Awake()
        {
            Player.OnPlayerSpawn += Init;
        }

        void Init(Player player)
        {
            vehicle = player.Vehicle;

            // default
            SetDistance(0);

            if (!updateOnlyOnEnable)
            {
                vehicle.OnDistanceChange += SetDistance;
            }
        }

        void OnDestroy()
        {
            if (!updateOnlyOnEnable)
            {
                vehicle.OnDistanceChange -= SetDistance;
            }

            Player.OnPlayerSpawn -= Init;
        }

        void OnEnable()
        {
            if (updateOnlyOnEnable && vehicle)
            {
                SetDistance(vehicle.TravelledDistance);
            }
        }

        void SetDistance(float meters)
        {
            int a = (int)meters;
            //int b = (int)(meters * 10) % 10;
            distanceText.text = a.ToString(); // + "." + b;
        }
    }
}
