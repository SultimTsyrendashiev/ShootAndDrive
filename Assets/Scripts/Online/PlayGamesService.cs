using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

namespace SD.Online
{
    class PlayGamesService : IOnlineService
    {
        SavedGames savedGames;

        public void Activate()
        {
            PlayGamesClientConfiguration config = new
                PlayGamesClientConfiguration.Builder()
                .EnableSavedGames()
                .Build();

            PlayGamesPlatform.DebugLogEnabled = true;

            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.Activate();

            savedGames = new SavedGames();
        }

        public void SignIn()
        {
            if (!Social.localUser.authenticated)
            {
                Social.localUser.Authenticate(SignInCallback);
            }
            else
            {
                Debug.Log("GPG::Already signed in");
            }
        }

        public void SignOut()
        {
            if (Social.localUser.authenticated)
            {
                PlayGamesPlatform.Instance.SignOut();
            }
            else
            {
                Debug.Log("GPG::Already signed out");
            }
        }

        void SignInCallback(bool success)
        {
            if (success)
            {
                Debug.Log("GPG::Authenticated");
            }
            else
            {
                Debug.LogError("GPG::Error::Authenticated");
            }
        }

        public void ShowAchievements()
        {
            if (PlayGamesPlatform.Instance.localUser.authenticated)
            {
                PlayGamesPlatform.Instance.ShowAchievementsUI();
            }
            else
            {
                Debug.Log("GPG::Cannot show Achievements, not logged in");
            }
        }

        public void ReportProgress(string id, double value)
        {
            if (Social.localUser.authenticated)
            {
                PlayGamesPlatform.Instance.ReportProgress(
                    id, value, (bool success) => Debug.Log("GPG::Achievement: " + success));
            }
            else
            {
                Debug.Log("GPG::Report progress error: not authenticated");
            }
        }

        public void IncrementProgress(string id, int steps)
        {
            if (Social.localUser.authenticated)
            {
                PlayGamesPlatform.Instance.IncrementAchievement(
                    id, steps, (bool success) => Debug.Log("GPG::Achievement increment: " + success));
            }
            else
            {
                Debug.Log("GPG::Report progress error: not authenticated");
            }
        }

        public void ShowLeaderboards()
        {
            if (PlayGamesPlatform.Instance.localUser.authenticated)
            {
                PlayGamesPlatform.Instance.ShowLeaderboardUI();
            }
            else
            {
                Debug.Log("GPG::Cannot show leaderboard: not authenticated");
            }
        }

        public void ReportScore(string id, long score)
        {
            if (PlayGamesPlatform.Instance.localUser.authenticated)
            {
                // Note: make sure to add 'using GooglePlayGames'
                PlayGamesPlatform.Instance.ReportScore(score, id,
                    (bool success) => Debug.Log("GPG::Leaderboard update: " + success));
            }
            else
            {
                Debug.Log("GPG::Cannot show leaderboard: not authenticated");
            }
        }

        public void Save(byte[] data)
        {
            savedGames.SaveToCloud(data);
        }

        public bool IsLoaded { get; private set; }

        /// <summary>
        /// Use this array after calling Load().
        /// It's not null, if SGIsLoaded is true
        /// </summary>
        public byte[] LoadedData { get; private set; }

        public void Load()
        {
            IsLoaded = false;
            LoadedData = null;

            savedGames.LoadFromCloud(FinishLoad);
        }

        void FinishLoad(byte[] loadedData)
        {
            IsLoaded = true;
            LoadedData = loadedData;
        }
    }
}