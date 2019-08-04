using UnityEngine;

namespace SD.PlayerLogic
{
    /// <summary>
    /// Health pickup class. 
    /// Pickup receiver and pickups themselves must be on the same layer
    /// </summary>
    [RequireComponent(typeof(Collider))]
    class HealthPickup : MonoBehaviour, IPickupable
    {
        [SerializeField]
        int healthPoints;

        public void Pickup(IPickupReceiver receiver)
        {
            bool received = receiver.ReceiveHealthPickup(healthPoints);

            if (received)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
