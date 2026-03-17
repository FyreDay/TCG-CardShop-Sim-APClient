
using ApClient.data;
using ApClient.mapping;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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

    public float CustomerMoneyMult { get; set; }

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

    public APSaveData saveData;
    public AchievementHandler achievementHandler;

    public SaveHandler(string seed, string slot)
    {
        this.seed = seed;
        this.slot = slot;
        saveData = new APSaveData();
    }

    public void HandleNewGame()
    {
        achievementHandler = new AchievementHandler(Plugin.ArchipelagoHandler.slotData.GetAchievementDefinitions(), saveData);
        saveData.foundCards.found = new();
        saveData.foundCards.notfound = new();

        for (int pack = 0; (ECollectionPackType)pack < ECollectionPackType.GhostPack; pack++)
        {
            foreach (ECardExpansionType t in new[] { ECardExpansionType.Tetramon, ECardExpansionType.Destiny })
            {
                for (int monster = 0; (EMonsterType)monster < EMonsterType.MAX; monster++)
                {
                    for (int border = 0; (ECardBorderType)border <= ECardBorderType.FullArt; border++)
                    {
                        foreach (bool foil in new[] { true, false })
                        {
                            saveData.foundCards.notfound.Add(CardMapping.getId(new CardData
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
        }
        saveData.foundCards.notfound.Sort();
    }

    public void AddCard(CardData card, string achievementType)
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
                var mod = combined.ModData;
                saveData.ProcessedIndex = mod.ProcessedIndex;
                saveData.Luck = mod.Luck;
                saveData.GhostCardsSold = mod.GhostCardsSold;
                saveData.StoredXP = mod.StoredXP;
                saveData.achievementSave = mod.achievementSave;
                achievementHandler = new AchievementHandler(Plugin.ArchipelagoHandler.slotData.GetAchievementDefinitions(), saveData);
                saveData.foundCards = mod.foundCards;
                saveData.CustomerMoneyMult = mod.CustomerMoneyMult;
            }

            return true;
        }
        catch
        {
            Plugin.Logger.LogError("Failed to retrieve save data");
        }
        return false;
    }
}
