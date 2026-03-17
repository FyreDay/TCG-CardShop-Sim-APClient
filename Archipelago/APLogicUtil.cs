using ApClient.data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApClient.Archipelago;

public class APLogicUtil
{
    public static int GetMaxLevel(int currentLevel)
    {
        int maxLevel = (((CPlayerData.m_ShopLevel + 1) + 4) / 5) * 5;

        while(GetRemainingLicenses(maxLevel) <= 0 || maxLevel > Plugin.ArchipelagoHandler.slotData.MaxLevel)
        {
            maxLevel+=5;
        }
        return maxLevel;
    }
    public static int GetRemainingLicenses(int nextLevel5)
    {
        
        SlotData slotData = Plugin.ArchipelagoHandler.slotData;
        var allLicenses = new Dictionary<EItemType, int>();
        foreach (DictionaryEntry entry in slotData.pg1IndexMapping)
        {
            EItemType itemId = (EItemType)entry.Key;
            int level = (int)entry.Value;

            if (level < nextLevel5 && !allLicenses.ContainsKey(itemId))
            {
                allLicenses[itemId] = level;
            }
        }
        foreach (DictionaryEntry entry in slotData.pg2IndexMapping)
        {
            EItemType itemId = (EItemType)entry.Key;
            int level = (int)entry.Value;

            if (level < nextLevel5 && !allLicenses.ContainsKey(itemId))
            {
                allLicenses[itemId] = level;
            }
        }
        foreach (DictionaryEntry entry in slotData.pg3IndexMapping)
        {
            EItemType itemId = (EItemType)entry.Key;
            int level = (int)entry.Value;

            if (level < nextLevel5 && !allLicenses.ContainsKey(itemId))
            {
                allLicenses[itemId] = level;
            }
        }
        foreach (DictionaryEntry entry in slotData.ttIndexMapping)
        {
            EItemType itemId = (EItemType)entry.Key;
            int level = (int)entry.Value;

            if (level < nextLevel5 && !allLicenses.ContainsKey(itemId))
            {
                allLicenses[itemId] = level;
            }
        }

        if (allLicenses.Count == 0)
            return 0; // no requirements, so zero remaining

        int required = slotData.RequiredLicenses;
        int sect_1 = nextLevel5;
        int sect_2 = 0;
        int sect_3 = 0;
        if (nextLevel5 > 25)
        {
            sect_1 = 25;
            sect_2 = nextLevel5 - 25;
        }

        if (nextLevel5 > 50)
        {
            sect_2 = 25;
            sect_3 = nextLevel5 - 50;
        }
        // Calculate how many licenses are required at this level
        int requiredCount = (sect_1 / 5) * slotData.RequiredLicenses;
        requiredCount += (sect_2 / 5) * 3;
        requiredCount += (sect_3 / 5) * 2;

        // Count how many licenses the player currently owns
        int ownedCount = allLicenses.Keys.Count(itemId => Plugin.ArchipelagoHandler.itemCount((long)itemId) > 0);

        int remaining = requiredCount - ownedCount;
        return remaining > 0 ? remaining : 0; // return 0 if none remaining
    }
}
