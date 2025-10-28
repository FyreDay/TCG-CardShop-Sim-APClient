using ApClient.data;
using ApClient.mapping;
using ApClient.ui;
using I2.Loc.SimpleJSON;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace ApClient;

public class APClientSaveManager
{
    private APSaveData aPSaveData;
    private APAchievementManager achievementManager;

    public Dictionary<EGameEventFormat, int> customersPlayedGames;

    public APClientSaveManager() {
        Clear();
        achievementManager = new APAchievementManager();
    }

    public void Clear()
    {
        aPSaveData = new APSaveData();
        aPSaveData.ProcessedIndex = 0;
        aPSaveData.MoneyMultiplier = 1;
        aPSaveData.StoredXP = 0;

        customersPlayedGames = new Dictionary<EGameEventFormat, int>();

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

    public void IncreaselicensesReceived()
    {
        aPSaveData.LicensesReceived++;
    }
    public int GetlicensesReceived()
    {
        return aPSaveData.LicensesReceived;
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

    public int IncreaseCustomersPlayed(EGameEventFormat format)
    {
        customersPlayedGames[format]++;

        if (customersPlayedGames[format] % 2 == 0)
        {
            return customersPlayedGames[format] / 2;
        }
        else return -1;
    }
    public void DecreaseLuck()
    {
        if (aPSaveData.Luck > 0)
        {
            aPSaveData.Luck--;
        }
    }

    public void IncreaseGhostChecks()
    {
        aPSaveData.GhostCardsSold++;
    } 

    public int GetGhostChecks()
    {
        return aPSaveData.GhostCardsSold;
    }

    public int GetStoredXP(int maxToGrab)
    {
        if (aPSaveData.StoredXP > maxToGrab)
        {
            aPSaveData.StoredXP -= maxToGrab;
            return maxToGrab;
        }
        return aPSaveData.StoredXP;
    }

    public int TotalStoredXP()
    {
        return aPSaveData.StoredXP;
    }

    public void IncreaseStoredXP(int xp)
    {
        Plugin.Log($"xp: {xp}");
        aPSaveData.StoredXP += xp;
    }
    public void AddOpenedCard(CardData card)
    {
        var cardRecord = achievementManager.AddOpenedCard(card);
    
        if (Plugin.m_SessionHandler.GetSlotData().CardSanity > 0
            && cardRecord.Opened == 1
            && Plugin.m_SessionHandler.maxBorder() >= (int)card.borderType)
        {

            card.isFoil = Plugin.m_SessionHandler.GetSlotData().CardSanity == 2 ? card.isFoil : false;
            
            Plugin.m_SessionHandler.CompleteLocationChecks(CardMapping.getId(card));
        }
        //

        //send achiement
        //Plugin.m_SessionHandler.CompleteLocationChecks(CardMapping.getCheckId(packtype, i-1));
    }

    public void AddSoldCard(CardData card)
    {
        var achievements = achievementManager.AddSoldCard(card);
    }

    public void AddGradedCard(CardData card)
    {
        var achievements = achievementManager.AddGradedCard(card);
        //send card grade achievements
    }

    public CardData GenerateUnopenedCard(int maxBorderInclusive, bool uniqueFoil)
    {
        for (int monster = 1; monster < (int)EMonsterType.MAX; monster++)
        {
            for (int border = 0; border <= maxBorderInclusive; border++)
            {
                for (int destiny = 0; destiny < 2; destiny++)
                {
                    if (uniqueFoil)
                    {
                        for (int foil = 0; foil < 2; foil++)
                        {
                            var candidate = new CardData
                            {
                                monsterType = (EMonsterType)monster,
                                borderType = (ECardBorderType)border,
                                isDestiny = destiny == 1,
                                isFoil = foil == 1,
                                isChampionCard = false
                            };

                            int hash = candidate.GetUniqueHash();
                            if (!achievementManager.HasCardOpened(hash))
                                return candidate;
                        }
                    }
                    else
                    {
                        var candidate1 = new CardData
                        {
                            monsterType = (EMonsterType)monster,
                            borderType = (ECardBorderType)border,
                            isDestiny = destiny == 1,
                            isFoil = false,
                            isChampionCard = false
                        };

                        var candidate2 = new CardData
                        {
                            monsterType = (EMonsterType)monster,
                            borderType = (ECardBorderType)border,
                            isDestiny = destiny == 1,
                            isFoil = true,
                            isChampionCard = false
                        };
                        //if I don't have BOTH the foil and non-foil
                        if (!achievementManager.HasCardOpened(candidate1.GetUniqueHash()) && !achievementManager.HasCardOpened(candidate2.GetUniqueHash()))
                        {
                            return UnityEngine.Random.Range(0, 2) > 0 ? candidate1 : candidate2;
                        }

                    }
                }
            }
        }
        return CardHelper.CardRoller((ECollectionPackType)UnityEngine.Random.Range(0, 8));
    }

    private string GetBaseDirectory()
    {
        return Application.persistentDataPath;
    }
    private string getGdSavePath()
    {
        return $"{this.GetBaseDirectory()}/APSaves/{MyPluginInfo.PLUGIN_GUID}_{aPSaveData.slotname}_{aPSaveData.seed}.gd";
    }

    private string getJsonSavePath()
    {
        return $"{this.GetBaseDirectory()}/APSaves/{MyPluginInfo.PLUGIN_GUID}_{aPSaveData.slotname}_{aPSaveData.seed}.json";
    }
    public bool doesSaveExist() { return File.Exists(getJsonSavePath()) || File.Exists(getGdSavePath()); }
    public void Save(int saveSlotIndex)
    {
        System.IO.Directory.CreateDirectory($"{this.GetBaseDirectory()}/APSaves/");
        CSaveLoad.m_SavedGame = CGameData.instance;

        string unityJson = JsonUtility.ToJson(CGameData.instance, true);

        var modData = new SaveDataWrapper
        {
            CardProgress = achievementManager.Save(),
            OpenAchievements = APinfoMenu.Instance.cardOpenItems,
            SellAchievements = APinfoMenu.Instance.cardSellItems,
            GradeAchievements = APinfoMenu.Instance.cardGradeItems,
            ProcessedIndex = aPSaveData.ProcessedIndex,
            MoneyMultiplier = aPSaveData.MoneyMultiplier,
            Luck = aPSaveData.Luck,
            GhostCardsSold = aPSaveData.GhostCardsSold,
            EventGamesPlayed = aPSaveData.EventGamesPlayed,
            LicensesReceived = aPSaveData.LicensesReceived,
            StoredXP = aPSaveData.StoredXP,
        };

        string modJson = JsonConvert.SerializeObject(modData, Formatting.Indented);

        var combined = new CombinedSaveWrapper
        {
            UnityGameData = unityJson,
            ModData = modData
        };

        try
        {
            string combinedJson = JsonConvert.SerializeObject(combined, Formatting.Indented);
            File.WriteAllText(getJsonSavePath(), combinedJson);
        }
        catch (Exception ex)
        {
            Plugin.Log("Error saving combined JSON: " + ex);
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

            var combined = JsonConvert.DeserializeObject<CombinedSaveWrapper>(text);
            if (combined == null)
            {
                Plugin.Log("Combined save wrapper is null!");
                return false;
            }

            if (!string.IsNullOrEmpty(combined.UnityGameData))
            {
                CGameData gameData = JsonUtility.FromJson<CGameData>(combined.UnityGameData);
                CSaveLoad.m_SavedGame = gameData;
            }

            if (combined.ModData != null)
            {
                var mod = combined.ModData;
                achievementManager.Load(mod.CardProgress ?? new PlayerCardProgress());
                APinfoMenu.Instance.setCardOpenList(mod.OpenAchievements);
                APinfoMenu.Instance.setCardSellList(mod.SellAchievements);
                APinfoMenu.Instance.setCardGradeList(mod.GradeAchievements);
                aPSaveData.ProcessedIndex = mod.ProcessedIndex;
                aPSaveData.MoneyMultiplier = mod.MoneyMultiplier;
                aPSaveData.Luck = mod.Luck;
                aPSaveData.GhostCardsSold = mod.GhostCardsSold;
                aPSaveData.EventGamesPlayed = mod.EventGamesPlayed;
                aPSaveData.LicensesReceived = mod.LicensesReceived;
                aPSaveData.StoredXP = mod.StoredXP;
            }

            return true;
        }
        catch
        {
            Plugin.Log("Failed to retrieve save data");
        }
        return false;
    }

    public class CombinedSaveWrapper
    {
        public string UnityGameData { get; set; }
        public SaveDataWrapper ModData { get; set; }
    }

    [Serializable]
    public class SaveDataWrapper
    {
        public PlayerCardProgress CardProgress;
        public int ProcessedIndex;
        public float MoneyMultiplier;
        public int Luck;
        public int GhostCardsSold;
        public int EventGamesPlayed;
        public int LicensesReceived;
        public int StoredXP;
        public List<CardLocation> OpenAchievements;
        public List<CardLocation> SellAchievements;
        public List<CardLocation> GradeAchievements;
    }
}
