using ApClient.Archipelago.Mapping;
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
    public ulong cardPackMask;
    public ulong rarityMask;
    public ulong borderMask;
    public ulong expansion;
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

    static ulong BuildCardPackMask(int[] rarities, int[] expansions)
    {
        ulong itemMask = 0;

        foreach (var r in rarities)
        {
            foreach (var e in expansions)
            {
                int index = (r - 1) + (4 * e);
                itemMask |= 1UL << index;
            }
        }
        return itemMask;
    }
    public CompiledAchievement(AchievementData data)
    {
        this.data = data;
        id = data.id;
        rarityMask = BuildMask(data.rarity);
        borderMask = BuildMask(data.border);
        expansion = BuildMask(data.expansion);
        foilMask = BuildMask(data.foil);
        gradeMask = BuildMask(data.grade);
        cardPackMask = BuildCardPackMask(data.rarity, data.expansion);
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
                
                //make sure the dict exists
                if (!saveDataRef.achievementSave.Achievements.TryGetValue(typekey, out Dictionary<long, AchievementProgress> saveDict))
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
                    saveDataRef.achievementSave.Achievements[typekey].Add(ach.id, new AchievementProgress());
                }

                achievementsByType[typekey].Add(compiled);
            }
        }

    }
   
    public void SetHinted(long achievementId)
    {
        if (achievementById.TryGetValue(achievementId, out var ach))
        {
            ach.IsHinted = true;
        }
    }

    public void UpdateAvailability(List<ECollectionPackType> packTypes)
    {
        ulong selectedMask = 0;

        foreach (var p in packTypes)
        {
            Plugin.Logger.LogInfo($"Updating achievement availability with pack: {p.ToString()}");
            selectedMask |= 1UL << (int)p;
        }
        bool hascardtable = Plugin.ArchipelagoHandler.GetItemCount(FurnatureMapping.getIdFromType(EObjectType.CardShelf)) > 0;
        Plugin.Logger.LogInfo($"has card table {hascardtable}");
        foreach (var kvp in achievementsByType)
        {
            string achievementType = kvp.Key;
            List<CompiledAchievement> list = kvp.Value;

            foreach (var ach in list)
            {
                if (ach.Available)
                {
                    continue;
                }
                bool matches = (ach.cardPackMask & selectedMask) != 0;
                
                if (matches && achievementType == Constants.SELL_ACHIEVEMENT_TYPE)
                {
                    matches = hascardtable;
                    
                }
                if (matches) {
                    Plugin.Logger.LogInfo($"Set Available {ach.data.name}");
                }
                ach.Available = matches;
            }
        }
    }

    //doing bitwise checks to make this fast (and because I find it cool)
    private bool Matches(CompiledAchievement a, CardMask c, string achievementType)
    {
        if ((a.borderMask & c.border) == 0) return false;
        if ((a.cardPackMask & c.packType) == 0) return false;
        if ((a.foilMask & c.foil) == 0) return false;
        if (achievementType == Constants.GRADE_ACHIEVEMENT_TYPE && (a.gradeMask & c.grade) == 0) return false;

        return true;
    }

    public long[] OnCard(CardData card, string achievementType)
    {
        var unlocked = new List<long>();

        var mask = card.GetCardMask();

        foreach (var a in achievementsByType[achievementType])
        {
            if (a.completed)
                continue;
            
            if (!Matches(a, mask, achievementType))
                continue;
            Plugin.Logger.LogInfo($"Match! : {card.monsterType} : {a.data.name}");
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

        return unlocked.ToArray();
    }

    public void CheckWithHashSet(HashSet<long> checkedLocations)
    {
        foreach (var a in achievementById)
        {
            a.Value.completed = checkedLocations.Contains(a.Key);
        }
    }
}