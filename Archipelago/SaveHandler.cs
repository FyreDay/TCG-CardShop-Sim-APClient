
using ApClient.data;
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
public class APSaveData
{
    public int ProcessedIndex { get; set; }
    public int Luck { get; set; }
    public int GhostCardsSold { get; set; }
    public int StoredXP { get; set; }
    public PlayerAchievementSave achievementSave { get; set; }

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
