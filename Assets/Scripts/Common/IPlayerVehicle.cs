namespace SD
{
    interface IPlayerVehicle : IVehicle, IDamageable
    {
        /// <summary>
        /// Increase speed of this vehicle to max
        /// </summary>
        void Accelerate();
        /// <summary>
        /// Stop the vehicle
        /// </summary>
        void Brake();
    }
}
