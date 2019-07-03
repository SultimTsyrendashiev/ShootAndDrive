namespace SD
{
    interface IPickupReceiver
    {
        /// <summary>
        /// Receive pickup
        /// </summary>
        /// <returns>true, if pickup is received</returns>
        bool ReceivePickup(AmmunitionType type, int amount);
    }
}
