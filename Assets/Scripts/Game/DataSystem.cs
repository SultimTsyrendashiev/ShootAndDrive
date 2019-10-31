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

        public static void SaveInventory(PlayerInventory inventory, IOnlineService onlineService)
        {
            // load from PlayerInventory to InventoryData
            InventoryData inventoryData = new InventoryData();
            inventoryData.LoadFrom(inventory);

            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                // save file
                formatter.Serialize(stream, inventoryData);

                onlineService.Save(stream.GetBuffer());
            }
        }

        public static void SaveInventory(PlayerInventory inventory)
        {
            string path = Application.persistentDataPath + "/" + PlayerDataFileName;

            // load from PlayerInventory to InventoryData
            InventoryData inventoryData = new InventoryData();
            inventoryData.LoadFrom(inventory);

            using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                // save file
                formatter.Serialize(stream, inventoryData);
            }
        }

        /// <summary>
        /// Load data from byte array to the inventory
        /// </summary>
        public static void LoadInventory(PlayerInventory inventory, byte[] data, out InventoryData invData)
        {
            invData = null;

            try
            {
                using (MemoryStream stream = new MemoryStream(data))
                {
                    BinaryFormatter formatter = new BinaryFormatter();

                    // deserialize InventoryData
                    InventoryData fromFile = (InventoryData)formatter.Deserialize(stream);

                    // load from it to PlayerInventory
                    fromFile.SaveTo(inventory);

                    invData = fromFile;
                }
            }
            catch (Exception e)
            {
                Debug.Log("DataSystem::Setting inventory to default as: " + e.Message);

                inventory.SetDefault();
            }
        }

        public static void LoadInventory(PlayerInventory inventory, out InventoryData invData)
        {
            invData = null;

            try
            {
                string path = Application.persistentDataPath + "/" + PlayerDataFileName;

                if (File.Exists(path))
                {
                    // load from file
                    using (FileStream stream = File.Open(path, FileMode.Open))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();

                        // reset position in file
                        stream.Position = 0;

                        // deserialize InventoryData
                        InventoryData fromFile = (InventoryData)formatter.Deserialize(stream);

                        // load from it to PlayerInventory
                        fromFile.SaveTo(inventory);

                        invData = fromFile;
                    }
                }
                else
                {
                    // return default
                    inventory.SetDefault();
                }
            }
            catch (Exception e)
            {
                Debug.Log("DataSystem::Setting inventory to default as: " + e.Message);

                inventory.SetDefault();
            }
        }
    }
}