using UnityEngine;
using SD.PlayerLogic;
using SD.UI.Indicators;

namespace SD.UI.Menus
{
    class ScoreMenu : MonoBehaviour
    {
        [SerializeField]
        MenuController menuController;
        [SerializeField]
        string scoreMenuName = "Score";

        [SerializeField]
        SmoothCounter scoreText;
        [SerializeField]
        SmoothCounter moneyText;

        // use this field ONLY for events
        Player player;

        void Awake()
        {
            Player.OnPlayerSpawn += Init;
        }

        void Init(Player player)
        {
            this.player = player;
            player.OnPlayerDeath += PlayerDied;
        }

        void OnDestroy()
        {
            player.OnPlayerDeath -= PlayerDied;
            Player.OnPlayerSpawn -= Init;
        }

        void PlayerDied(GameScore score)
        {
            menuController.EnableMenu(scoreMenuName);

            scoreText.Set(ScoreCalculator.CalculateScorePoints(score));
            moneyText.Set(ScoreCalculator.CalculateMoney(score));
        }
    }
}
