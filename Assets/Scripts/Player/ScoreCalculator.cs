using UnityEngine;

namespace SD.PlayerLogic
{
    static class ScoreCalculator
    {
        public static int CalculateScorePoints(GameScore score)
        {
            int scorePoints = score.ScorePoints +
                (int)score.TravelledDistance * 2 +
                score.VehicleHealth / 10;

            return scorePoints;
        }

        public static int CalculateMoney(GameScore score)
        {
            int points = CalculateScorePoints(score);
            return (int)(points / Random.Range(14f, 17f));
        }
    }
}
