namespace SD.PlayerLogic
{
    struct GameScore
    {
        public float TravelledDistance;
        public int KillsAmount;
        public int ScorePoints;
        public int DestroyedVehiclesAmount;
        public int VehicleHealth;

        /// <summary>
        /// Default constructor
        /// </summary>
        public GameScore(int maxVehicleHealth)
        {
            TravelledDistance = KillsAmount = ScorePoints  = DestroyedVehiclesAmount = 0;
            VehicleHealth = maxVehicleHealth;
        }
    }
}
