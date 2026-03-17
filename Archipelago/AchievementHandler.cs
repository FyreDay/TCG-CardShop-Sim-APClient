using ApClient.ui;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static ApClient.data.CardDataExtensions;

namespace ApClient.Archipelago;


[Serializable]
public class AchievementData
{
    public long id;
    public string type;
    public string name;
    public int difficulty;
    public int threshold;

    public int[] rarity;
    public int[] border;
    public int[] expansion;
    public int[] foil;
    public int[] grade;
}

public class CompiledAchievement
{
    public AchievementData data;

    public long id;
    public ulong rarityMask;
    public ulong borderMask;
    public ulong expansionMask;
    public ulong foilMask;
    public ulong gradeMask;

    public int progress;
    public bool completed;

    public bool IsHinted;
    public bool Available;

    static ulong BuildMask(int[] values)
    {
        if (values == null || values.Length == 0)
            return ulong.MaxValue; // means ANY

        ulong mask = 0;

        foreach (var v in values)
            mask |= 1UL << v;

        return mask;
    }
    public CompiledAchievement(AchievementData data)
    {
        this.data = data;
        id = data.id;
        rarityMask = BuildMask(data.rarity);
        borderMask = BuildMask(data.border);
        expansionMask = BuildMask(data.expansion);
        foilMask = BuildMask(data.foil);
        gradeMask = BuildMask(data.grade);
    }
}

[Serializable]
public class AchievementProgress
{
    public int Progress;
    public bool Completed;
}

[Serializable]
public class PlayerAchievementSave
{
    public Dictionary<string, Dictionary<long, AchievementProgress>> Achievements = new();
}

public class AchievementHandler
{
    private Dictionary<long, CompiledAchievement> achievementById = new();
    public Dictionary<string, List<CompiledAchievement>> achievementsByType;
    APSaveData saveDataRef;
    public AchievementHandler(Dictionary<string, List<AchievementData>> defs, APSaveData save)
    {
        achievementsByType = new Dictionary<string, List<CompiledAchievement>>(defs.Count);
        saveDataRef = save;

        if(saveDataRef.achievementSave == null)
        {
            saveDataRef.achievementSave = new PlayerAchievementSave();
        }

        foreach (string typekey in defs.Keys)
        {
            achievementsByType[typekey] = new List<CompiledAchievement>(defs[typekey].Count);
            foreach (var ach in defs[typekey])
            {
                var compiled = new CompiledAchievement(ach);
                //add to id dictionary
                achievementById[compiled.id] = compiled;
                Plugin.Logger.LogInfo(compiled.id);
                //make sure the dict exists
                if (!saveDataRef.achievementSave.Achievements.TryGetValue(typekey, out var saveDict))
                {
                    saveDict = new Dictionary<long, AchievementProgress>();
                    saveDataRef.achievementSave.Achievements[typekey] = saveDict;
                }
                //get progress from dict or create it
                if (saveDataRef.achievementSave.Achievements[typekey].TryGetValue(ach.id, out var progress))
                {
                    compiled.progress = progress.Progress;
                    compiled.completed = progress.Completed;
                }
                else
                {
                    saveDataRef.achievementSave.Achievements[typekey][ach.id] = new AchievementProgress();
                }

                achievementsByType[typekey].Add(compiled);
            }
        }
       

        
    }
    //doing bitwise checks to make this fast (and because I find it cool)
    private bool Matches(CompiledAchievement a, CardMask c)
    {
        if ((a.rarityMask & c.rarity) == 0) return false;
        if ((a.borderMask & c.border) == 0) return false;
        if ((a.expansionMask & c.expansion) == 0) return false;
        if ((a.foilMask & c.foil) == 0) return false;
        if ((a.gradeMask & c.grade) == 0) return false;

        return true;
    }

    public void SetHinted(long achievementId)
    {
        if (achievementById.TryGetValue(achievementId, out var ach))
        {
            ach.IsHinted = true;
        }
    }

    public void UpdateAvailability(List<int> unlockedRarities, List<int> unlockedExpansions)
    {
        ulong rarityMask = 0;
        ulong expansionMask = 0;

        foreach (var r in unlockedRarities)
            rarityMask |= 1UL << r;

        foreach (var e in unlockedExpansions)
            expansionMask |= 1UL << e;
        //loop through all 3 achievement type dictionaries. if special handling, split into 3
        foreach (var list in achievementsByType.Values)
        {
            foreach (var ach in list)
            {
                bool rarityOK = (ach.rarityMask & rarityMask) != 0;
                bool expansionOK = (ach.expansionMask & expansionMask) != 0;

                ach.Available = rarityOK && expansionOK;
            }
        }
    }

    public List<long> OnCard(CardData card, string achievementType)
    {
        var unlocked = new List<long>();

        var mask = card.GetCardMask();

        foreach (var a in achievementsByType[achievementType])
        {
            if (a.completed)
                continue;

            if (!Matches(a, mask))
                continue;

            a.progress++;

            var saveEntry = saveDataRef.achievementSave.Achievements[achievementType][a.id];
            saveEntry.Progress = a.progress;
            if (a.progress >= a.data.threshold)
            {
                a.completed = true;
                saveEntry.Completed = true;
                unlocked.Add(a.id);
            }
        }

        return unlocked;
    }


}