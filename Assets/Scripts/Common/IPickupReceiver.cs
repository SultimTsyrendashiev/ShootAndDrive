namespace SD
{
    interface IPickupReceiver
    {
        /// <summary>
        /// Receive pickup
        /// </summary>
        /// <returns>true, if pickup is received</returns>
        bool ReceiveAmmoPickup(AmmunitionType type, int amount);

        /// <summary>
        /// Receive pickup
        /// </summary>
        /// <returns>true, if pickup is received</returns>
        bool ReceiveHealthPickup(int healthPoints);
    }
}
