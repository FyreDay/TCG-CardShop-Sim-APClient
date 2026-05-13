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
            //Plugin.Log($"{item} has sold {CPlayerData.m_StockSoldList[(int)item.GetItemType()]} times");
            var locations = LicenseMapping.GetLocations(item.GetItemType());
            foreach (var loc in locations)
            {
                int amount = CPlayerData.m_StockSoldList[(int)item.GetItemType()];
                if (loc.count == amount)
                {
                    //Plugin.Log($"{item.GetItemType()} has {locations.Count()} goals left");
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
            if (__result)
            {
                PopupTextPatches.ShowCustomText("Too much Stink! Sending Deathlink");
                Plugin.ArchipelagoHandler.sendDeath();
            }
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
                __instance.m_MaxMoney = __instance.m_MaxMoney * Plugin.SaveHandler.GetSaveData().CustomerMoneyMult;
            }
            //__instance.m_IsChattyCustomer = true;
            //Plugin.Log($"Customer spawned with {__instance.m_MaxMoney} instead of {old}");
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

                int checknum = ++Plugin.SaveHandler.GetSaveData().PlayedGames[CPlayerData.m_GameEventFormat];

                if (checknum != -1)
                {
                    Plugin.ArchipelagoHandler.CompleteLocationChecks(PlayTableMapping.PlayCheckStartingId + (int)CPlayerData.m_GameEventFormat * 15 + checknum);
                }
            }
        }

    }

    [HarmonyPatch(typeof(CustomerTradeCardScreen), "SetCustomer")]
    public static class OverrideTrades
    {
        [HarmonyPrefix]
        public static bool Prefix(CustomerTradeCardScreen __instance, Customer customer, CustomerTradeData customerTradeData)
        {
            CSingleton<CustomerManager>.Instance.m_IsPlayerTrading = true;
            __instance.m_CurrentCustomer = customer;
            __instance.m_HasAccepted = false;
            __instance.m_IsTrading = false;
            int num = CPlayerData.m_ShopLevel;
            if (num > 40)
            {
                num = 40;
            }

            if (UnityEngine.Random.Range(0, 100) < num)
            {
                __instance.m_IsTrading = true;
            }
            //remove this
            __instance.m_IsTrading = true;
            if (customerTradeData != null)
            {
                __instance.m_IsTrading = customerTradeData.m_IsTrading;
            }

            if (__instance.m_IsTrading)
            {
                __instance.m_CustomerTopText.text = LocalizationManager.GetTranslation("CustomerTrade/" + __instance.m_CustomerTradeCardTextList[UnityEngine.Random.Range(0, __instance.m_CustomerTradeCardTextList.Count)]);
                __instance.m_CustomerTopTextAnim.Rewind();
                __instance.m_CustomerTopTextAnim.Play();
            }
            else
            {
                __instance.m_MaxDeclineCount = UnityEngine.Random.Range(0, 5);
                __instance.m_DeclineCount = 0;
                __instance.m_CustomerTopText.text = LocalizationManager.GetTranslation("CustomerTrade/" + __instance.m_CustomerSellCardTextList[UnityEngine.Random.Range(0, __instance.m_CustomerSellCardTextList.Count)]);
                __instance.m_CustomerTopTextAnim.Rewind();
                __instance.m_CustomerTopTextAnim.Play();
            }

            __instance.m_CustomerTradingText.SetActive(__instance.m_IsTrading);
            __instance.m_CustomerSellingText.SetActive(!__instance.m_IsTrading);
            __instance.m_TradeGrp_R.SetActive(__instance.m_IsTrading);
            __instance.m_SellGrp_R.SetActive(!__instance.m_IsTrading);

            __instance.m_CardData_L = Plugin.SaveHandler.NewRandomCard();
            ECardExpansionType eCardExpansionType = __instance.m_CardData_L.expansionType;
            if (customerTradeData != null)
            {
                __instance.m_CardData_L = customerTradeData.m_CardData_L;
                eCardExpansionType = __instance.m_CardData_L.expansionType;
            }


            //end L

            bool active = CPlayerData.GetCardAmount(__instance.m_CardData_L) == 0;
            __instance.m_IsNewUI.SetActive(active);
            __instance.m_CardUI_L.SetCardUI(__instance.m_CardData_L);
            __instance.m_CardUI_Album_L.SetCardUI(__instance.m_CardData_L);
            __instance.m_AlbumCardCount_L.text = "X" + CPlayerData.GetCardAmount(__instance.m_CardData_L);
            __instance.m_AcceptBtn.SetActive(value: true);
            __instance.m_CancelBtn.SetActive(value: true);
            __instance.m_LetMeThinkBtn.SetActive(value: true);
            __instance.m_DoneBtn.SetActive(value: false);

            float num3 = 0;
            if (!__instance.m_IsTrading)
            {
                float num8 = UnityEngine.Random.Range(0.6f, 1.3f);

                __instance.m_SellCardMarketPrice = CPlayerData.GetCardMarketPrice(__instance.m_CardData_L);
                num3 = (__instance.m_SellCardAskPrice = __instance.m_SellCardMarketPrice * num8);
                if (customerTradeData != null)
                {
                    __instance.m_PriceSet = customerTradeData.m_PriceSet;
                    __instance.m_LastPriceSet = customerTradeData.m_LastPriceSet;
                    __instance.m_SellCardAskPrice = customerTradeData.m_SellCardAskPrice;
                    __instance.m_MaxDeclineCount = customerTradeData.m_MaxDeclineCount;
                    __instance.m_DeclineCount = customerTradeData.m_DeclineCount;
                    __instance.m_SetPrice.text = GameInstance.GetPriceString(__instance.m_PriceSet);
                    __instance.m_SetPriceInputDisplay.text = GameInstance.GetPriceString(__instance.m_PriceSet);
                }
                else
                {
                    __instance.OnInputTextUpdated("0");
                }

                __instance.m_MarketPrice_L.text = LocalizationManager.GetTranslation("Ask Price") + " : " + GameInstance.GetPriceString(__instance.m_SellCardAskPrice);
                __instance.m_MarketPrice_L.text += "\n";
                TextMeshProUGUI marketPrice_L = __instance.m_MarketPrice_L;
                marketPrice_L.text = marketPrice_L.text + LocalizationManager.GetTranslation("Market Price") + " : " + GameInstance.GetPriceString(__instance.m_SellCardMarketPrice);
                __instance.m_CardUI_Buying.SetCardUI(__instance.m_CardData_L);
            }
            else
            {
                num3 = CPlayerData.GetCardMarketPrice(__instance.m_CardData_L);
                __instance.m_MarketPrice_L.text = LocalizationManager.GetTranslation("Market Price") + " : " + GameInstance.GetPriceString(num3);
            }

            if (!__instance.m_IsTrading)
            {
                return false;
            }

            //right card
            int num2;
            int maxExclusive = InventoryBase.GetShownMonsterList(eCardExpansionType).Count * CPlayerData.GetCardAmountPerMonsterType(eCardExpansionType);
            int num7 = UnityEngine.Random.Range(0, maxExclusive);
            bool flag = false;
            bool flag2 = false;
            if (customerTradeData == null)
            {
                if (eCardExpansionType == ECardExpansionType.Ghost && UnityEngine.Random.Range(0, 100) < 50)
                {
                    flag = true;
                }

                for (int i = 0; i < InventoryBase.GetShownMonsterList(eCardExpansionType).Count * CPlayerData.GetCardAmountPerMonsterType(eCardExpansionType); i++)
                {
                    num2 = i;
                    int grade = 0;
                    float cardMarketPrice = CPlayerData.GetCardMarketPrice(num2, eCardExpansionType, flag, grade);
                    if (CPlayerData.GetCardAmountByIndex(num2, eCardExpansionType, flag) > 0 && cardMarketPrice >= num3 * 0.75f && cardMarketPrice < num3 * 1.5f && UnityEngine.Random.Range(0, 100) < 25 && num7 != num2)
                    {
                        flag2 = true;
                        __instance.m_CardData_R = CPlayerData.GetCardData(num2, eCardExpansionType, flag);
                        if (__instance.m_CardData_L != __instance.m_CardData_R)
                        {
                            break;
                        }
                    }
                }

                if (!flag2)
                {
                    for (int j = 0; j < InventoryBase.GetShownMonsterList(eCardExpansionType).Count * CPlayerData.GetCardAmountPerMonsterType(eCardExpansionType); j++)
                    {
                        num2 = j;
                        int grade = 0;
                        float cardMarketPrice2 = CPlayerData.GetCardMarketPrice(num2, eCardExpansionType, flag, grade);
                        if (cardMarketPrice2 >= num3 * 0.75f && cardMarketPrice2 < num3 * 1.5f && UnityEngine.Random.Range(0, 100) < 15 && num7 != num2)
                        {
                            flag2 = true;
                            __instance.m_CardData_R = CPlayerData.GetCardData(num2, eCardExpansionType, flag);
                            if (__instance.m_CardData_L != __instance.m_CardData_R)
                            {
                                break;
                            }
                        }
                    }
                }

                bool flag3 = false;
                if (__instance.m_CardData_L.isFoil == __instance.m_CardData_R.isFoil && __instance.m_CardData_L.expansionType == __instance.m_CardData_R.expansionType && __instance.m_CardData_L.monsterType == __instance.m_CardData_R.monsterType && __instance.m_CardData_L.borderType == __instance.m_CardData_R.borderType && __instance.m_CardData_L.isDestiny == __instance.m_CardData_R.isDestiny)
                {
                    flag3 = true;
                }

                if (!flag2 || flag3)
                {
                    int k = 0;
                    if (eCardExpansionType == ECardExpansionType.Ghost)
                    {
                        flag = !flag;
                    }

                    for (; !flag2 || k >= 100000; k++)
                    {
                        num2 = UnityEngine.Random.Range(0, maxExclusive);
                        __instance.m_CardData_R = CPlayerData.GetCardData(num2, eCardExpansionType, flag);
                        float cardMarketPrice3 = CPlayerData.GetCardMarketPrice(__instance.m_CardData_R);
                        if (cardMarketPrice3 >= num3 * 0.35f && cardMarketPrice3 < num3 * 2f && num7 != num2 && __instance.m_CardData_L != __instance.m_CardData_R)
                        {
                            flag2 = true;
                        }
                    }
                }
            }
            else if (customerTradeData != null)
            {
                flag2 = true;
                __instance.m_CardData_R = customerTradeData.m_CardData_R;
            }

            if (flag2)
            {
                __instance.m_CardUI_R.SetCardUI(__instance.m_CardData_R);
                __instance.m_CardUI_Album.SetCardUI(__instance.m_CardData_R);
                __instance.m_AlbumCardCount.text = "X" + CPlayerData.GetCardAmount(__instance.m_CardData_R);
                __instance.m_MarketPrice_R.text = LocalizationManager.GetTranslation("Market Price") + " : " + GameInstance.GetPriceString(CPlayerData.GetCardMarketPrice(__instance.m_CardData_R));
            }
            return false;
        }
    }
}
