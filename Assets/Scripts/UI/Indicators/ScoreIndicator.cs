using UnityEngine;
using UnityEngine.UI;
using SD.PlayerLogic;

namespace SD.UI.Indicators
{
    class ScoreIndicator : MonoBehaviour
    {
        // use this field ONLY for events
        Player player;

        [SerializeField]
        Text scorePointsText;

        void Awake()
        {
            Player.OnPlayerSpawn += Init;
        }

        void Init(Player player)
        {
            this.player = player;
            player.OnScoreChange += SetScorePoints;
        }

        void OnDestroy()
        {
            Player.OnPlayerSpawn -= Init;
            player.OnScoreChange -= SetScorePoints;
        }

        void SetScorePoints(GameScore score)
        {
            scorePointsText.text = score.ScorePoints.ToString();
        }
    }
}
