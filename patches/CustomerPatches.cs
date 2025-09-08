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

namespace ApClient.patches;

public class CustomerPatches
{

    [HarmonyPatch(typeof(Customer), "EvaluateFinishScanItem")]
    public static class FinishScan
    {
        [HarmonyPostfix]
        public static void Postfix(Customer __instance)
        {
            if (Plugin.isCashOnly())
            {
                __instance.m_CustomerCash.SetIsCard(false);
                CSingleton<CustomerManager>.Instance.ResetCustomerExactChangeChance();
                __instance.m_CurrentQueueCashierCounter.SetCustomerPaidAmount(false, __instance.GetRandomPayAmount(__instance.m_TotalScannedItemCost));

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
                if (loc.count <= CPlayerData.m_StockSoldList[(int)item.GetItemType()])
                {
                    //Plugin.Log($"{item.GetItemType()} has {locations.Count()} goals left");
                    Plugin.m_SessionHandler.CompleteLocationChecks(loc.id);
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
            if (card.m_Card3dUI.m_CardUI.GetCardData().expansionType == ECardExpansionType.Ghost)
            {
                if(Plugin.m_SessionHandler.GetSlotData().Goal == 2)
                {
                    Plugin.m_SaveManager.IncreaseGhostChecks();
                    if (Plugin.m_SaveManager.GetGhostChecks() >= Plugin.m_SessionHandler.GetSlotData().GhostGoalAmount)
                    {
                        Plugin.m_SessionHandler.SendGoalCompletion();
                    }
                }
            }

            if (card.m_Card3dUI.m_CardUI.GetCardData().expansionType == ECardExpansionType.Tetramon || card.m_Card3dUI.m_CardUI.GetCardData().expansionType == ECardExpansionType.Destiny)
            {
                Plugin.m_SaveManager.IncreaseCardSold(card.m_Card3dUI.m_CardUI.GetCardData().expansionType, card.m_Card3dUI.m_CardUI.m_MonsterData.Rarity);
                
                int sold = Plugin.m_SaveManager.GetCardsSold(card.m_Card3dUI.m_CardUI.GetCardData().expansionType, card.m_Card3dUI.m_CardUI.m_MonsterData.Rarity);

                for (int i = 1; i <= Plugin.m_SessionHandler.GetSlotData().NumberOfSellCardChecks; i++)
                {
                    if (sold >= i * Plugin.m_SessionHandler.GetSlotData().SellCardsPerCheck)
                    {
                        
                        Plugin.m_SessionHandler.CompleteLocationChecks(CardMapping.getSellCheckId(card.m_Card3dUI.m_CardUI.GetCardData().expansionType, i-1));
                    }
                }
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
            __instance.m_MaxMoney = __instance.m_MaxMoney * Plugin.m_SaveManager.GetMoneyMult();
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
                Plugin.m_SaveManager.IncreaseCustomersPlayed();

                for (int i = 0; i < Plugin.m_SessionHandler.GetSlotData().NumberOfGameChecks; i++)
                {
                    if (Plugin.m_SaveManager.GetEventGamesPlayed() >= (i+1) * Plugin.m_SessionHandler.GetSlotData().GamesPerCheck)
                    {
                        Plugin.m_SessionHandler.CompleteLocationChecks(PlayTableMapping.PlayCheckStartingId + i);
                    }
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
            if (!Plugin.OverrideTrades())
            {
                return true;
            }
            Plugin.Log("Starting override");
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

            __instance.m_CardData_L = Plugin.getNewCard();
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

    //[HarmonyPatch(typeof(CustomerTradeCardScreen), "SetCustomer")]
    //public static class CustomerTradeScreen_SetCustomer_Transpiler
    //{
    //    // We assume that the original call to GetCardData is a call instruction we can intercept.
    //    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
    //    {
    //        // Get a reference to the private field we're interested in.
    //        FieldInfo field_CardDataL = AccessTools.Field(typeof(CustomerTradeCardScreen), "m_CardData_L");
    //        // Get the original method that creates random card data.
    //        MethodInfo originalGetCardData = AccessTools.Method(typeof(CPlayerData), "GetCardData", new[] { typeof(int), typeof(ECardExpansionType), typeof(bool) });
    //        // Get our custom builder method.
    //        MethodInfo myCustomMethod = AccessTools.Method(typeof(Plugin), nameof(Plugin.getNewCard));
    //        // Get the Plugin.ControlTrades() method.
    //        MethodInfo controlTradesMethod = AccessTools.Method(typeof(Plugin), nameof(Plugin.OverrideTrades));

    //        List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
    //        Label skipCustom = il.DefineLabel();
    //        Label endIf = il.DefineLabel();
    //        bool patchApplied = false;

    //        for (int i = 0; i < codes.Count; i++)
    //        {
    //            CodeInstruction code = codes[i];

    //            // Look for the call to CPlayerData.GetCardData that is used to set m_CardData_L.
    //            if (!patchApplied &&
    //                code.opcode == OpCodes.Call &&
    //                code.operand is MethodInfo mi &&
    //                mi == originalGetCardData)
    //            {
    //                // We assume the method's parameters (num7, eCardExpansionType, flag) are already on the stack.
    //                // We now insert our conditional logic:
    //                // if (customerTradeData == null && Plugin.ControlTrades() == true)
    //                //    CardData = BuildCustomCardData();
    //                // else
    //                //    CardData = original GetCardData(...);

    //                List<CodeInstruction> newInstructions = new List<CodeInstruction>();

    //                // --- Check if customerTradeData is null ---
    //                // Load argument 2 (customerTradeData)
    //                newInstructions.Add(new CodeInstruction(OpCodes.Ldarg_2));
    //                // Load null
    //                newInstructions.Add(new CodeInstruction(OpCodes.Ldnull));
    //                // Compare equality (customerTradeData == null)
    //                newInstructions.Add(new CodeInstruction(OpCodes.Ceq));
    //                // If false (i.e. customerTradeData != null), branch to skip custom logic.
    //                newInstructions.Add(new CodeInstruction(OpCodes.Brfalse_S, skipCustom));

    //                // --- Now check Plugin.ControlTrades() ---
    //                // Call Plugin.ControlTrades()
    //                newInstructions.Add(new CodeInstruction(OpCodes.Call, controlTradesMethod));
    //                // Load constant 0 (false)
    //                newInstructions.Add(new CodeInstruction(OpCodes.Ldc_I4_0));
    //                // Compare equality (if Plugin.ControlTrades() == false)
    //                newInstructions.Add(new CodeInstruction(OpCodes.Ceq));
    //                // If true (i.e. Plugin.ControlTrades() returned false), branch to skip custom.
    //                newInstructions.Add(new CodeInstruction(OpCodes.Brtrue_S, skipCustom));

    //                // --- If both conditions passed, we want to call BuildCustomCardData() ---
    //                // Pop the three parameters that were pushed for the original call.
    //                newInstructions.Add(new CodeInstruction(OpCodes.Pop)); // flag
    //                newInstructions.Add(new CodeInstruction(OpCodes.Pop)); // eCardExpansionType
    //                newInstructions.Add(new CodeInstruction(OpCodes.Pop)); // num7
    //                                                                       // Call our custom method to build the CardData.
    //                newInstructions.Add(new CodeInstruction(OpCodes.Call, myCustomMethod));
    //                // Branch to end, skipping the original call.
    //                newInstructions.Add(new CodeInstruction(OpCodes.Br_S, endIf));

    //                // --- Label for skipping custom logic ---
    //                CodeInstruction labelOriginal = new CodeInstruction(OpCodes.Nop) { labels = new List<Label> { skipCustom } };
    //                newInstructions.Add(labelOriginal);

    //                // Yield our new instructions instead of the original call.
    //                foreach (var instr in newInstructions)
    //                {
    //                    yield return instr;
    //                }
    //                // Yield the original call instruction for when the branch was skipped.
    //                yield return code;

    //                patchApplied = true;
    //                continue;
    //            }
    //            // When we reach the stfld that assigns m_CardData_L, attach the endIf label.
    //            if (patchApplied &&
    //                code.opcode == OpCodes.Stfld &&
    //                code.operand is FieldInfo field &&
    //                field == field_CardDataL)
    //            {
    //                code.labels.Add(endIf);
    //                yield return code;
    //                patchApplied = false; // Only patch once.
    //                continue;
    //            }

    //            // Otherwise, yield the instruction unmodified.
    //            yield return code;
    //        }
    //    }
    //}

    
}
