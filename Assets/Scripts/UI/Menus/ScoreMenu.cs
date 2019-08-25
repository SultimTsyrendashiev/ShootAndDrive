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
        const float BalanceCountTime = 1.5f;

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

        protected override void DoInit()
        {
            GameController.OnPlayerDeath += SetScore;
            GameController.Instance.Inventory.OnBalanceChange += SetBalance;
        }

        protected override void DoDestroy()
        {
            GameController.OnPlayerDeath -= SetScore;
            GameController.Instance.Inventory.OnBalanceChange -= SetBalance;
        }

        protected override void PlayActivationAnimation(string animName)
        {
            if (!firstTimeAfterDeath)
            {
                base.PlayActivationAnimation(animName);
            }
        }

        void SetBalance(int oldBalance, int newBalance)
        {
            playerBalanceText.Set(newBalance, oldBalance, MoneyFormatter.MoneyFormat, BalanceCountTime);

            if (!toCountScore && !toCountBalance)
            {
                toCountBalance = true;
            }
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
            moneyText.Set(score.Money, 0, MoneyFormatter.MoneyFormat);

            // StartCoroutine(WaitForCount());
            tempTime = Time.unscaledTime + scoreCountingDelay;
            toCountScore = true;
        }

        float tempTime;
        bool toCountScore;
        bool toCountBalance;

        void Update()
        {
            if (Time.unscaledTime > tempTime)
            {
                if (toCountScore)
                {            
                    // start counting
                    scoreText.StartCounting();
                    moneyText.StartCounting();

                    // wait for these counters
                    float toWait = balanceCountingDelay + Mathf.Max(scoreText.CountTime, moneyText.CountTime);
                    tempTime = Time.unscaledTime + toWait;

                    // deactivate this, enable next
                    toCountScore = false;
                    toCountBalance = true;
                }
                else if (toCountBalance)
                {
                    // start counting balance
                    playerBalanceText.StartCounting();

                    toCountBalance = false;
                }
            }
        }
    }
}
