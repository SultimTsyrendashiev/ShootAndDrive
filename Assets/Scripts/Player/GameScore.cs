namespace SD.PlayerLogic
{
    struct GameScore
    {
        public float TravelledDistance;
        public int KillsAmount;
        public int DestroyedVehiclesAmount;
        public int VehicleHealth;
        public int ScorePoints;

        /// <summary>
        /// Must be calculated when player died
        /// </summary>
        public int Money { get; private set; }
        /// <summary>
        /// Must be calculated when player died.
        /// Represents all score including vehicle's health and distance
        /// </summary>
        public int ActualScorePoints { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public GameScore(int maxVehicleHealth)
        {
            TravelledDistance = KillsAmount = ScorePoints = DestroyedVehiclesAmount = 0;
            Money = ActualScorePoints = 0; 
            VehicleHealth = maxVehicleHealth;
        }

        /// <summary>
        /// Calculate money and score point
        /// </summary>
        public void Calculate()
        {
            // recalculate score points
            ActualScorePoints = ScoreCalculator.CalculateScorePoints(this);
            Money = ScoreCalculator.CalculateMoney(this);
        }
    }
}
