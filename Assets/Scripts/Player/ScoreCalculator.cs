using UnityEngine;

namespace SD.PlayerLogic
{
    static class ScoreCalculator
    {
        public static int CalculateScorePoints(GameScore score)
        {
            return 
                score.ScorePoints + 
                (int)score.TravelledDistance * 3 +
                score.VehicleHealth;
        }

        public static int CalculateMoney(GameScore score)
        {
            int points = CalculateScorePoints(score);
            return (int)((points + Random.Range(0f, points)) / Random.Range(30f, 50f));
        }
    }
}
