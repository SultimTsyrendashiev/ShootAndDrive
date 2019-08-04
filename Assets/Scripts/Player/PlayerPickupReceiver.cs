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
        Player              player;
        PlayerInventory     playerInventory;

        void Start()
        {
            player = GetComponentInParent<Player>();
            playerInventory = player.Inventory;
            Debug.Assert(playerInventory != null, "PlayerPickupReceiver must be a child object of PlayerVehicle", this);
        }

        public bool ReceiveAmmoPickup(AmmunitionType type, int amount)
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

        public bool ReceiveHealthPickup(int healthPoints)
        {
            return player.RegenerateHealth(healthPoints);
        }

        void OnTriggerEnter(Collider col)
        {
            // if layers are same
            if (col.gameObject.layer == gameObject.layer)
            {
                // try to pick up
                col.GetComponent<IPickupable>()?.Pickup(this);
            }
        }
    }
}
