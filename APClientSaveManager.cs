using ApClient.data;
using I2.Loc.SimpleJSON;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

namespace ApClient;

public class APClientSaveManager
{
    private APSaveData aPSaveData;
    private int cachedTetramonCheckCount = -1;
    private int cachedDestinyCheckCount = -1;

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
        aPSaveData.MoneyMultiplier = aPSaveData.MoneyMultiplier += 0.1f;
    }
    public float GetMoneyMult()
    {
        if(aPSaveData.MoneyMultiplier < 1)
        {
            aPSaveData.MoneyMultiplier = 1;
        }
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

    public void IncreaseTetramonChecks()
    {
        aPSaveData.TetramonChecksFound++;
    }

    public void IncreaseDestinyChecks()
    {
        aPSaveData.DestinyChecksFound++;
    }

    public void IncreaseGhostChecks()
    {
        aPSaveData.GhostChecksFound++;
    }

    public int getTotalExpansionChecks(ECardExpansionType cardExpansionType)
    {
        if (cardExpansionType != ECardExpansionType.Tetramon && cardExpansionType != ECardExpansionType.Destiny)
        {
            return 0;
        }

        if (cardExpansionType == ECardExpansionType.Tetramon && cachedTetramonCheckCount != -1)
        {
            return cachedTetramonCheckCount;
        }

        if (cardExpansionType == ECardExpansionType.Destiny && cachedDestinyCheckCount != -1)
        {
            return cachedDestinyCheckCount;
        }

        int checkAmount = 0;
        for (int i = 0; i < InventoryBase.GetShownMonsterList(cardExpansionType).Count; i++)
        {
            ERarity rarity = InventoryBase.GetMonsterData(InventoryBase.GetShownMonsterList(cardExpansionType)[i]).Rarity;

            int packtype = cardExpansionType == ECardExpansionType.Destiny ? 4 : 0;

            if (ERarity.Common == rarity && Plugin.m_SessionHandler.GetSlotData().CardSanity > packtype + (int)ERarity.Common)
            {
                    checkAmount += (Plugin.m_SessionHandler.GetSlotData().BorderInSanity + 1) * (Plugin.m_SessionHandler.GetSlotData().FoilInSanity ? 2 : 1);
            }

            if (ERarity.Rare == rarity && Plugin.m_SessionHandler.GetSlotData().CardSanity > packtype + (int)ERarity.Rare)
            {
                checkAmount += (Plugin.m_SessionHandler.GetSlotData().BorderInSanity + 1) * (Plugin.m_SessionHandler.GetSlotData().FoilInSanity ? 2 : 1);
            }

            if (ERarity.Epic == rarity && Plugin.m_SessionHandler.GetSlotData().CardSanity > packtype + (int)ERarity.Epic)
            {
                checkAmount += (Plugin.m_SessionHandler.GetSlotData().BorderInSanity + 1) * (Plugin.m_SessionHandler.GetSlotData().FoilInSanity ? 2 : 1);
            }

            if (ERarity.Legendary == rarity && Plugin.m_SessionHandler.GetSlotData().CardSanity > packtype + (int)ERarity.Legendary)
            {
                checkAmount += (Plugin.m_SessionHandler.GetSlotData().BorderInSanity + 1) * (Plugin.m_SessionHandler.GetSlotData().FoilInSanity ? 2 : 1);
            }
        }

        if (cardExpansionType == ECardExpansionType.Tetramon && cachedTetramonCheckCount != -1)
        {
            cachedTetramonCheckCount = checkAmount;
        }

        if (cardExpansionType == ECardExpansionType.Destiny && cachedDestinyCheckCount != -1)
        {
            cachedDestinyCheckCount = checkAmount;
        }
        return checkAmount;
    }
    public int GetExpansionChecks(ECardExpansionType type)
    {
        switch(type)
        {
            case ECardExpansionType.Tetramon:
                return GetTetramonChecks();
            case ECardExpansionType.Destiny:
                return GetDestinyChecks();
            case ECardExpansionType.Ghost:
                return GetGhostChecks();
            default: return 0;
        }
    }
    public int GetTetramonChecks()
    {
        return aPSaveData.TetramonChecksFound;
    }

    public int GetDestinyChecks()
    {
        return aPSaveData.DestinyChecksFound;
    }

    public int GetGhostChecks()
    {
        return aPSaveData.GhostChecksFound;
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
            modified = modified.TrimEnd('}') + $", \"TetramonChecksFound\": \"{aPSaveData.TetramonChecksFound}\", \"DestinyChecksFound\": \"{aPSaveData.DestinyChecksFound}\", \"GhostChecksFound\": \"{aPSaveData.GhostChecksFound}\"}}";
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
                    //Ghost num
                    Match ghostmatch = Regex.Match(text, @"""GhostChecksFound"":\s*""([^""]*)""");
                    if (ghostmatch.Success)
                    {
                        string LastExtraDataValue = ghostmatch.Groups[1].Value;
                        int.TryParse(LastExtraDataValue, out int data);
                        aPSaveData.GhostChecksFound = data;
                        Debug.Log($"Extracted GhostChecksFound: {data}");
                    }


                    //Destiny num
                    Match destinymatch = Regex.Match(text, @"""DestinyChecksFound"":\s*""([^""]*)""");
                    if (destinymatch.Success)
                    {
                        string LastExtraDataValue = destinymatch.Groups[1].Value;
                        int.TryParse(LastExtraDataValue, out int data);
                        aPSaveData.DestinyChecksFound = data;
                        Debug.Log($"Extracted DestinyChecksFound: {data}");
                    }


                    //trtramon num
                    Match basicmatch = Regex.Match(text, @"""TetramonChecksFound"":\s*""([^""]*)""");
                    if (basicmatch.Success)
                    {
                        string LastExtraDataValue = basicmatch.Groups[1].Value;
                        int.TryParse(LastExtraDataValue, out int data);
                        aPSaveData.TetramonChecksFound = data;
                        Debug.Log($"Extracted TetramonChecksFound: {data}");
                    }

                    //max money
                    Match match = Regex.Match(text, @"""maxMoney"":\s*""([^""]*)""");
                    if (match.Success)
                    {
                        string LastExtraDataValue = match.Groups[1].Value;
                        float.TryParse(LastExtraDataValue, out float data);
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
        return false;
    }

}
