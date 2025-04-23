using ApClient.data;
using I2.Loc.SimpleJSON;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ApClient
{
    public class APClientSaveManager
    {
        private APSaveData aPSaveData;

        public APClientSaveManager() {
            aPSaveData = new APSaveData();
            Clear();
        }

        public void Clear()
        {
            aPSaveData.ProcessedIndex = 0;
            aPSaveData.MoneyMultiplier = 1;
            aPSaveData.Luck = 0;
            aPSaveData.newCards = new List<int> { };
            for (int i = 1; i < (int)EMonsterType.MAX; i++)
            {
                aPSaveData.newCards.Add(i);
            }
        }
        public void setSeed(string seed)
        {
            aPSaveData.seed = seed;
        }

        public void setProcessedIndex(int index)
        {
            aPSaveData.ProcessedIndex = index;
        }

        public void IncreaseProcessedIndex()
        {
            aPSaveData.ProcessedIndex++;
        }
        public int GetProcessedIndex()
        {
            return aPSaveData.ProcessedIndex;
        }
        public void IncreaseMoneyMult()
        {
            aPSaveData.MoneyMultiplier = aPSaveData.MoneyMultiplier += 1.025f;
        }
        public float GetMoneyMult()
        {
            return aPSaveData.MoneyMultiplier;
        }

        public int GetLuck()
        {
            return aPSaveData.Luck;
        }

        public void IncreaseLuck()
        {
            aPSaveData.Luck++;
        }
        public void DecreaseLuck()
        {
            if (aPSaveData.Luck > 0)
            {
                aPSaveData.Luck--;
            }
        }
        public List<int> GetIncompleteCards()
        {
            return aPSaveData.newCards;
        }

        public void CompleteCardId(int id)
        {
            aPSaveData.newCards.Remove(id);
        }

        public void setIncompleteCards(List<int> list)
        {
            aPSaveData.newCards = list;
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
                string dictJson = JsonConvert.SerializeObject(aPSaveData.newCards);
                string modified = contents.TrimEnd('}') + $", \"processedItems\": \"{aPSaveData.ProcessedIndex}\", \"newCardIds\": {dictJson} , \"maxMoney\": \"{aPSaveData.MoneyMultiplier}\", \"luck\": \"{aPSaveData.Luck}\"}}";
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
                        //max money
                        Match match = Regex.Match(text, @"""maxMoney"":\s*""([^""]*)""");
                        if (match.Success)
                        {
                            string LastExtraDataValue = match.Groups[1].Value;
                            int.TryParse(LastExtraDataValue, out int data);
                            aPSaveData.MoneyMultiplier = data;
                            Debug.Log($"Extracted maxMoney: {data}");
                        }

                        text = Regex.Replace(text, @",\s*""maxMoney"":\s*""[^""]*""", "");

                        //luck
                        match = Regex.Match(text, @"""luck"":\s*""([^""]*)""");
                        if (match.Success)
                        {
                            string LastExtraDataValue = match.Groups[1].Value;
                            int.TryParse(LastExtraDataValue, out int data);
                            aPSaveData.Luck = data;
                            Debug.Log($"Extracted luck: {data}");
                        }

                        text = Regex.Replace(text, @",\s*""luck"":\s*""[^""]*""", "");
                        //new ids
                        Match dictmatch = Regex.Match(text, "\"newCardIds\"\\s*:\\s*\\{(.*?)\\}\\s*(,|})", RegexOptions.Singleline);

                        if (dictmatch.Success)
                        {
                            string dictJson = "{" + dictmatch.Groups[1].Value + "}";
                            aPSaveData.newCards = JsonConvert.DeserializeObject<List<int>>(dictJson);
                        }
                        string jsonWithoutDict = Regex.Replace(text, "\"newCardIds\"\\s*:\\s*\\{(.*?)\\}\\s*(,|})", "", RegexOptions.Singleline);

                        //recieved items
                        match = Regex.Match(text, @"""processedItems"":\s*""([^""]*)""");
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
