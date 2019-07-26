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

        [SerializeField]
        Image vehicleHealthImage;

        /// <summary>
        /// Current player, use this field only for events
        /// </summary>
        PlayerLogic.Player player;

        float maxVehicleHealth;

        #region init / destroy
        void Awake()
        {
            PlayerLogic.Player.OnPlayerSpawn += Init;
        }

        void Start()
        {
            SetActiveHUD(true);
            SetActivePauseMenu(false);
            SetActiveWeaponSelectionMenu(false);
        }

        void Init(PlayerLogic.Player player)
        {
            this.player = player;

            // sign to events
            Weapons.Weapon.OnAmmoChange += SetAmmoAmount;
            player.Vehicle.OnDistanceChange += SetDistance;

            SetAmmoAmount(-1);
        }

        void UnsignFromEvents()
        {
            Weapons.Weapon.OnAmmoChange -= SetAmmoAmount;

            player.Vehicle.OnDistanceChange -= SetDistance;
        }

        void OnDestroy()
        {
            UnsignFromEvents();
        }
        #endregion

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
    }
}