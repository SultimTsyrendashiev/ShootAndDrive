using UnityEngine;

namespace SD.PlayerLogic
{
    /// <summary>
    /// Class for receiving pickups,
    /// must be on the same class as pickups.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    class PlayerPickupReceiver : MonoBehaviour, IPickupReceiver
    {
        // Note: pickup class already contains OnTriggerEnter

        PlayerInventory playerInventory;

        void Start()
        {
            playerInventory = GetComponentInParent<PlayerVehicle>().Player.Inventory;
            Debug.Assert(playerInventory != null, "PlayerPickupReceiver must be a child object of PlayerVehicle", this);
        }

        public bool ReceivePickup(AmmunitionType type, int amount)
        {
            // if ammo is not max
            if (playerInventory.Ammo.Get(type) < AllAmmoStats.Instance.Get(type).MaxAmount)
            {
                // then add
                playerInventory.Ammo.Add(type, amount);
                return true;
            }

            return false;
        }
    }
}
