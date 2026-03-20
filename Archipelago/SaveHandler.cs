
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
    public List<int> found  = new();
    public List<int> notfound = new();
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
        newSave.foundCards = new FoundCards();

        newSave.foundCards.found = new();
        newSave.foundCards.notfound = new();

        for (int format = 0; format < (int)EGameEventFormat.MAX; format++)
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

    public void AddCard(CardData card, string achievementType)
    {
        Plugin.Logger.LogInfo($"Added new card : {card.monsterType} : {achievementType}");
        if (achievementType == Constants.OPEN_ACHIEVEMENT_TYPE)
        {
            int id = CardMapping.getId(card);
            if (saveData.foundCards.notfound.Remove(id))
            {
                int index = saveData.foundCards.found.BinarySearch(id);
                if (index < 0)
                {
                    saveData.foundCards.found.Insert(~index, id);
                }
            }
            if (Plugin.ArchipelagoHandler.slotData.Goal == 1)
            {
                decimal percentCollected = (decimal)saveData.foundCards.found.Count / (decimal)(saveData.foundCards.found.Count + saveData.foundCards.notfound.Count);
                if (percentCollected >= Plugin.ArchipelagoHandler.slotData.CollectionGoalPercent / (decimal)100)
                {
                    Plugin.ArchipelagoHandler.Release();
                }
            }
        }
        achievementHandler.OnCard(card, achievementType);

    }

    public CardData NewRandomCard()
    {
        int id = Plugin.SaveHandler.saveData.foundCards.notfound[UnityEngine.Random.RandomRangeInt(0, Plugin.SaveHandler.saveData.foundCards.notfound.Count)];
        CardData data = CardMapping.getCardFromId(id);
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

        string unityJson = JsonUtility.ToJson(CGameData.instance, true);

        var combined = new CombinedSaveWrapper
        {
            UnityGameData = unityJson,
            ModData = saveData
        };

        try
        {
            string combinedJson = JsonConvert.SerializeObject(combined, Formatting.Indented);
            File.WriteAllText(getJsonSavePath(), combinedJson);
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError("Error saving combined JSON: " + ex);
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
                Plugin.Logger.LogError("Combined save wrapper is null!");
                return false;
            }

            if (!string.IsNullOrEmpty(combined.UnityGameData))
            {
                CGameData gameData = JsonUtility.FromJson<CGameData>(combined.UnityGameData);
                CSaveLoad.m_SavedGame = gameData;
            }

            if (combined.ModData != null)
            {
                saveData = combined.ModData;
                achievementHandler = new AchievementHandler(Plugin.ArchipelagoHandler.slotData.GetAchievementDefinitions(), saveData);
            }
            Plugin.Logger.LogInfo("Completed AP Save Load");
            return true;
        }
        catch
        {
            Plugin.Logger.LogError("Failed to retrieve save data");
        }
        return false;
    }
}
