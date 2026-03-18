using ApClient.Archipelago;
using ApClient.Archipelago.Mapping;
using ApClient.patches;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

namespace ApClient.Patches.Functionality.RestockScreen;

public class RestockItemUIPatches
{
    [HarmonyPatch(typeof(RestockItemPanelUI), "OnPressPurchaseButton")]
    public class OnClick
    {
        [HarmonyPrefix]
        static bool Prefix(RestockItemPanelUI __instance)
        {
            if (Plugin.ArchipelagoHandler.GetItemCount(__instance.m_ItemType == EItemType.BasicCardPack ? 190 : (long)__instance.m_ItemType) > 1)
            {
                return true;
            }
            else
            {
                PopupTextPatches.ShowCustomText("Item Locked");
                return false;
            }
        }
    }

    [HarmonyPatch(typeof(RestockItemPanelUI), "Init")]
    public class Init
    {
        private static bool setdefaultAnchortext = false;
        private static Vector2 defaultAnchorText = new Vector2();
        private static bool setdefaultAnchorprice = false;
        private static Vector2 defaultAnchorPrice = new Vector2();
        private static TextMeshProUGUI[] cardCheckText = new TextMeshProUGUI[8];
        private static TextMeshProUGUI[] cardProgressText = new TextMeshProUGUI[8];
        private static void SetGoalText(RestockItemPanelUI __instance, List<(int id, int count)> goals)
        {
            if (goals.Any())
            {

                var targetRect = __instance.m_UIGrp.GetComponentsInChildren<RectTransform>(true).FirstOrDefault(rt => rt.name == "UnitPriceText");
                if (targetRect != null)
                {
                    var localizeComponent = targetRect.GetComponent<I2.Loc.Localize>();
                    if (localizeComponent != null)
                    {
                        localizeComponent.SetTerm("Check Progress:");
                        var textComponent = localizeComponent.GetComponent<TextMeshProUGUI>();
                        if (textComponent != null)
                        {
                            if (!setdefaultAnchortext)
                            {
                                textComponent.rectTransform.anchoredPosition += new Vector2(-100, 0);
                                defaultAnchorText = textComponent.rectTransform.anchoredPosition;
                                setdefaultAnchortext = true;
                            }
                            else
                            {
                                textComponent.rectTransform.anchoredPosition = defaultAnchorText;
                            }
                            textComponent.enableWordWrapping = false;
                            textComponent.overflowMode = TextOverflowModes.Overflow;
                            textComponent.enableAutoSizing = false;
                        }
                    }
                }

                __instance.m_UnitPriceText.text = $"{CPlayerData.m_StockSoldList[(int)__instance.m_ItemType]} / {goals.OrderBy(x => x.count).FirstOrDefault().count}   Checks Left: {goals.Count()}";
                __instance.m_UnitPriceText.color = UnityEngine.Color.cyan;
                __instance.m_UnitPriceText.outlineColor = UnityEngine.Color.black;

                __instance.m_UnitPriceText.enableWordWrapping = false;
                __instance.m_UnitPriceText.overflowMode = TextOverflowModes.Overflow;
                __instance.m_UnitPriceText.enableAutoSizing = false;
                if (!setdefaultAnchorprice)
                {
                    __instance.m_UnitPriceText.rectTransform.anchoredPosition += new Vector2(20, 0);
                    defaultAnchorPrice = __instance.m_UnitPriceText.rectTransform.anchoredPosition;
                    setdefaultAnchorprice = true;
                }
                else
                {
                    __instance.m_UnitPriceText.rectTransform.anchoredPosition = defaultAnchorPrice;
                }

            }
            else
            {
                var targetRect = __instance.m_UIGrp.GetComponentsInChildren<RectTransform>(true).FirstOrDefault(rt => rt.name == "UnitPriceText");
                if (targetRect != null)
                {
                    var localizeComponent = targetRect.GetComponent<I2.Loc.Localize>();
                    if (localizeComponent != null)
                    {
                        localizeComponent.SetTerm("Checks Completed. Total Sold:");
                        var textComponent = localizeComponent.GetComponent<TextMeshProUGUI>();
                        if (textComponent != null)
                        {
                            textComponent.enableWordWrapping = true;
                            textComponent.overflowMode = TextOverflowModes.Overflow;
                            textComponent.enableAutoSizing = true;
                        }
                    }
                }
                __instance.m_UnitPriceText.text = $"{CPlayerData.m_StockSoldList[(int)__instance.m_ItemType]}";
                __instance.m_UnitPriceText.color = UnityEngine.Color.green;
            }
        }

        private static readonly HashSet<EItemType> CardBoxes = new HashSet<EItemType>
        {
            EItemType.BasicCardBox,
            EItemType.RareCardBox,
            EItemType.EpicCardBox,
            EItemType.LegendaryCardBox,
            EItemType.DestinyBasicCardBox,
            EItemType.DestinyRareCardBox,
            EItemType.DestinyEpicCardBox,
            EItemType.DestinyLegendaryCardBox,
        };

        private static void SetCardBoxText(RestockItemPanelUI __instance)
        {
            int packtype = ((int)__instance.m_ItemType / 2);
            if (cardCheckText[packtype] == null)
            {
                cardCheckText[packtype] = GameObject.Instantiate(__instance.m_UnitPriceText, __instance.m_UnitPriceText.transform.parent);
                cardCheckText[packtype].text = "Pack Checks:";
                cardCheckText[packtype].rectTransform.anchoredPosition += new Vector2(-230, 575);
                cardCheckText[packtype].enableWordWrapping = false;
                cardCheckText[packtype].overflowMode = TextOverflowModes.Overflow;
                cardCheckText[packtype].enableAutoSizing = false;
                cardCheckText[packtype].color = UnityEngine.Color.white;
            }

            if (cardProgressText[packtype] == null)
            {
                cardProgressText[packtype] = GameObject.Instantiate(__instance.m_UnitPriceText, __instance.m_UnitPriceText.transform.parent);
                cardProgressText[packtype].text = "";
                cardProgressText[packtype].rectTransform.anchoredPosition += new Vector2(0, 575);
                cardProgressText[packtype].enableWordWrapping = false;
                cardProgressText[packtype].overflowMode = TextOverflowModes.Overflow;
                cardProgressText[packtype].enableAutoSizing = false;
                cardProgressText[packtype].color = UnityEngine.Color.cyan;
            }

            var cardgoals = LicenseMapping.GetLocations((EItemType)((int)__instance.m_ItemType - 1)).Where(i => i.count > CPlayerData.m_StockSoldList[(int)__instance.m_ItemType - 1]);

            if (cardgoals.Any())
            {
                cardProgressText[packtype].text = $"{CPlayerData.m_StockSoldList[(int)__instance.m_ItemType - 1]} / {cardgoals.OrderBy(x => x.count).FirstOrDefault().count}   Checks Left: {cardgoals.Count()}";
                cardCheckText[packtype].text = "Pack Checks:";
            }
            else
            {
                cardProgressText[packtype].text = "";
                cardCheckText[packtype].text = "        No Card Pack Checks";
            }
        }

        [HarmonyPostfix]
        static void Postfix(RestockItemPanelUI __instance, RestockItemScreen restockItemScreen, int index)
        {
            int licenses_required = APLogicUtil.GetRemainingLicenses(__instance.m_LevelRequired);
            bool hasItem = Plugin.ArchipelagoHandler.GetItemCount((long)__instance.m_ItemType) > 0;
            if (hasItem && CPlayerData.m_ShopLevel + 1 >= __instance.m_LevelRequired && licenses_required <= 0)
            {
                __instance.m_UIGrp.SetActive(value: true);
                __instance.m_LicenseUIGrp.SetActive(value: false);
                __instance.m_LockPurchaseBtn.gameObject.SetActive(value: false);

                List<(int id, int count)> goals = LicenseMapping.GetLocations(__instance.m_ItemType).Where(i => i.count > CPlayerData.m_StockSoldList[(int)__instance.m_ItemType]).ToList();

                SetGoalText(__instance, goals);

                if (CardBoxes.Contains(__instance.m_ItemType))
                {
                    SetCardBoxText(__instance);
                }
            }
            else if (0 < licenses_required)
            {
                __instance.m_UIGrp.SetActive(value: false);
                __instance.m_LicenseUIGrp.SetActive(value: true);
                __instance.m_LockPurchaseBtn.gameObject.SetActive(value: true);
                __instance.m_LevelRequirementText.text = $"Level {__instance.m_LevelRequired} Requires {licenses_required} more Licenses";
                __instance.m_LevelRequirementText.gameObject.SetActive(value: true);
                __instance.m_LicensePriceText.text = "License Locked";

                if (hasItem)
                {
                    __instance.m_LicensePriceText.text = "License Found";
                }
            }
            else if (hasItem)
            {
                __instance.m_UIGrp.SetActive(value: false);
                __instance.m_LicenseUIGrp.SetActive(value: true);
                __instance.m_LockPurchaseBtn.gameObject.SetActive(value: true);
                __instance.m_LevelRequirementText.text = $"Level {__instance.m_LevelRequired} Required.";
                __instance.m_LevelRequirementText.gameObject.SetActive(value: true);
                __instance.m_LicensePriceText.text = "License Found";
            }
            else if (CPlayerData.m_ShopLevel + 1 >= __instance.m_LevelRequired)
            {
                __instance.m_UIGrp.SetActive(value: false);
                __instance.m_LockPurchaseBtn.gameObject.SetActive(value: true);
                __instance.m_LicenseUIGrp.SetActive(value: true);
                __instance.m_LevelRequirementText.gameObject.SetActive(value: true);
                __instance.m_LevelRequirementText.text = "Level Reached";
                __instance.m_LicensePriceText.text = "License Locked";

            }
            else
            {
                __instance.m_UIGrp.SetActive(value: false);
                __instance.m_LicenseUIGrp.SetActive(value: true);
                __instance.m_LockPurchaseBtn.gameObject.SetActive(value: true);
                __instance.m_LevelRequirementText.text = $"Level {__instance.m_LevelRequired} Required.";
                __instance.m_LevelRequirementText.gameObject.SetActive(value: true);
                __instance.m_LicensePriceText.text = "License Locked";
            }
        }
    }
}
