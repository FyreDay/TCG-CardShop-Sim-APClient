using ApClient.data;
using ApClient.mapping;
using I2.Loc.SimpleJSON;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Rendering;
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
    public int customersPlayedGames = 0;

    public APClientSaveManager() {
        Clear();
    }

    public void Clear()
    {
        aPSaveData = new APSaveData();
        aPSaveData.ProcessedIndex = 0;
        aPSaveData.MoneyMultiplier = 1;

        cachedTetramonCheckCount = -1;
        cachedDestinyCheckCount = -1;
        cachedCommonChecks = -1;
        cachedRareChecks = -1;
        cachedEpicChecks = -1;
        cachedLegendaryChecks = -1;
        customersPlayedGames = 0;
}
    public void setConnectionData(string seed, string slot)
    {
        aPSaveData.seed = seed;
        aPSaveData.slotname = slot;
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
    public int GetEventGamesPlayed()
    {
        return aPSaveData.EventGamesPlayed;
    }

    public void IncreaseCustomersPlayed()
    {
        customersPlayedGames++;
        if (customersPlayedGames % 2 == 0)
        {
            aPSaveData.EventGamesPlayed++;
        }
    }
    public void DecreaseLuck()
    {
        if (aPSaveData.Luck > 0)
        {
            aPSaveData.Luck--;
        }
    }

    public bool isEventUnlocked(EGameEventFormat format)
    {
        return aPSaveData.UnlockedGameEvents.Contains(format);
    }

    public void setEventUnlocked(EGameEventFormat format)
    {
        aPSaveData.UnlockedGameEvents.Add(format);
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

    public void IncreaseCardSold(ECardExpansionType expansionType, ERarity rarity)
    {
        if (expansionType == ECardExpansionType.Tetramon)
        {
            switch (rarity)
            {
                case ERarity.Common:
                    aPSaveData.TetramonCommonChecksSold++;
                    break;
                case ERarity.Rare:
                    aPSaveData.TetramonRareChecksSold++;
                    break;
                case ERarity.Epic:
                    aPSaveData.TetramonEpicChecksSold++;
                    break;
                case ERarity.Legendary:
                    aPSaveData.TetramonLegendaryChecksSold++;
                    break;
            }
        }
        else
        {
            switch (rarity)
            {
                case ERarity.Common:
                    aPSaveData.DestinyCommonChecksSold++;
                    break;
                case ERarity.Rare:
                    aPSaveData.DestinyRareChecksSold++;
                    break;
                case ERarity.Epic:
                    aPSaveData.DestinyEpicChecksSold++;
                    break;
                case ERarity.Legendary:
                    aPSaveData.DestinyLegendaryChecksSold++;
                    break;
            }
        }
    }

    public int GetCardsSold(ECardExpansionType expansionType, ERarity rarity)
    {
        if (expansionType == ECardExpansionType.Tetramon)
        {
            switch (rarity)
            {
                case ERarity.Common:
                    return aPSaveData.TetramonCommonChecksSold;
                case ERarity.Rare:
                    return aPSaveData.TetramonRareChecksSold;
                case ERarity.Epic:
                    return aPSaveData.TetramonEpicChecksSold;
                case ERarity.Legendary:
                    return aPSaveData.TetramonLegendaryChecksSold;
            }
        }
        else
        {
            switch (rarity)
            {
                case ERarity.Common:
                    return aPSaveData.DestinyCommonChecksSold;
                case ERarity.Rare:
                    return aPSaveData.DestinyRareChecksSold;
                case ERarity.Epic:
                    return aPSaveData.DestinyEpicChecksSold;
                case ERarity.Legendary:
                    return aPSaveData.DestinyLegendaryChecksSold;
            }
        }
        return -1;
    }

    public void IncreaseGhostChecks()
    {
        aPSaveData.GhostCardsSold++;
    }

    public int getTotalExpansionChecks(ECardExpansionType cardExpansionType)
    {
        if(cardExpansionType == ECardExpansionType.Ghost)
        {
            if (Plugin.m_SessionHandler.GetSlotData().Goal == 2)
            {
                return Plugin.m_SessionHandler.GetSlotData().GhostGoalAmount;
            }
            else
            {
                return 0;
            }
        }

        if (cardExpansionType != ECardExpansionType.Tetramon && cardExpansionType != ECardExpansionType.Destiny)
        {
            return 0;
        }

        if (cardExpansionType == ECardExpansionType.Tetramon && cachedTetramonCheckCount != -1)
        {
            Plugin.Log("-1");
            return cachedTetramonCheckCount;
        }

        if (cardExpansionType == ECardExpansionType.Destiny && cachedDestinyCheckCount != -1)
        {
            Plugin.Log("-1");
            return cachedDestinyCheckCount;
        }
        //if (Plugin.m_SessionHandler.GetSlotData().CardSanity == 0)
        //{
        //    cachedCommonChecks = Plugin.m_SessionHandler.GetSlotData().ChecksPerPack;
        //    cachedRareChecks = Plugin.m_SessionHandler.GetSlotData().ChecksPerPack;
        //    cachedEpicChecks = Plugin.m_SessionHandler.GetSlotData().ChecksPerPack;
        //    cachedLegendaryChecks = Plugin.m_SessionHandler.GetSlotData().ChecksPerPack;
        //    return Plugin.m_SessionHandler.GetSlotData().ChecksPerPack * 4;
        //}
        //else
        //{
        cachedCommonChecks = 0;
        cachedRareChecks = 0;
        cachedEpicChecks = 0;
        cachedLegendaryChecks = 0;
        int checkAmount = 0;
        for (int i = 0; i < InventoryBase.GetShownMonsterList(cardExpansionType).Count; i++)
        {
            ERarity rarity = InventoryBase.GetMonsterData(InventoryBase.GetShownMonsterList(cardExpansionType)[i]).Rarity;

            int packtype = cardExpansionType == ECardExpansionType.Destiny ? 4 : 0;

            if (ERarity.Common == rarity)
            {
                Plugin.Log("common");
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
        //}
    }

    
    public int GetTotalCountedCards(ECollectionPackType packType)
    {
        getTotalExpansionChecks(ECardExpansionType.Tetramon);
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
    public int GetSentChecks(ECollectionPackType packType)
    {
        int totalcards = Plugin.m_SaveManager.GetTotalCountedCards(packType);
        float maxcollect = (totalcards * (Plugin.m_SessionHandler.GetSlotData().CardCollectPercentage / 100f));
        float numPercheck = maxcollect / Plugin.m_SessionHandler.GetSlotData().ChecksPerPack;
        switch (packType)
        {
            case ECollectionPackType.BasicCardPack:
                return (int)(aPSaveData.TetramonCommonChecksFound / numPercheck);
            case ECollectionPackType.RareCardPack:
                return (int)(aPSaveData.TetramonRareChecksFound / numPercheck);
            case ECollectionPackType.EpicCardPack:
                return (int)(aPSaveData.TetramonEpicChecksFound / numPercheck);
            case ECollectionPackType.LegendaryCardPack:
                return (int)(aPSaveData.TetramonLegendaryChecksFound / numPercheck);
            case ECollectionPackType.DestinyBasicCardPack:
                return (int)(aPSaveData.DestinyCommonChecksFound / numPercheck);
            case ECollectionPackType.DestinyRareCardPack:
                return (int)(aPSaveData.DestinyRareChecksFound / numPercheck);
            case ECollectionPackType.DestinyEpicCardPack:
                return (int)(aPSaveData.DestinyEpicChecksFound / numPercheck);
            case ECollectionPackType.DestinyLegendaryCardPack:
                return (int)(aPSaveData.DestinyLegendaryChecksFound / numPercheck);

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
        return $"{this.GetBaseDirectory()}/Saves/{MyPluginInfo.PLUGIN_GUID}_{aPSaveData.slotname}_{aPSaveData.seed}.gd";
    }

    private string getJsonSavePath()
    {
        return $"{this.GetBaseDirectory()}/Saves/{MyPluginInfo.PLUGIN_GUID}_{aPSaveData.slotname}_{aPSaveData.seed}.json";
    }
    public bool doesSaveExist() { return File.Exists(getJsonSavePath()) || File.Exists(getGdSavePath()); }
    public void Save(int saveSlotIndex)
    {
        System.IO.Directory.CreateDirectory($"{this.GetBaseDirectory()}/Saves/");
        CSaveLoad.m_SavedGame = CGameData.instance;

        var wrapper = new SaveDataWrapper
        {
            gameData = CGameData.instance,
            ProcessedIndex = aPSaveData.ProcessedIndex,
            MoneyMultiplier = aPSaveData.MoneyMultiplier,
            Luck = aPSaveData.Luck,
            TetramonCommonChecksFound = aPSaveData.TetramonCommonChecksFound,
            DestinyCommonChecksFound = aPSaveData.DestinyCommonChecksFound,
            TetramonRareChecksFound = aPSaveData.TetramonRareChecksFound,
            DestinyRareChecksFound = aPSaveData.DestinyRareChecksFound,
            TetramonEpicChecksFound = aPSaveData.TetramonEpicChecksFound,
            DestinyEpicChecksFound = aPSaveData.DestinyEpicChecksFound,
            TetramonLegendaryChecksFound = aPSaveData.TetramonLegendaryChecksFound,
            DestinyLegendaryChecksFound = aPSaveData.DestinyLegendaryChecksFound,
            TetramonCommonChecksSold = aPSaveData.TetramonCommonChecksSold,
            DestinyCommonChecksSold = aPSaveData.DestinyCommonChecksSold,
            TetramonRareChecksSold = aPSaveData.TetramonRareChecksSold,
            DestinyRareChecksSold = aPSaveData.DestinyRareChecksSold,
            TetramonEpicChecksSold = aPSaveData.TetramonEpicChecksSold,
            DestinyEpicChecksSold = aPSaveData.DestinyEpicChecksSold,
            TetramonLegendaryChecksSold = aPSaveData.TetramonLegendaryChecksSold,
            DestinyLegendaryChecksSold = aPSaveData.DestinyLegendaryChecksSold,
            GhostCardsSold = aPSaveData.GhostCardsSold,
            EventGamesPlayed = aPSaveData.EventGamesPlayed
        };

        try
        {
            string json = JsonUtility.ToJson(wrapper, true);
            File.WriteAllText(getJsonSavePath(), json);
        }
        catch (Exception ex)
        {
            Plugin.Log("Error saving JSON: " + ex);
        }
    }

    public bool Load()
    {
        string jsonpath = getJsonSavePath();

        if (!File.Exists(jsonpath))
            return false;

        try
        {
            string text = File.ReadAllText(jsonpath);
            var wrapper = JsonConvert.DeserializeObject<SaveDataWrapper>(text);

            if (wrapper == null) return false;

            CSaveLoad.m_SavedGame = wrapper.gameData;

            aPSaveData.ProcessedIndex = wrapper.ProcessedIndex;
            aPSaveData.MoneyMultiplier = wrapper.MoneyMultiplier;
            aPSaveData.Luck = wrapper.Luck;
            aPSaveData.TetramonCommonChecksFound = wrapper.TetramonCommonChecksFound;
            aPSaveData.DestinyCommonChecksFound = wrapper.DestinyCommonChecksFound;
            aPSaveData.TetramonRareChecksFound = wrapper.TetramonRareChecksFound;
            aPSaveData.DestinyRareChecksFound = wrapper.DestinyRareChecksFound;
            aPSaveData.TetramonEpicChecksFound = wrapper.TetramonEpicChecksFound;
            aPSaveData.DestinyEpicChecksFound = wrapper.DestinyEpicChecksFound;
            aPSaveData.TetramonLegendaryChecksFound = wrapper.TetramonLegendaryChecksFound;
            aPSaveData.DestinyLegendaryChecksFound = wrapper.DestinyLegendaryChecksFound;
            aPSaveData.TetramonCommonChecksSold = wrapper.TetramonCommonChecksSold;
            aPSaveData.DestinyCommonChecksSold = wrapper.DestinyCommonChecksSold;
            aPSaveData.TetramonRareChecksSold = wrapper.TetramonRareChecksSold;
            aPSaveData.DestinyRareChecksSold = wrapper.DestinyRareChecksSold;
            aPSaveData.TetramonEpicChecksSold = wrapper.TetramonEpicChecksSold;
            aPSaveData.DestinyEpicChecksSold = wrapper.DestinyEpicChecksSold;
            aPSaveData.TetramonLegendaryChecksSold = wrapper.TetramonLegendaryChecksSold;
            aPSaveData.DestinyLegendaryChecksSold = wrapper.DestinyLegendaryChecksSold;
            aPSaveData.GhostCardsSold = wrapper.GhostCardsSold;
            aPSaveData.EventGamesPlayed = wrapper.EventGamesPlayed;

            return true;
        }
        catch
        {
            Plugin.Log("Failed to retrieve save data");
        }
        return false;
    }

    [Serializable]
    public class SaveDataWrapper
    {
        public CGameData gameData;
        public int ProcessedIndex;
        public List<int> newCards = new();
        public float MoneyMultiplier;
        public int Luck;
        public int TetramonCommonChecksFound;
        public int DestinyCommonChecksFound;
        public int TetramonRareChecksFound;
        public int DestinyRareChecksFound;
        public int TetramonEpicChecksFound;
        public int DestinyEpicChecksFound;
        public int TetramonLegendaryChecksFound;
        public int DestinyLegendaryChecksFound;
        public int TetramonCommonChecksSold;
        public int DestinyCommonChecksSold;
        public int TetramonRareChecksSold;
        public int DestinyRareChecksSold;
        public int TetramonEpicChecksSold;
        public int DestinyEpicChecksSold;
        public int TetramonLegendaryChecksSold;
        public int DestinyLegendaryChecksSold;
        public int GhostCardsSold;
        public int EventGamesPlayed;
    }

}
