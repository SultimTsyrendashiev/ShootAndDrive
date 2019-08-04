using UnityEngine;

namespace SD.PlayerLogic
{
    /// <summary>
    /// Ammo pickup class. 
    /// Pickup receiver and pickups themselves must be on the same layer
    /// </summary>
    [RequireComponent(typeof(Collider))]
    class AmmoPickup : MonoBehaviour, IPickupable
    {
        [SerializeField]
        private AmmunitionType type;
        [SerializeField]
        private int amount;

        public void Pickup(IPickupReceiver receiver)
        {
            bool received = receiver.ReceiveAmmoPickup(type, amount);

            if (received)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
