using ApClient.Archipelago;
using ApClient.mapping;
using ApClient.patches;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine.Rendering;

namespace ApClient.Patches.Functionality;

public class PlayerDataPatches
{
    [HarmonyPatch(typeof(CPlayerData), "CPlayer_OnAddShopExp")]
    class AddXp
    {
        private static int oldLevel;
        [HarmonyPrefix]
        static bool Prefix(CEventPlayer_AddShopExp evt)
        {
            int maxLevel = APLogicUtil.GetMaxLevel(CPlayerData.m_ShopLevel);
            oldLevel = CPlayerData.m_ShopLevel;

            if (oldLevel + 2 >= maxLevel)
            {
                if (CPlayerData.m_ShopExpPoint + evt.m_ExpValue >= CPlayerData.GetExpRequiredToLevelUp())
                {
                    int xptonext = CPlayerData.GetExpRequiredToLevelUp() - CPlayerData.m_ShopExpPoint;
                    Plugin.SaveHandler.saveData.StoredXP+=evt.m_ExpValue;
                    CEventManager.QueueEvent(new CEventPlayer_AddShopExp(xptonext - 1));
                    return false;
                }
                return true;

            }

            if (Plugin.SaveHandler.saveData.StoredXP > 50)
            {
                int xp = Plugin.SaveHandler.saveData.StoredXP;
                Plugin.SaveHandler.saveData.StoredXP = 0;
                CEventManager.QueueEvent(new CEventPlayer_AddShopExp(xp));
            }
            return true;
        }

        [HarmonyPostfix]
        static void Postfix(CEventPlayer_AddShopExp evt)
        {

            if (oldLevel < CPlayerData.m_ShopLevel && CPlayerData.m_ShopLevel + 1 >= 2)
            {
                //Plugin.Log($"Level Up: {oldLevel+1} -> {CPlayerData.m_ShopLevel+1}");
                Plugin.ArchipelagoHandler.CompleteLocationChecks(LevelMapping.startValue + CPlayerData.m_ShopLevel);
                if (Plugin.ArchipelagoHandler.slotData.Goal == 0 && CPlayerData.m_ShopLevel + 1 >= Plugin.ArchipelagoHandler.slotData.MaxLevel)
                {
                    Plugin.ArchipelagoHandler.Release();
                    PopupTextPatches.ShowCustomText("Congrats! Your Shop Has Leveled To Your Goal!");
                }
            }
        }
    }


    [HarmonyPatch(typeof(CPlayerData), "AddCard")]
    class AddCard
    {
        [HarmonyPrefix]
        static void Prefix(CardData cardData, int addAmount)
        {
            if (cardData.cardGrade > 0)
            {
                Plugin.SaveHandler.AddCard(cardData, Constants.GRADE_ACHIEVEMENT_TYPE);
                return;
            }
            Plugin.SaveHandler.AddCard(cardData, Constants.OPEN_ACHIEVEMENT_TYPE);
        }
    }

    [HarmonyPatch(typeof(CPlayerData), "SetUnlockItemLicense")]
    class UnlockLicense
    {
        [HarmonyPrefix]
        static void Prefix(int index)
        {
            if (!CPlayerData.GetIsItemLicenseUnlocked(index) && Plugin.IsGameReady())
            {
                Plugin.SaveHandler.saveData.numLicensesOwned++;
                UIInfoPanel.getInstance().SetLicensesToLevel(APLogicUtil.GetRemainingLicenses(APLogicUtil.GetMaxLevel(CPlayerData.m_ShopLevel)));
                UIInfoPanel.getInstance().SetLevelMax(APLogicUtil.GetMaxLevel(CPlayerData.m_ShopLevel));
            }
        }

        [HarmonyPostfix]
        static void Postfix(int index)
        {
            //TODO: THIS DOES NOT FUNCTION
            List<ECollectionPackType> ownedPacks = new List<ECollectionPackType>();

            for (int i = 0; i < CPlayerData.m_IsItemLicenseUnlocked.Count; i++)
            {
                if (!CPlayerData.m_IsItemLicenseUnlocked[i])
                    continue;

                ECollectionPackType packType = InventoryBase.ItemTypeToCollectionPackType(InventoryBase.GetRestockData(i).itemType);

                if (packType != ECollectionPackType.None)
                {
                    ownedPacks.Add(packType);
                }
            }

            Plugin.SaveHandler.achievementHandler.UpdateAvailability(ownedPacks);
        }
    }

    [HarmonyPatch(typeof(CPlayerData), "CreateDefaultData")]
    class CreateDefault
    {
        [HarmonyPostfix]
        static void CreateDefaultData(bool isRebornReset = false)
        {
            //remove card packs
            CPlayerData.m_IsItemLicenseUnlocked[0] = false;
        }
    }

}
