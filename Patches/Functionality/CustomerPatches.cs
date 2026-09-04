using ApClient.Archipelago.Mapping;
using ApClient.mapping;
using HarmonyLib;
using I2.Loc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using TMPro;

namespace ApClient.Patches.Functionality;

public class CustomerPatches
{

    [HarmonyPatch(typeof(Customer), "EvaluateFinishScanItem")]
    public static class FinishScan
    {
        [HarmonyPostfix]
        public static void Postfix(Customer __instance)
        {
            if (Plugin.ItemHandler.cashOnly)
            {
                __instance.m_CustomerCash.SetIsCard(false);
                __instance.m_CustomerCash.gameObject.SetActive(value: true);
                __instance.m_Anim.SetBool("HandingOverCash", value: true);
                __instance.m_CurrentQueueCashierCounter.SetCustomerPaidAmount(false, __instance.GetRandomPayAmount(__instance.m_TotalScannedItemCost));
                __instance.m_CurrentQueueCashierCounter.UpdateCashierCounterState(ECashierCounterState.TakingCash);
                __instance.m_IsCheckScanItemOutOfBound = false;

            }
        }
    }

    [HarmonyPatch(typeof(Customer), "OnItemScanned")]
    public static class OnScan
    {
        [HarmonyPostfix]
        public static void Postfix(Item item)
        {
            CPlayerData.m_StockSoldList[(int)item.GetItemType()]++;
            Plugin.Logger.LogInfo($"{item} has sold {CPlayerData.m_StockSoldList[(int)item.GetItemType()]} times");
            var locations = LicenseMapping.GetLocations(item.GetItemType());
            foreach (var loc in locations)
            {
                int amount = CPlayerData.m_StockSoldList[(int)item.GetItemType()];
                if (loc.count == amount)
                {
                    Plugin.Logger.LogInfo($"{item.GetItemType()} has {locations.Count()} goals left");
                    Plugin.ArchipelagoHandler.CompleteLocationChecks(loc.id);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Customer), "OnCardScanned")]
    public static class OnCardScan
    {
        [HarmonyPostfix]
        public static void Postfix(InteractableCard3d card)
        {
            if (card.m_Card3dUI.m_CardUI.GetCardData().expansionType == ECardExpansionType.Ghost && Plugin.IsGameReady())
            {
                if (Plugin.ArchipelagoHandler.slotData.Goal == 2)
                {
                    Plugin.SaveHandler.AddGhostSold();
                    if (Plugin.SaveHandler.GetSaveData().GhostCardsSold >= Plugin.ArchipelagoHandler.slotData.GhostGoalAmount)
                    {
                        Plugin.ArchipelagoHandler.Release();
                    }
                }
            }

            Plugin.SaveHandler.AddCard(card.m_Card3dUI.m_CardUI.GetCardData(), Constants.SELL_ACHIEVEMENT_TYPE);

        }
    }

    [HarmonyPatch(typeof(Customer), "StenchLeaveCheck")]
    public static class StenchLeaveCheck
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __result)
        {
            if (__result && Plugin.ArchipelagoHandler.slotData.Deathlink)
            {
                PopupTextPatches.ShowCustomText("Too much Stink! Sending Deathlink");
                Plugin.ArchipelagoHandler.sendDeath();
            }
        }
    }

    [HarmonyPatch(typeof(CustomerManager), "EvaluateTargetBuyItemList")]
    public static class TargetBuyList
    {
        [HarmonyPrefix]
        public static bool Prefix(CustomerManager __instance)
        {
            if (CPlayerData.m_CurrentDay % 7 != 0 && __instance.m_TargetBuyItemList.Count != 0)
            {
                return false;
            }
            __instance.m_TargetBuyItemList.Clear();

            int num = 2 + (CPlayerData.m_ShopLevel + 1) / 10;
            if (num > 8)
            {
                num = 8;
            }

            List<EItemType> itemTypeListOnShelf = ShelfManager.GetItemTypeListOnShelf();
            for (int i = 0; i < itemTypeListOnShelf.Count; i++)
            {
                if (itemTypeListOnShelf.Count <= 0)
                {
                    break;
                }

                int index = UnityEngine.Random.Range(0, itemTypeListOnShelf.Count);
                __instance.m_TargetBuyItemList.Add(itemTypeListOnShelf[index]);
                itemTypeListOnShelf.RemoveAt(index);
                if (__instance.m_TargetBuyItemList.Count >= num)
                {
                    break;
                }
            }

            if (Plugin.IsGameReady())
            {
                List<EItemType> unlockedAP = Archipelago.APLogicUtil.GetAllAvailableItems();
                for (int j = 0; j < unlockedAP.Count; j++)
                {
                    int index2 = UnityEngine.Random.Range(0, unlockedAP.Count);
                    __instance.m_TargetBuyItemList.Add(unlockedAP[index2]);
                    unlockedAP.RemoveAt(index2);
                    if (__instance.m_TargetBuyItemList.Count >= num + num / 2)
                    {
                        break;
                    }
                }
            }

            CPlayerData.m_TargetBuyItemList = __instance.m_TargetBuyItemList;
            return false;
        }
    }

    [HarmonyPatch(typeof(Customer), "ActivateCustomer")]
    public static class Activate
    {
        [HarmonyPostfix]
        public static void Postfix(Customer __instance, bool canSpawnSmelly)
        {
            float old = __instance.m_MaxMoney;
            if (Plugin.IsGameReady())
            {
                __instance.m_MaxMoney = __instance.m_MaxMoney * (1 + Plugin.SaveHandler.GetSaveData().CustomerMoneyMult);
            }
            //__instance.m_IsChattyCustomer = true;
            Plugin.Logger.LogInfo($"Customer spawned with {__instance.m_MaxMoney} instead of {old}");
        }
    }

    [HarmonyPatch(typeof(Customer), "PlayTableGameEnded")]
    public static class FinishPaytableGame
    {
        [HarmonyPostfix]
        public static void Postfix(Customer __instance, float totalPlayTime, float playTableFee)
        {
            if (totalPlayTime > 0f)
            {

                EGameEventFormat format = Plugin.ArchipelagoHandler.slotData.NoFormat ? EGameEventFormat.MAX : CPlayerData.m_GameEventFormat;
                var save = Plugin.SaveHandler.GetSaveData();
                Plugin.Logger.LogInfo($"format: {format} num {save.PlayedGames[format]}");
                if (!save.PlayedGames.ContainsKey(format))
                {
                    Plugin.Logger.LogWarning($"Initializing missing PlayedGames key: {format}");
                    
                }

                
                int checknum = (++save.PlayedGames[format])/2;

                if (checknum != -1 && checknum <= Plugin.ArchipelagoHandler.slotData.PlayTableChecks)
                {
                    Plugin.Logger.LogInfo($"Completing location check for format {format} and check number {checknum} and ID {PlayTableMapping.PlayCheckStartingId + ((int)format * 15) + checknum - 1}");
                    Plugin.ArchipelagoHandler.CompleteLocationChecks(PlayTableMapping.PlayCheckStartingId + ((int)format * 15) + checknum - 1);
                    UIInfoPanel.getInstance().UpdateFormatCount(format, checknum);
                }
            }
        }

    }

    [HarmonyPatch(typeof(CustomerTradeCardScreen), nameof(CustomerTradeCardScreen.SetCustomer))]
    public static class OverrideTrades
    {
        [HarmonyPostfix]
        public static void Postfix(CustomerTradeCardScreen __instance, CustomerTradeData customerTradeData)
        {
            if (!__instance.m_IsTrading)
                return;
            if (customerTradeData != null)
                return;
            CardData newCard = Plugin.SaveHandler.NewRandomCard();
            __instance.m_CardData_L = newCard;

            bool isNew = CPlayerData.GetCardAmount(newCard) == 0;
            __instance.m_IsNewUI.SetActive(isNew);
            __instance.m_CardUI_L.SetCardUI(newCard);
            __instance.m_CardUI_Album_L.SetCardUI(newCard);
            __instance.m_AlbumCardCount_L.text = "X" + CPlayerData.GetCardAmount(newCard);
            if (newCard.cardGrade > 0)
            {
                __instance.m_AlbumCardCount_L.text = CPlayerData.HasGradedCardInAlbum(newCard) ? "X1" : "X0";
            }

            float marketPrice = CPlayerData.GetCardMarketPrice(newCard);
            __instance.m_MarketPrice_L.text =  LocalizationManager.GetTranslation("Market Price") + " : " + GameInstance.GetPriceString(marketPrice);
        }
    }
}
