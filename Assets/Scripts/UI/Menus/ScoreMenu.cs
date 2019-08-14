using System.Collections;
using UnityEngine;
using SD.UI.Indicators;
using SD.PlayerLogic;

namespace SD.UI.Menus
{
    class ScoreMenu : MonoBehaviour, IMenu
    {
        /// <summary>
        /// How long to count from old to new balance
        /// </summary>
        const float BalanceCountTime = 2.5f;

        MenuController menuController;

        [SerializeField]
        SmoothCounter scoreText;
        [SerializeField]
        SmoothCounter moneyText;

        [SerializeField]
        SmoothCounter playerBalanceText;

        [SerializeField]
        Animation deadAnimation;

        [SerializeField]
        Animation scoreDisplayAnimation;

        public void Init(MenuController menuController)
        {
            this.menuController = menuController;
            GameController.OnPlayerDeath += ShowThisMenu;
            GameController.OnPlayerBalanceChange += SetBalance;
        }

        void OnDestroy()
        {
            GameController.OnPlayerDeath -= ShowThisMenu;
            GameController.OnPlayerBalanceChange -= SetBalance;
        }

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        void ShowThisMenu(GameScore score)
        {
            // enable this menu
            menuController.EnableMenu(gameObject.name);

            // set values
            scoreText.Set(score.ActualScorePoints);
            moneyText.Set(score.Money);

            StartCoroutine(WaitForAnimation());
        }

        void SetBalance(int oldBalance, int newBalance)
        {
            playerBalanceText.Set(newBalance, oldBalance, BalanceCountTime);
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

            // play another animation
            scoreDisplayAnimation.Play();

            // wait for some time
            yield return new WaitForSeconds(scoreDisplayAnimation.clip.length - 0.1f);

            // start counting
            scoreText.StartCounting();
            moneyText.StartCounting();

            // wait them
            float toWait = 0.25f + Mathf.Max(scoreText.CountTime, moneyText.CountTime);
            yield return new WaitForSeconds(toWait);

            // start counting balance
            playerBalanceText.StartCounting();
        }
    }
}
