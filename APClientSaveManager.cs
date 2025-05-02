using ApClient.data;
using I2.Loc.SimpleJSON;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
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

        var wrapper = new SaveDataWrapper
        {
            gameData = CGameData.instance,
            ProcessedIndex = aPSaveData.ProcessedIndex,
            newCards = aPSaveData.newCards,
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
            GhostCardsSold = aPSaveData.GhostCardsSold
        };

        try
        {
            string json = JsonUtility.ToJson(wrapper, true);
            File.WriteAllText(getJsonSavePath(), json);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error saving JSON: " + ex);
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
            aPSaveData.newCards = wrapper.newCards ?? new();
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
    }

}
