using System.Collections;
using UnityEngine;
using SD.UI.Indicators;
using SD.PlayerLogic;

namespace SD.UI.Menus
{
    class ScoreMenu : AAnimatedMenu
    {
        /// <summary>
        /// How long to count from old to new balance
        /// </summary>
        const float BalanceCountTime = 2.5f;

        [SerializeField]
        string deathScreenAnimation;

        [SerializeField]
        SmoothCounter scoreText;
        [SerializeField]
        SmoothCounter moneyText;

        [SerializeField]
        SmoothCounter playerBalanceText;

        /// <summary>
        /// How long to wait from enabling this menu to start of counting score and money
        /// </summary>
        [SerializeField]
        float scoreCountingDelay = 1.0f;

        /// <summary>
        /// How long to wait from end of counting score and money
        /// to start of counting player's balance
        /// </summary>
        [SerializeField]
        float balanceCountingDelay = 0.25f;

        /// <summary>
        /// If true, then don't play activation animation
        /// </summary>
        bool firstTimeAfterDeath;

        protected override void SignToEvents()
        {
            GameController.OnPlayerDeath += SetScore;
            GameController.Instance.Inventory.OnBalanceChange += SetBalance;
        }

        protected override void UnsignFromEvents()
        {
            GameController.OnPlayerDeath -= SetScore;
            GameController.Instance.Inventory.OnBalanceChange -= SetBalance;
        }

        /// <summary>
        /// Activates this menu and set score for counters
        /// </summary>
        void SetScore(GameScore score)
        {
            firstTimeAfterDeath = true;

            // activate this menu
            ShowThisMenu();
            firstTimeAfterDeath = false;

            Animator.Play(deathScreenAnimation);

            // set values
            scoreText.Set(score.ActualScorePoints);
            moneyText.Set(score.Money);

            StartCoroutine(WaitForCount());
        }

        void SetBalance(int oldBalance, int newBalance)
        {
            playerBalanceText.Set(newBalance, oldBalance, BalanceCountTime);
        }

        protected override void PlayActivationAnimation(string animName)
        {
            if (!firstTimeAfterDeath)
            {
                base.PlayActivationAnimation(animName);
            }
        }

        /// <summary>
        /// Waits for UI death animation
        /// </summary>
        /// <returns></returns>
        IEnumerator WaitForCount()
        {
            yield return new WaitForSecondsRealtime(scoreCountingDelay);

            // start counting
            scoreText.StartCounting();
            moneyText.StartCounting();

            // wait for these counters
            float toWait = balanceCountingDelay + Mathf.Max(scoreText.CountTime, moneyText.CountTime);
            yield return new WaitForSecondsRealtime(toWait);

            // start counting balance
            playerBalanceText.StartCounting();
        }
    }
}
