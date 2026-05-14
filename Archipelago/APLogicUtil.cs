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
            (Plugin.ArchipelagoHandler.GetItemCount(190) > 2 || Plugin.ArchipelagoHandler.GetItemCount(1) > 3) &&
            (Plugin.ArchipelagoHandler.GetItemCount(190) > 4 || Plugin.ArchipelagoHandler.GetItemCount(1) > 5) &&
            (Plugin.ArchipelagoHandler.GetItemCount(190) > 6 || Plugin.ArchipelagoHandler.GetItemCount(1) > 7) &&
            (Plugin.ArchipelagoHandler.GetItemCount(190) > 8 || Plugin.ArchipelagoHandler.GetItemCount(1) > 9) &&
            (Plugin.ArchipelagoHandler.GetItemCount(190) > 10 || Plugin.ArchipelagoHandler.GetItemCount(1) > 11) &&
            (Plugin.ArchipelagoHandler.GetItemCount(190) > 12 || Plugin.ArchipelagoHandler.GetItemCount(1) > 13) &&
            (Plugin.ArchipelagoHandler.GetItemCount(190) > 14 || Plugin.ArchipelagoHandler.GetItemCount(1) > 15));
    }
}
