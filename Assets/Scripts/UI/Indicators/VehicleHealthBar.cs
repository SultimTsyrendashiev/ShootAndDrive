using UnityEngine;
using UnityEngine.UI;
using SD.PlayerLogic;

namespace SD.UI.Indicators
{
    class VehicleHealthBar : MonoBehaviour
    {
        // use this field ONLY for events
        PlayerVehicle vehicle;

        // indicator image itself
        [SerializeField]
        Image vehicleHealthImage;

        [SerializeField]
        float maxHealthImageWidth = 160;

        float maxVehicleHealth;

        void Awake()
        {
            Player.OnPlayerSpawn += Init;
        }

        void Init(Player player)
        {
            vehicle = player.Vehicle;
            maxVehicleHealth = vehicle.MaxHealth;

            vehicle.OnVehicleHealthChange += SetVehicleHealth;
        }

        void OnDestroy()
        {
            if (vehicle != null)
            {
                vehicle.OnVehicleHealthChange -= SetVehicleHealth;
            }

            Player.OnPlayerSpawn -= Init;
        }

        /// <summary>
        /// Set health in HUD
        /// </summary>
        /// <param name="health">health in [0..100]</param>
        public void SetVehicleHealth(float health)
        {
            Vector2 d = vehicleHealthImage.rectTransform.sizeDelta;
            d.x = health / maxVehicleHealth * 160;

            vehicleHealthImage.rectTransform.sizeDelta = d;
        }
    }
}
