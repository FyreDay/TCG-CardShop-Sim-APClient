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
    public int cachedCommonChecks = -1;
    public int cachedRareChecks= -1;
    public int cachedEpicChecks = -1;
    public int cachedLegendaryChecks= -1;

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

    public void IncreaseCardChecks(ECollectionPackType packType)
    {
        switch (packType)
        {
            case ECollectionPackType.BasicCardPack:
                aPSaveData.TetramonCommonChecksFound++;
                break;
            case ECollectionPackType.RareCardPack:
                aPSaveData.TetramonRareChecksFound++;
                break;
            case ECollectionPackType.EpicCardPack:
                aPSaveData.TetramonEpicChecksFound++;
                break;
            case ECollectionPackType.LegendaryCardPack:
                aPSaveData.TetramonLegendaryChecksFound++;
                break;
            case ECollectionPackType.DestinyBasicCardPack:
                aPSaveData.DestinyCommonChecksFound++;
                break;
            case ECollectionPackType.DestinyRareCardPack:
                aPSaveData.DestinyRareChecksFound++;
                break;
            case ECollectionPackType.DestinyEpicCardPack:
                aPSaveData.DestinyEpicChecksFound++;
                break;
            case ECollectionPackType.DestinyLegendaryCardPack:
                aPSaveData.DestinyLegendaryChecksFound++;
                break;

        }
    }

    public void IncreaseGhostChecks()
    {
        aPSaveData.GhostCardsSold++;
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

            if (ERarity.Common == rarity)
            {

                cachedCommonChecks += 12;


                if (Plugin.m_SessionHandler.GetSlotData().CardSanity > packtype + (int)ERarity.Common)
                {
                    checkAmount += (Plugin.m_SessionHandler.GetSlotData().BorderInSanity + 1) * (Plugin.m_SessionHandler.GetSlotData().FoilInSanity ? 2 : 1);
                }
            }

            if (ERarity.Rare == rarity)
            {

                cachedRareChecks += 12;
                if (Plugin.m_SessionHandler.GetSlotData().CardSanity > packtype + (int)ERarity.Rare)
                {
                    checkAmount += (Plugin.m_SessionHandler.GetSlotData().BorderInSanity + 1) * (Plugin.m_SessionHandler.GetSlotData().FoilInSanity ? 2 : 1);

                }
            }

            if (ERarity.Epic == rarity)
            {

                cachedEpicChecks += 12;

                if (Plugin.m_SessionHandler.GetSlotData().CardSanity > packtype + (int)ERarity.Epic)
                {
                    checkAmount += (Plugin.m_SessionHandler.GetSlotData().BorderInSanity + 1) * (Plugin.m_SessionHandler.GetSlotData().FoilInSanity ? 2 : 1);
                }
            }

            if (ERarity.Legendary == rarity)
            {

                cachedLegendaryChecks += 12;
                if (Plugin.m_SessionHandler.GetSlotData().CardSanity > packtype + (int)ERarity.Legendary)
                {
                    checkAmount += (Plugin.m_SessionHandler.GetSlotData().BorderInSanity + 1) * (Plugin.m_SessionHandler.GetSlotData().FoilInSanity ? 2 : 1);
                }
            }
        }

        if (cardExpansionType == ECardExpansionType.Tetramon)
        {
            cachedTetramonCheckCount = checkAmount;
        }

        if (cardExpansionType == ECardExpansionType.Destiny)
        {
            cachedDestinyCheckCount = checkAmount;
        }
        return checkAmount;
    }

    public int GetTotalCardChecks(ECollectionPackType packType)
    {
        switch (packType)
        {
            case ECollectionPackType.BasicCardPack:
                return cachedCommonChecks;
            case ECollectionPackType.RareCardPack:
                return cachedRareChecks;
            case ECollectionPackType.EpicCardPack:
                return cachedEpicChecks;
            case ECollectionPackType.LegendaryCardPack:
                return cachedLegendaryChecks;
            case ECollectionPackType.DestinyBasicCardPack:
                return cachedCommonChecks;
            case ECollectionPackType.DestinyRareCardPack:
                return cachedRareChecks;
            case ECollectionPackType.DestinyEpicCardPack:
                return cachedEpicChecks;
            case ECollectionPackType.DestinyLegendaryCardPack:
                return cachedLegendaryChecks;

        }

        return -1;
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
        return aPSaveData.TetramonCommonChecksFound + aPSaveData.TetramonRareChecksFound + aPSaveData.TetramonEpicChecksFound + aPSaveData.TetramonLegendaryChecksFound;
    }

    public int GetDestinyChecks()
    {
        return aPSaveData.DestinyCommonChecksFound + aPSaveData.DestinyRareChecksFound + aPSaveData.DestinyEpicChecksFound + aPSaveData.DestinyLegendaryChecksFound;
    }

    public int GetCardChecks(ECollectionPackType packType)
    {
        switch (packType)
        {
            case ECollectionPackType.BasicCardPack:
                return aPSaveData.TetramonCommonChecksFound;
            case ECollectionPackType.RareCardPack:
                return aPSaveData.TetramonRareChecksFound;
            case ECollectionPackType.EpicCardPack:
                return aPSaveData.TetramonEpicChecksFound;
            case ECollectionPackType.LegendaryCardPack:
                return aPSaveData.TetramonLegendaryChecksFound;
            case ECollectionPackType.DestinyBasicCardPack:
                return aPSaveData.DestinyCommonChecksFound;
            case ECollectionPackType.DestinyRareCardPack:
                return aPSaveData.DestinyRareChecksFound;
            case ECollectionPackType.DestinyEpicCardPack:
                return aPSaveData.DestinyEpicChecksFound;
            case ECollectionPackType.DestinyLegendaryCardPack:
                return aPSaveData.DestinyLegendaryChecksFound;

        }

        return -1;
    }

    public int GetGhostChecks()
    {
        return aPSaveData.GhostCardsSold;
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
            modified = modified.TrimEnd('}') + $", \"TetramonCommonChecksFound\": \"{aPSaveData.TetramonCommonChecksFound}\", \"DestinyCommonChecksFound\": \"{aPSaveData.DestinyCommonChecksFound}\"}}";
            modified = modified.TrimEnd('}') + $", \"TetramonRareChecksFound\": \"{aPSaveData.TetramonRareChecksFound}\", \"DestinyRareChecksFound\": \"{aPSaveData.DestinyRareChecksFound}\"}}";
            modified = modified.TrimEnd('}') + $", \"TetramonEpicChecksFound\": \"{aPSaveData.TetramonEpicChecksFound}\", \"DestinyEpicChecksFound\": \"{aPSaveData.DestinyEpicChecksFound}\"}}";
            modified = modified.TrimEnd('}') + $", \"TetramonLegendaryChecksFound\": \"{aPSaveData.TetramonLegendaryChecksFound}\", \"DestinyLegendaryChecksFound\": \"{aPSaveData.DestinyLegendaryChecksFound}\"}}";
            modified = modified.TrimEnd('}') + $", \"GhostCardsSold\": \"{aPSaveData.GhostCardsSold}\"}}";
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
                        aPSaveData.GhostCardsSold = data;
                        Debug.Log($"Extracted GhostChecksFound: {data}");
                    }
                    text = Regex.Replace(text, @",\s*""GhostChecksFound"":\s*""[^""]*""", "");

                    //Destiny num
                    Match destinymatch = Regex.Match(text, @"""DestinyLegendaryChecksFound"":\s*""([^""]*)""");
                    if (destinymatch.Success)
                    {
                        string LastExtraDataValue = destinymatch.Groups[1].Value;
                        int.TryParse(LastExtraDataValue, out int data);
                        aPSaveData.DestinyLegendaryChecksFound = data;
                        Debug.Log($"Extracted DestinyLegendaryChecksFound: {data}");
                    }
                    text = Regex.Replace(text, @",\s*""DestinyLegendaryChecksFound"":\s*""[^""]*""", "");

                    //trtramon num
                    Match basicmatch = Regex.Match(text, @"""TetramonLegendaryChecksFound"":\s*""([^""]*)""");
                    if (basicmatch.Success)
                    {
                        string LastExtraDataValue = basicmatch.Groups[1].Value;
                        int.TryParse(LastExtraDataValue, out int data);
                        aPSaveData.TetramonLegendaryChecksFound = data;
                        Debug.Log($"Extracted TetramonLegendaryChecksFound: {data}");
                    }
                    text = Regex.Replace(text, @",\s*""TetramonLegendaryChecksFound"":\s*""[^""]*""", "");
                    //Destiny num
                    destinymatch = Regex.Match(text, @"""DestinyEpicChecksFound"":\s*""([^""]*)""");
                    if (destinymatch.Success)
                    {
                        string LastExtraDataValue = destinymatch.Groups[1].Value;
                        int.TryParse(LastExtraDataValue, out int data);
                        aPSaveData.DestinyEpicChecksFound = data;
                        Debug.Log($"Extracted DestinyEpicChecksFound: {data}");
                    }
                    text = Regex.Replace(text, @",\s*""DestinyEpicChecksFound"":\s*""[^""]*""", "");

                    //trtramon num
                    basicmatch = Regex.Match(text, @"""TetramonEpicChecksFound"":\s*""([^""]*)""");
                    if (basicmatch.Success)
                    {
                        string LastExtraDataValue = basicmatch.Groups[1].Value;
                        int.TryParse(LastExtraDataValue, out int data);
                        aPSaveData.TetramonEpicChecksFound = data;
                        Debug.Log($"Extracted TetramonEpicChecksFound: {data}");
                    }
                    text = Regex.Replace(text, @",\s*""TetramonEpicChecksFound"":\s*""[^""]*""", "");
                    //Destiny num
                    destinymatch = Regex.Match(text, @"""DestinyRareChecksFound"":\s*""([^""]*)""");
                    if (destinymatch.Success)
                    {
                        string LastExtraDataValue = destinymatch.Groups[1].Value;
                        int.TryParse(LastExtraDataValue, out int data);
                        aPSaveData.DestinyRareChecksFound = data;
                        Debug.Log($"Extracted DestinyRareChecksFound: {data}");
                    }
                    text = Regex.Replace(text, @",\s*""DestinyRareChecksFound"":\s*""[^""]*""", "");

                    //trtramon num
                    basicmatch = Regex.Match(text, @"""TetramonRareChecksFound"":\s*""([^""]*)""");
                    if (basicmatch.Success)
                    {
                        string LastExtraDataValue = basicmatch.Groups[1].Value;
                        int.TryParse(LastExtraDataValue, out int data);
                        aPSaveData.TetramonRareChecksFound = data;
                        Debug.Log($"Extracted TetramonRareChecksFound: {data}");
                    }

                    text = Regex.Replace(text, @",\s*""TetramonRareChecksFound"":\s*""[^""]*""", "");

                    //Destiny num
                    destinymatch = Regex.Match(text, @"""DestinyCommonChecksFound"":\s*""([^""]*)""");
                    if (destinymatch.Success)
                    {
                        string LastExtraDataValue = destinymatch.Groups[1].Value;
                        int.TryParse(LastExtraDataValue, out int data);
                        aPSaveData.DestinyCommonChecksFound = data;
                        Debug.Log($"Extracted DestinyCommonChecksFound: {data}");
                    }
                    text = Regex.Replace(text, @",\s*""DestinyCommonChecksFound"":\s*""[^""]*""", "");
                    //trtramon num
                    basicmatch = Regex.Match(text, @"""TetramonCommonChecksFound"":\s*""([^""]*)""");
                    if (basicmatch.Success)
                    {
                        string LastExtraDataValue = basicmatch.Groups[1].Value;
                        int.TryParse(LastExtraDataValue, out int data);
                        aPSaveData.TetramonCommonChecksFound = data;
                        Debug.Log($"Extracted TetramonCommonChecksFound: {data}");
                    }
                    text = Regex.Replace(text, @",\s*""TetramonCommonChecksFound"":\s*""[^""]*""", "");

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
            }
            catch
            {
                Plugin.Log("Failed to retrieve save data");
            }
        }
        return false;
    }

}
