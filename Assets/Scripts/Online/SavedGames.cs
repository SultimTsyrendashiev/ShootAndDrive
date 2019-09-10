using System;
using System.Text;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine;

namespace SD.Online
{
    /// <summary>
    /// GPG must be activated before using this class
    /// </summary>
    class SavedGames
    {
        const string SaveFileName = "SDPlayerData";

        Action<byte[]> afterLoadAction;

        public bool IsProcessing { get; private set; }
        public byte[] LoadedData { get; private set; }

        public void LoadFromCloud(Action<byte[]> afterLoadAction)
        {
            if (IsProcessing)
            {
                return;
            }

            if (Social.localUser.authenticated)
            {
                this.afterLoadAction = afterLoadAction;

                IsProcessing = true;
                Debug.Log("SavedGames::Loading game progress from the cloud.");

                ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(
                    SaveFileName,
                    DataSource.ReadCacheOrNetwork,
                    ConflictResolutionStrategy.UseLongestPlaytime,
                    OnFileOpenToLoad);
            }
            else
            {
                Debug.Log("SavedGames::Load::Not authenticated");
            }
        }

        public void SaveToCloud(byte[] dataToSave)
        {
            if (IsProcessing)
            {
                return;
            }

            if (Social.localUser.authenticated)
            {
                // save only reference, don't copy array
                LoadedData = dataToSave;

                IsProcessing = true;
                ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(SaveFileName, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnFileOpenToSave);
            }
            else
            {
                Debug.Log("SavedGames::Save::Not authenticated");
            }
        }

        void OnFileOpenToSave(SavedGameRequestStatus status, ISavedGameMetadata metaData)
        {
            if (status == SavedGameRequestStatus.Success)
            {
                byte[] data = LoadedData;

                SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();

                SavedGameMetadataUpdate updatedMetadata = builder.Build();

                ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(metaData, updatedMetadata, data, OnGameSave);
            }
            else
            {
                Debug.LogWarning("SavedGames::Error opening Saved Game" + status);
                IsProcessing = false;
            }
        }


        void OnFileOpenToLoad(SavedGameRequestStatus status, ISavedGameMetadata metaData)
        {
            if (status == SavedGameRequestStatus.Success)
            {
                ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(metaData, OnGameLoad);
            }
            else
            {
                Debug.LogWarning("SavedGames::Error opening Saved Game" + status);
                IsProcessing = false;
            }
        }

        void OnGameLoad(SavedGameRequestStatus status, byte[] bytes)
        {
            if (status == SavedGameRequestStatus.Success)
            {
                if (bytes != null)
                {
                    //LoadedData = BytesToString(bytes);

                    // create copy of loaded data
                    LoadedData = new byte[bytes.Length];
                    bytes.CopyTo(LoadedData, 0);

                    if (afterLoadAction != null)
                    {
                        afterLoadAction.Invoke(LoadedData);
                    }
                    else
                    {
                        Debug.Log("SavedGames::Action after loading is not specified");
                    }
                }
                else
                {
                    Debug.Log("SavedGames::No Data saved to the cloud");
                }
            }
            else
            {
                Debug.LogWarning("SavedGames::Error Saving" + status);
            }

            IsProcessing = false;
        }

        void OnGameSave(SavedGameRequestStatus status, ISavedGameMetadata metaData)
        {
            if (status != SavedGameRequestStatus.Success)
            {
                Debug.LogWarning("SavedGames::Error Saving" + status);
            }

            IsProcessing = false;
        }

        //byte[] StringToBytes(string stringToConvert)
        //{
        //    return Encoding.UTF8.GetBytes(stringToConvert);
        //}

        //string BytesToString(byte[] bytes)
        //{
        //    return Encoding.UTF8.GetString(bytes);
        //}
    }
}