using I2.Loc.SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ApClient
{
    public class APClientSaveManager
    {
        private APSaveData aPSaveData;

        public APClientSaveManager() { 
            aPSaveData = new APSaveData();
            aPSaveData.ProcessedIndex = 0;
        }
        public void setSeed(string seed)
        {
            aPSaveData.seed = seed;
        }

        public void setProcessedIndex(int index)
        {
            aPSaveData.ProcessedIndex = index;
        }

        public void increaseProcessedIndex()
        {
            aPSaveData.ProcessedIndex++;
        }
        public int getProcessedIndex()
        {
            return aPSaveData.ProcessedIndex;
        }

        private string GetBaseDirectory()
        {
            return Path.GetDirectoryName(this.GetType().Assembly.Location);
        }
        private string getGdSavePath()
        {
            return $"{this.GetBaseDirectory()}/Saves/{MyPluginInfo.PLUGIN_GUID}_{aPSaveData.seed}.gd";
        }

        private string getJsonSavePath()
        {
            return $"{this.GetBaseDirectory()}/Saves/{MyPluginInfo.PLUGIN_GUID}_{aPSaveData.seed}.json";
        }

        public bool doesSaveExist() { return File.Exists(getJsonSavePath()) || File.Exists(getGdSavePath()); }
        public void Save(int saveSlotIndex)
        {
            System.IO.Directory.CreateDirectory($"{this.GetBaseDirectory()}/Saves/");
            Plugin.Log("AP Save saveSlotIndex " + saveSlotIndex);
            CSaveLoad.m_SavedGame = CGameData.instance;

            //try
            //{
                
            //    using FileStream fileStream = File.Create(getGdSavePath(Plugin.session.RoomState.Seed));
            //    new BinaryFormatter().Serialize(fileStream, CSaveLoad.m_SavedGame);
            //    fileStream.Close();
            //}
            //catch
            //{
            
            //    Debug.Log("Error saving gd");
            //}

            try
            {

                string contents = JsonUtility.ToJson(CSaveLoad.m_SavedGame);
                string modified = contents.TrimEnd('}') + $", \"processedItems\": \"{aPSaveData.ProcessedIndex}\" }}";
                File.WriteAllText(getJsonSavePath(), modified);
            }
            catch
            {
                Debug.Log("Error saving JSON");
            }
        }


        public bool Load()
        {
            var path = getGdSavePath();
            var jsonpath = getJsonSavePath();

            bool flag = false;
            if (File.Exists(jsonpath))
            {
                try
                {
                    string text = File.ReadAllText(jsonpath);

                    if (!(text == "") && text != null)
                    {
                        Match match = Regex.Match(text, @"""processedItems"":\s*""([^""]*)""");
                        if (match.Success)
                        {
                            string LastExtraDataValue = match.Groups[1].Value;
                            int.TryParse(LastExtraDataValue, out int data);
                            aPSaveData.ProcessedIndex = data;
                            Debug.Log($"Extracted processedItems: {data}");
                        }
                        
                        text = Regex.Replace(text, @",\s*""processedItems"":\s*""[^""]*""", "");

                        Debug.Log("Modified JSON before deserialization");
                        CSaveLoad.m_SavedGame = JsonUtility.FromJson<CGameData>(text);
                        return true;
                    }

                    flag = true;
                }
                catch
                {
                    flag = true;
                }
            }
            //if (flag)
            //{
            //    if (File.Exists(path))
            //    {
            //        using (FileStream fileStream = File.Open(path, FileMode.Open))
            //        {
            //            BinaryFormatter binaryFormatter = new BinaryFormatter();
            //            if (fileStream.Length > 0)
            //            {
            //                try
            //                {
            //                    fileStream.Position = 0L;
            //                    CSaveLoad.m_SavedGame = (CGameData)binaryFormatter.Deserialize(fileStream);
            //                    fileStream.Close();
            //                    binaryFormatter = null;
            //                    return true;
            //                }
            //                catch
            //                {
            //                    fileStream.Close();
            //                    binaryFormatter = null;
            //                    return false;
            //                }
            //            }

            //            fileStream.Close();
            //            binaryFormatter = null;
            //            return false;
            //        }
            //    }
            //}
            return false;
        }

    }
}
