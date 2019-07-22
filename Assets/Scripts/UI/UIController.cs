using UnityEngine;
using UnityEngine.UI;

namespace SD.UI
{
    class UIController : MonoBehaviour
    {
        [SerializeField]
        GameObject hud;
        [SerializeField]
        GameObject interactive;
        [SerializeField]
        GameObject weaponSelection;
        [SerializeField]
        GameObject pauseMenu;

        MovementInputType movementInputType;

        [SerializeField]
        GameObject movementField;
        [SerializeField]
        GameObject movementButtons;

        [SerializeField]
        Text currentAmmoText;
        [SerializeField]
        Text distanceText;
        [SerializeField]
        Text killsAmountText;

        const float MaxHealthImageWidth = 160;
        [SerializeField]
        Image healthImage;
        [SerializeField]
        Image vehicleHealthImage;

        /// <summary>
        /// Player to follow
        /// </summary>
        PlayerLogic.Player player;

        public void Start()
        {
            SetActiveHUD(true);
            SetActivePauseMenu(false);
            SetActiveWeaponSelectionMenu(false);
        }

        public void Init(PlayerLogic.Player player)
        {
            this.player = player;

            // sign to events
            player.OnHealthChange += SetHealth;
            player.OnScoreChange += SetScore;
            player.Vehicle.OnDistanceChange += SetDistance;
            player.Vehicle.OnVehicleHealthChange += SetVehicleHealth;
            Weapons.Weapon.OnAmmoChange += SetAmmoAmount;

            // set start stats
            SetHealth(player.Health);
            SetVehicleHealth(player.Vehicle.Health);
            SetAmmoAmount(-1);
        }

        void UnsignFromEvents()
        {
            Weapons.Weapon.OnAmmoChange -= SetAmmoAmount;

            player.OnHealthChange -= SetHealth;
            player.OnScoreChange -= SetScore;
            player.Vehicle.OnDistanceChange -= SetDistance;
            player.Vehicle.OnVehicleHealthChange -= SetVehicleHealth;
        }

        void OnDestroy()
        {
            UnsignFromEvents();
        }

        public MovementInputType MovementInputType
        {
            get
            {
                return MovementInputType;
            }
            set
            {
                if (movementInputType == value)
                {
                    return;
                }

                switch (value)
                {
                    case MovementInputType.Joystick:
                        movementField.SetActive(true);
                        movementButtons.SetActive(false);
                        break;
                    case MovementInputType.Buttons:
                        movementField.SetActive(false);
                        movementButtons.SetActive(true);
                        break;
                    case MovementInputType.Gyroscope:
                        movementField.SetActive(false);
                        movementButtons.SetActive(false);
                        break;
                }
            }
        }

        public void SetActiveWeaponSelectionMenu(bool active)
        {
            weaponSelection.SetActive(active);
        }

        public void SetActiveHUD(bool active)
        {
            hud.SetActive(active);
            interactive.SetActive(active);
        }

        public void SetActivePauseMenu(bool active)
        {
            pauseMenu.SetActive(active);
        }

        public void SetAmmoAmount(int amount)
        {
            currentAmmoText.text = amount >= 0 ? amount.ToString() : "";
        }

        public void SetDistance(float meters)
        {
            distanceText.text = meters.ToString("N1");
        }

        // TODO: delete
        public void SetKillsAmount(int amount)
        {
            killsAmountText.text = amount.ToString();
        }

        void SetScore(PlayerLogic.GameScore score)
        {
            killsAmountText.text = score.KillsAmount.ToString();
        }

        /// <summary>
        /// Set health in HUD
        /// </summary>
        /// <param name="health">health in [0..100]</param>
        public void SetHealth(float health)
        {
            const float maxPlayerHealth = PlayerLogic.Player.MaxHealth;

            Vector2 d = healthImage.rectTransform.sizeDelta;
            d.x = health / maxPlayerHealth * MaxHealthImageWidth;

            healthImage.rectTransform.sizeDelta = d;
        }

        /// <summary>
        /// Set health in HUD
        /// </summary>
        /// <param name="health">health in [0..100]</param>
        public void SetVehicleHealth(float health)
        {
            const float maxVehicleHealth = PlayerLogic.PlayerVehicle.MaxHealth;

            Vector2 d = vehicleHealthImage.rectTransform.sizeDelta;
            d.x = health / maxVehicleHealth * MaxHealthImageWidth;

            vehicleHealthImage.rectTransform.sizeDelta = d;
        }
    }
}