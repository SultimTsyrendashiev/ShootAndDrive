using UnityEngine;

namespace SD.PlayerLogic
{
    static class ScoreCalculator
    {
        public static int CalculateScorePoints(GameScore score)
        {
            return 
                score.ScorePoints + 
                (int)score.TravelledDistance * 2 +
                score.VehicleHealth / 10;
        }

        public static int CalculateMoney(GameScore score)
        {
            int points = CalculateScorePoints(score);
            return (int)((points) / Random.Range(25f, 35f));
        }
    }
}
