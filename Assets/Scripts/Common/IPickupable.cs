using UnityEngine;

namespace SD
{
    interface IPickupable
    {
        /// <summary>
        /// Called when this object is picked up
        /// </summary>
        void Pickup(IPickupReceiver receiver);
    }
}
