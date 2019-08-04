using System.Collections;
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

        [SerializeField]
        Animation deadAnimation;

        // use this field ONLY for events
        Player player;

        void Awake()
        {
            Player.OnPlayerSpawn += Init;
        }

        void Init(Player player)
        {
            this.player = player;
            player.OnPlayerDeath += ShowScoreMenu;
        }

        void OnDestroy()
        {
            player.OnPlayerDeath -= ShowScoreMenu;
            Player.OnPlayerSpawn -= Init;
        }

        void ShowScoreMenu(GameScore score)
        {
            // enable this menu
            menuController.EnableMenu(scoreMenuName);

            // set values
            scoreText.Set(score.ActualScorePoints);
            moneyText.Set(score.Money);

            StartCoroutine(WaitForAnimation());
        }

        /// <summary>
        /// Waits for UI death animation
        /// </summary>
        /// <returns></returns>
        IEnumerator WaitForAnimation()
        {
            // show ui dead animation
            deadAnimation.Play();

            yield return new WaitForSeconds(deadAnimation.clip.length);

            // start counting
            scoreText.StartCounting();
            moneyText.StartCounting();
        }
    }
}
