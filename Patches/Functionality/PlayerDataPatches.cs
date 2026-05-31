using ApClient.Archipelago;
using ApClient.mapping;
using HarmonyLib;
using System.Collections.Generic;

namespace ApClient.Patches.Functionality;

public class PlayerDataPatches
{
    [HarmonyPatch(typeof(CPlayerData), "CPlayer_OnAddShopExp")]
    class AddXp
    {
        private static List<RestockData> GetImportantData()
        {
            List<RestockData> importantRestock = new List<RestockData>();
            foreach (EItemType type in CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownItemType)
            {
                var data = InventoryBase.GetRestockDataIndexList(type);
                if (InventoryBase.GetRestockData(data[0]).licenseShopLevelRequired <= CPlayerData.m_ShopLevel + 1
                    && !CPlayerData.GetIsItemLicenseUnlocked(data[0]))
                {
                    importantRestock.Add(InventoryBase.GetRestockData(data[0]));
                }
            }
            foreach (EItemType type in CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownAccessoryItemType)
            {
                var data = InventoryBase.GetRestockDataIndexList(type);
                if (InventoryBase.GetRestockData(data[0]).licenseShopLevelRequired <= CPlayerData.m_ShopLevel + 1
                    && !CPlayerData.GetIsItemLicenseUnlocked(data[0]))
                {
                    importantRestock.Add(InventoryBase.GetRestockData(data[0]));
                }
            }
            foreach (EItemType type in CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownFigurineItemType)
            {
                var data = InventoryBase.GetRestockDataIndexList(type);
                if (InventoryBase.GetRestockData(data[0]).licenseShopLevelRequired <= CPlayerData.m_ShopLevel + 1
                    && !CPlayerData.GetIsItemLicenseUnlocked(data[0]))
                {
                    importantRestock.Add(InventoryBase.GetRestockData(data[0]));
                }
            }
            foreach (EItemType type in CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownBoardGameItemType)
            {
                var data = InventoryBase.GetRestockDataIndexList(type);
                if (InventoryBase.GetRestockData(data[0]).licenseShopLevelRequired <= CPlayerData.m_ShopLevel + 1
                    && !CPlayerData.GetIsItemLicenseUnlocked(data[0]))
                {
                    importantRestock.Add(InventoryBase.GetRestockData(data[0]));
                }
            }
            return importantRestock;
        }

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
                    Plugin.SaveHandler.GetSaveData().StoredXP+=evt.m_ExpValue;
                    CEventManager.QueueEvent(new CEventPlayer_AddShopExp(xptonext - 1));
                    return false;
                }
                return true;

            }

            if (Plugin.SaveHandler.GetSaveData().StoredXP > CPlayerData.GetExpRequiredToLevelUp())
            {
                Plugin.SaveHandler.GetSaveData().StoredXP -= CPlayerData.GetExpRequiredToLevelUp();
                
                CEventManager.QueueEvent(new CEventPlayer_AddShopExp(CPlayerData.GetExpRequiredToLevelUp()));
            }else if (Plugin.SaveHandler.GetSaveData().StoredXP > 0)
            {
                int xp = Plugin.SaveHandler.GetSaveData().StoredXP;
                Plugin.SaveHandler.GetSaveData().StoredXP = 0;
                CEventManager.QueueEvent(new CEventPlayer_AddShopExp(xp));
            }
            return true;
        }

        [HarmonyPostfix]
        static void Postfix(CEventPlayer_AddShopExp evt)
        {
            if (oldLevel < CPlayerData.m_ShopLevel && CPlayerData.m_ShopLevel + 1 >= 2 && Plugin.IsGameReady())
            {
                //UIInfoPanel.getInstance().UpdateImportantLicenses(GetImportantData());

                Plugin.ArchipelagoHandler.CompleteLocationChecks(LevelMapping.startValue + CPlayerData.m_ShopLevel);
                if (Plugin.ArchipelagoHandler.slotData.Goal == 0 && CPlayerData.m_ShopLevel + 1 >= Plugin.ArchipelagoHandler.slotData.MaxLevel)
                {
                    Plugin.ArchipelagoHandler.Release();
                    PopupTextPatches.ShowCustomText("Congrats! Your Shop Has Leveled To Your Goal!");
                }
            }
        }
    }

    public static bool stopCountingCard = false;
    [HarmonyPatch(typeof(CPlayerData), "AddCard")]
    class AddCard
    {
        [HarmonyPrefix]
        static void Prefix(CardData cardData, int addAmount)
        {
            if (stopCountingCard)
            {
                return;
            }
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
            if (!Plugin.IsGameReady()) return;

            var datas = InventoryBase.GetRestockDataUsingItemType(InventoryBase.GetRestockData(index).itemType);
            bool hasItem = false;
            foreach (var data in datas)
            {
                if(CPlayerData.GetIsItemLicenseUnlocked(datas[1].index))
                {
                    hasItem = true;
                }
            }
            if (hasItem) { return; }

            Plugin.SaveHandler.GetSaveData().numLicensesOwned++;
            UIInfoPanel.getInstance().SetLicensesToLevel(APLogicUtil.GetRemainingLicenses(APLogicUtil.GetMaxLevel(CPlayerData.m_ShopLevel)));
            UIInfoPanel.getInstance().SetLevelMax(APLogicUtil.GetMaxLevel(CPlayerData.m_ShopLevel));
        }

        [HarmonyPostfix]
        static void Postfix(int index)
        {
            CSingleton<GameUIScreen>.Instance.EvaluateShopLevelAndExp();

            Plugin.SaveHandler.GetAchievementHandler().UpdateAvailability(APLogicUtil.GetAllAvailablePacks());
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
