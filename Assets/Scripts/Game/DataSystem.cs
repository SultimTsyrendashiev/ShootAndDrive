using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using SD.PlayerLogic;
using System;

namespace SD.Game.Data
{
    static class DataSystem
    {
        const string SettingsFileName = "Settings.dat";
        const string PlayerDataFileName = "Player.dat";

        public static void SaveSettings(GlobalSettings settings)
        {
            string path = Application.persistentDataPath + "/" + SettingsFileName;

            using (FileStream file = new FileStream(path, FileMode.OpenOrCreate))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(file, settings);
            }
        }

        public static GlobalSettings LoadSettings()
        {
            string path = Application.persistentDataPath + "/" + SettingsFileName;
            
            if (File.Exists(path))
            {
                try
                {
                    // load from file
                    using (FileStream file = new FileStream(path, FileMode.Open))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();

                        // reset position in file
                        file.Position = 0;

                        return (GlobalSettings)formatter.Deserialize(file);
                    }
                }
                catch
                {
                    // return default settings
                    GlobalSettings settings = new GlobalSettings();
                    settings.SetDefaults();

                    return settings;
                }
            }
            else
            {
                // return default settings
                GlobalSettings settings = new GlobalSettings();
                settings.SetDefaults();

                return settings;
            }
        }

        public static void SaveInventory(PlayerInventory inventory)
        {
            string path = Application.persistentDataPath + "/" + PlayerDataFileName;

            using (FileStream file = new FileStream(path, FileMode.OpenOrCreate))
            {
                // load from PlayerInventory to InventoryData
                InventoryData data = new InventoryData();
                data.LoadFrom(inventory);

                // save file
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(file, data);
            }
        }

        /// <summary>
        /// Load data from file to the inventory
        /// </summary>
        public static void LoadInventory(PlayerInventory inventory)
        {
            try
            {
                string path = Application.persistentDataPath + "/" + PlayerDataFileName;

                if (File.Exists(path))
                {
                    // load from file
                    using (FileStream file = new FileStream(path, FileMode.Open))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();

                        // reset position in file
                        file.Position = 0;
                        // deserialize InventoryData
                        InventoryData fromFile = (InventoryData)formatter.Deserialize(file);

                        // load from it to PlayerInventory
                        fromFile.SaveTo(inventory);
                    }
                }
                else
                {
                    // return default
                    inventory.SetDefault();
                }
            }
            catch
            {
                inventory.SetDefault();
            }
        }
    }
}
