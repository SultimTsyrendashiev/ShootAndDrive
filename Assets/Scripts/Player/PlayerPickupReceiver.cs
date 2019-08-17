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
        IInventory          playerInventory;

        void Start()
        {
            player = GetComponentInParent<Player>();
            Debug.Assert(player != null, "PlayerPickupReceiver must be a child object of Player", this);

            playerInventory = player.Inventory;
            Debug.Assert(playerInventory != null, "Player doesn't have inventory", player);
        }

        public bool ReceiveAmmoPickup(AmmunitionType type, int amount)
        {
            IAmmoItem ammo = playerInventory.Ammo.Get(type);

            // if ammo is not max
            if (ammo.CurrentAmount < ammo.MaxAmount)
            {
                // then add
                ammo.CurrentAmount += amount;
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
