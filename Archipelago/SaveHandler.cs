
using ApClient.data;
using ApClient.mapping;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;
using UnityEngine;

namespace ApClient.Archipelago;

public class CombinedSaveWrapper
{
    public string UnityGameData { get; set; }
    public APSaveData ModData { get; set; }
}


[Serializable]
public class FoundCards
{
    public Dictionary<ECollectionPackType, List<int>> foundByPackType = new();
    public List<int> notfound = new();
    public Dictionary<ECollectionPackType, int> sanityCount = new();
}

[Serializable]
public class APSaveData
{
    public PlayerAchievementSave achievementSave { get; set; }
    public FoundCards foundCards { get; set; }
    public int ProcessedIndex { get; set; }
    public int Luck { get; set; }
    public int GhostCardsSold { get; set; }
    public int StoredXP { get; set; }
    public int numLicensesOwned { get; set; }
    public float CustomerMoneyMult { get; set; }
    public Dictionary<EGameEventFormat, int> PlayedGames = new();

    public APSaveData() {
        ProcessedIndex = 0;
        Luck = 0;
        StoredXP = 0;
        foundCards = new FoundCards();
        GhostCardsSold = 0;
        numLicensesOwned = 0;
        CustomerMoneyMult = 0;
    }
}
public class SaveHandler
{
    private string seed;
    private string slot;

    private APSaveData saveData;
    private AchievementHandler achievementHandler;

    public SaveHandler(string seed, string slot)
    {
        this.seed = seed;
        this.slot = slot;
    }
    
    public APSaveData GetSaveData()
    {
        if(saveData == null)
        {
            Plugin.Logger.LogError("AP SAVE DATA IS NULL");
            return null;
        }
        if (achievementHandler == null)
        {
            achievementHandler = new AchievementHandler(Plugin.ArchipelagoHandler.slotData.GetAchievementDefinitions(), saveData);
        }
        return saveData;
    }

    public AchievementHandler GetAchievementHandler()
    {
        if (achievementHandler == null)
        {
            achievementHandler = new AchievementHandler(Plugin.ArchipelagoHandler.slotData.GetAchievementDefinitions(), saveData);
        }
        return achievementHandler;
    }

    public void HandleNewGame()
    {
        var newSave = new APSaveData();
        

        for (int packType = 0; packType < (int)ECollectionPackType.MAX; packType++)
        {
            newSave.foundCards.foundByPackType.Add((ECollectionPackType)packType, new List<int>());
        }

        for (int packType = 0; packType < (int)ECollectionPackType.GhostPack; packType++)
        {
            newSave.foundCards.sanityCount.Add((ECollectionPackType)packType, 0);
        }

        newSave.foundCards.notfound = new();

        for (int format = 0; format <= (int)EGameEventFormat.MAX; format++)
        {
            newSave.PlayedGames.Add((EGameEventFormat)format, 0);
        }



        foreach (ECardExpansionType t in new[] { ECardExpansionType.Tetramon, ECardExpansionType.Destiny })
        {
            for (int monster = 0; (EMonsterType)monster < EMonsterType.MAX; monster++)
            {
                for (int border = 0; (ECardBorderType)border <= ECardBorderType.FullArt; border++)
                {
                    foreach (bool foil in new[] { true, false })
                    {
                        newSave.foundCards.notfound.Add(CardMapping.getId(new CardData
                        {
                            isFoil = foil,
                            isDestiny = t == ECardExpansionType.Destiny,
                            borderType = (ECardBorderType)border,
                            monsterType = (EMonsterType)monster,
                            expansionType = t,
                            isChampionCard = false,
                            isNew = true
                        }));
                    }
                }
            }
        }

        newSave.foundCards.notfound.Sort();

        saveData = newSave;
        achievementHandler = new AchievementHandler(Plugin.ArchipelagoHandler.slotData.GetAchievementDefinitions(), saveData);
    }

    private bool trackFoils(bool foil, int cardSanity)
    {
        if(cardSanity == 0)
        {
            return false;
        }

        if (cardSanity == 1)
        {
            return !foil;
        }

        return true;
    }

    private void UpdateCardCount(CardData card)
    {
        saveData.foundCards.sanityCount[card.GetPackType()] += 1;
        UIInfoPanel.getInstance().UpdateCardCollection(card.GetPackType(), saveData.foundCards.sanityCount[card.GetPackType()]);
        Plugin.ArchipelagoHandler.CompleteLocationChecks(CardMapping.getId(card));
    }

    public void AddCard(CardData card, string achievementType)
    {
        Plugin.Logger.LogInfo($"Added new card : {card.monsterType} : {achievementType}");
        foreach (var pair in saveData.foundCards.foundByPackType)
        {
            Plugin.Logger.LogInfo($"packtype: {pair.Key} : {pair.Value.Count} cards found");
        }
        if (achievementType == Constants.OPEN_ACHIEVEMENT_TYPE
            && saveData.foundCards.foundByPackType.TryGetValue(card.GetPackType(), out List<int> found))
        {
            int id = CardMapping.getId(card);
            if (saveData.foundCards.notfound.Remove(id))
            {
                int index = found.BinarySearch(id);
                if (index < 0)
                {
                    found.Insert(~index, id);

                    int foil = Plugin.ArchipelagoHandler.slotData.CardSanity;
                    switch (Plugin.ArchipelagoHandler.slotData.CardOpeningCheckDifficulty)
                    {
                        case 0: UIInfoPanel.getInstance().UpdateCardCollection(card.GetPackType(), found.Count); break;
                        case 1:
                            if ((int)card.borderType <= 1 && trackFoils(card.isFoil, foil))
                            {
                                UpdateCardCount(card);
                            }
                            break;
                        case 2:
                            if ((int)card.borderType <= 3 && trackFoils(card.isFoil, foil))
                            {
                                UpdateCardCount(card);
                            }
                            break;
                        case 3:
                            if ((int)card.borderType <= 4 && trackFoils(card.isFoil, foil))
                            {
                                UpdateCardCount(card);
                            }
                            break;

                        case 4:
                            if (trackFoils(card.isFoil, foil))
                            {
                                UpdateCardCount(card);
                            }
                            break;
                    }

                }
            }
            if (Plugin.ArchipelagoHandler.slotData.Goal == 1)
            {
                decimal countCollected = 0;
                foreach (var pair in saveData.foundCards.foundByPackType)
                {
                    countCollected += pair.Value.Count;
                }
                decimal percentCollected = countCollected / (decimal)(countCollected + saveData.foundCards.notfound.Count);
                UIInfoPanel.getInstance().setPercentGoalCollected(percentCollected * 100);
                if (percentCollected >= Plugin.ArchipelagoHandler.slotData.CollectionGoalPercent / (decimal)100
                    && APLogicUtil.hasAllCardPacks())
                {
                    Plugin.ArchipelagoHandler.Release();
                }
            }

            //TODO: Update AP UI with Each Pack Count
        }
        Plugin.Logger.LogInfo($"Completed card add");
        Plugin.ArchipelagoHandler.CompleteLocationChecks(achievementHandler.OnCard(card, achievementType));

        UIInfoPanel.getInstance().Refresh();

    }

    public void checkWithServer(HashSet<long> checkedLocations)
    {
        achievementHandler.CheckWithHashSet(checkedLocations);
        Save(Constants.SAVE_SLOT);
    }

    public void AddGhostSold()
    {
        saveData.GhostCardsSold++;
        UIInfoPanel.getInstance().setGhostGoalSold(saveData.GhostCardsSold);
    }

    public CardData NewRandomCard()
    {
        if (Plugin.SaveHandler.saveData.foundCards.notfound.Count < 0)
        {
            var cardData = new CardData();

            cardData.expansionType = (ECardExpansionType)(1);
            cardData.borderType = (ECardBorderType)(1);
            cardData.isFoil = true;
            cardData.monsterType = (EMonsterType)(1);
            cardData.cardGrade = 0;
            return cardData;
        }
        int id = Plugin.SaveHandler.saveData.foundCards.notfound[UnityEngine.Random.RandomRangeInt(0, Plugin.SaveHandler.saveData.foundCards.notfound.Count)];
        CardData data = CardMapping.getCardFromId(id);
        if(data == null)
        {
            Plugin.Logger.LogError("New Card was NULL. trying again");
            return NewRandomCard();
        }
        data.isNew = true;
        return data;
    }


    public string GetBaseDirectory()
    {
        return Application.persistentDataPath;
    }
    private string getGdSavePath()
    {
        return $"{this.GetBaseDirectory()}/APSaves/{MyPluginInfo.PLUGIN_GUID}_{slot}_{seed}.gd";
    }

    private string getJsonSavePath()
    {
        return $"{this.GetBaseDirectory()}/APSaves/{MyPluginInfo.PLUGIN_GUID}_{slot}_{seed}.json";
    }
    public bool doesSaveExist() { return File.Exists(getJsonSavePath()) || File.Exists(getGdSavePath()); }
    public void Save(int saveSlotIndex)
    {
        System.IO.Directory.CreateDirectory($"{this.GetBaseDirectory()}/APSaves/");
        CSaveLoad.m_SavedGame = CGameData.instance;
        string finalPath = getJsonSavePath();
        string tempPath = finalPath + ".tmp";

        try
        {
            string unityJson = JsonUtility.ToJson(CGameData.instance, true);

            var combined = new CombinedSaveWrapper
            {
                UnityGameData = unityJson,
                ModData = saveData
            };

            string combinedJson = JsonConvert.SerializeObject(combined, Formatting.Indented);
            File.WriteAllText(tempPath, combinedJson);

            FileInfo fi = new FileInfo(tempPath);
            using (var fs = fi.Open(FileMode.Open))
            {
                fs.Flush(true);
            }
            if (File.Exists(finalPath))
                File.Replace(tempPath, finalPath, null);
            else
                File.Move(tempPath, finalPath);
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError("Save failed: " + ex);

            // cleanup bad temp file
            if (File.Exists(tempPath))
                File.Delete(tempPath);
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
            Plugin.Logger.LogInfo("read");
            var combined = JsonConvert.DeserializeObject<CombinedSaveWrapper>(text);
            Plugin.Logger.LogInfo("deserialize");
            if (combined == null)
            {
                Plugin.Logger.LogError("Combined save wrapper is null!");
                return false;
            }

            

            if (combined.ModData != null)
            {
                Plugin.Logger.LogInfo("moddata");
                saveData = combined.ModData;
                Plugin.Logger.LogInfo("get moddata");
                achievementHandler = new AchievementHandler(Plugin.ArchipelagoHandler.slotData.GetAchievementDefinitions(), saveData);
                Plugin.Logger.LogInfo("after achievment");
                
                Plugin.Logger.LogInfo("moddata done");
            }
            Plugin.Logger.LogInfo("Completed AP Save Load");
            if (!string.IsNullOrEmpty(combined.UnityGameData))
            {
                Plugin.Logger.LogInfo("gamedata");
                CGameData gameData = JsonUtility.FromJson<CGameData>(combined.UnityGameData);
                CSaveLoad.m_SavedGame = gameData;
                Plugin.Logger.LogInfo("gamedata done");
            }
            return true;
        }
        catch
        {
            Plugin.Logger.LogError("Failed to retrieve save data");
        }
        return true;
    }

    public void UpdateSanityUI()
    {
        for (int packtype = 0; packtype < (int)ECollectionPackType.GhostPack; packtype++)
        {
            UIInfoPanel.getInstance().UpdateCardCollection((ECollectionPackType)packtype, 0);// saveData.foundCards.sanityCount[(ECollectionPackType)packtype]);
        }
    }
}
