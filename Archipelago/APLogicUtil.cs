using ApClient.data;
using ApClient.Patches.Functionality;
using I2.Loc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApClient.Archipelago;

public class APLogicUtil
{
    public static int GetNextLevel()
    {
        return (((CPlayerData.m_ShopLevel + 1) + 4) / 5) * 5;
    }
    public static int GetMaxLevel(int currentLevel)
    {
        int maxLevel = GetNextLevel();

        while (GetRemainingLicenses(maxLevel) <= 0 && maxLevel < Plugin.ArchipelagoHandler.slotData.MaxLevel)
        {
            maxLevel+=5;
        }
        if (maxLevel >= Plugin.ArchipelagoHandler.slotData.MaxLevel && GetRemainingLicenses(Plugin.ArchipelagoHandler.slotData.MaxLevel) <= 0)
        {
            return 1000;
        }
        return maxLevel;
    }
    public static int GetRemainingLicenses(int nextLevel5)
    {
        SlotData slotData = Plugin.ArchipelagoHandler.slotData;

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

        int remaining = requiredCount - Plugin.SaveHandler.GetSaveData().numLicensesOwned;
        return remaining > 0 ? remaining : 0; // return 0 if none remaining
    }

    public static void TriggerDeathlinkLogic()
    {
        PopupTextPatches.ShowCustomText("Deathlink Has Caused Customers to Leave!");
        foreach (Customer c in CSingleton<CustomerManager>.Instance.m_CustomerList)
        {
            if (!(UnityEngine.Random.Range(0, 1f) < Constants.SHOPLIFT_CHANCE))
            {
                continue;
            }
            List<string> list = new List<string>();
            if (c.m_ItemInBagList.Count > 0 || c.m_CardInBagList.Count > 0)
            {
                list.Add(LocalizationManager.GetTranslation("Im Going to Shoplift"));
            }
            else
            {
                list.Add(LocalizationManager.GetTranslation("Im leaving"));
            }
            c.PopupText(list, 100);
            c.m_CustomerTournamentData.m_IsTournamentCustomer = false;
            c.m_ItemInBagList.Clear();
            c.m_CardInBagList.Clear();
            c.ExitShop();

        }
    }

    public static bool hasAllCardPacks()
    {
        return ((Plugin.ArchipelagoHandler.GetItemCount(190) > 0 || Plugin.ArchipelagoHandler.GetItemCount(1) > 0) &&
            (Plugin.ArchipelagoHandler.GetItemCount(2) > 0 || Plugin.ArchipelagoHandler.GetItemCount(3) > 0) &&
            (Plugin.ArchipelagoHandler.GetItemCount(4) > 0 || Plugin.ArchipelagoHandler.GetItemCount(5) > 0) &&
            (Plugin.ArchipelagoHandler.GetItemCount(6) > 0 || Plugin.ArchipelagoHandler.GetItemCount(7) > 0) &&
            (Plugin.ArchipelagoHandler.GetItemCount(8) > 0 || Plugin.ArchipelagoHandler.GetItemCount(9) > 0) &&
            (Plugin.ArchipelagoHandler.GetItemCount(10) > 0 || Plugin.ArchipelagoHandler.GetItemCount(11) > 0) &&
            (Plugin.ArchipelagoHandler.GetItemCount(12) > 0 || Plugin.ArchipelagoHandler.GetItemCount(13) > 0) &&
            (Plugin.ArchipelagoHandler.GetItemCount(14) > 0 || Plugin.ArchipelagoHandler.GetItemCount(15) > 0));
    }

    public static List<ECollectionPackType> GetAllAvailablePacks()
    {
        List<ECollectionPackType> packs = new List<ECollectionPackType>();
        if (Plugin.ArchipelagoHandler.GetItemCount(190) > 0)
        {
            packs.Add(ECollectionPackType.BasicCardPack);
        }
        for (int i = 1; i < 15; i+=2)
        {
            if (Plugin.ArchipelagoHandler.GetItemCount(i) > 0 || Plugin.ArchipelagoHandler.GetItemCount(i+1) > 0)
            {
                packs.Add((ECollectionPackType)((i + 1) / 2));
            }
        }
        return packs;
    }

    public static List<EItemType> GetAllAvailableItems()
    {
        List<EItemType> items = new List<EItemType>();
        if (Plugin.ArchipelagoHandler.GetItemCount(190) > 0)
        {
            items.Add(EItemType.BasicCardPack);
        }
        for (int i = 1; i < (int)EItemType.Max; i++)
        {
            if(Plugin.ArchipelagoHandler.GetItemCount(i) > 0)
            {
                items.Add((EItemType)i);
            }
        }

        return items;
    }
}
