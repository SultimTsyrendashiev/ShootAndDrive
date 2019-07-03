using System.Collections.Generic;
using UnityEngine;
using SD.Weapons;

namespace SD.PlayerLogic
{
    /// <summary>
    /// Pickup class. Pickup receiver and pickups themselves 
    /// must be on the same layer
    /// </summary>
    [RequireComponent(typeof(Collider))]
    class Pickup : MonoBehaviour
    {
        [SerializeField]
        private AmmunitionType type;
        [SerializeField]
        private int amount;

        private int pickupLayer;

        void Start()
        {
            pickupLayer = LayerMask.NameToLayer(LayerNames.Pickups);
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.layer == pickupLayer)
            {
                IPickupReceiver receiver = col.GetComponent<IPickupReceiver>();

                if (receiver != null)
                { 
                    receiver.ReceivePickup(type, amount);
                }
            }
        }
    }
}
