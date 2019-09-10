using System;

namespace SD
{
    interface IOnlineService
    {
        void Activate();

        void SignIn();
        void SignOut();

        void ShowAchievements();
        void ShowLeaderboards();

        void ReportProgress(string id, double value);
        void IncrementProgress(string id, int steps);

        void Save(byte[] data);

        bool IsLoaded { get; }
        byte[] LoadedData { get; }
        void Load();
    }
}
