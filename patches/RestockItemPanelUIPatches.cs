using ApClient.mapping;
using HarmonyLib;
using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

namespace ApClient.patches;

public class RestockItemPanelUIPatches
{
    [HarmonyPatch(typeof(RestockItemPanelUI), "Init")]
    public class Init
    {
        [HarmonyPrefix]
        static void Prefix(RestockItemPanelUI __instance, RestockItemScreen restockItemScreen, ref int index)
        {
        }
        [HarmonyPostfix]
        static void Postfix(RestockItemPanelUI __instance, RestockItemScreen restockItemScreen, int index)
        {
            OrderedDictionary orderedDictionary = new OrderedDictionary();

            switch (restockItemScreen.m_PageIndex)
            {
                case 0:
                    orderedDictionary = Plugin.m_SessionHandler.GetSlotData().pg1IndexMapping;
                    break;
                case 1:
                    orderedDictionary = Plugin.m_SessionHandler.GetSlotData().pg2IndexMapping;
                    break;
                case 2:
                    orderedDictionary = Plugin.m_SessionHandler.GetSlotData().pg3IndexMapping;
                    break;
            }
            if (restockItemScreen is RestockItemBoardGameScreen)
            {
                orderedDictionary = Plugin.m_SessionHandler.GetSlotData().ttIndexMapping;
                if (index == -1)
                {
                    return;
                }
            }

            List<EItemType> list = orderedDictionary.Keys.Cast<EItemType>().ToList();

            __instance.m_LevelRequired = (int)orderedDictionary[__instance.m_ItemType];

            runLicenseBtnLogic(__instance, Plugin.m_SessionHandler.hasItem((int)__instance.m_ItemType), index, orderedDictionary);

        }

    }
    private static Vector2 defaultAnchorText = new Vector2();
    private static bool setdefaultAnchortext = false;
    private static Vector2 defaultAnchorPrice = new Vector2();
    private static bool setdefaultAnchorprice = false;

    public static void runLicenseBtnLogic(RestockItemPanelUI __instance, bool hasItem, int index, OrderedDictionary orderedDictionary)
    {
        int licenses_required = Plugin.m_SessionHandler.GetRemainingLicenses(__instance.m_LevelRequired);
        //Plugin.Log($"Item Data: {index} : {__instance.m_LevelRequired}");
        if (hasItem && CPlayerData.m_ShopLevel + 1 >= __instance.m_LevelRequired && licenses_required <= 0)
        {
            __instance.m_UIGrp.SetActive(value: true);
            __instance.m_LicenseUIGrp.SetActive(value: false);
            __instance.m_LockPurchaseBtn.gameObject.SetActive(value: false);

            var goals = LicenseMapping.GetLocations(__instance.m_ItemType).Where(i => i.count > CPlayerData.m_StockSoldList[(int)__instance.m_ItemType]);

            if (goals.Any())
            {
                //Set Text
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
                if (!setdefaultAnchorprice) {
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
                //Set Text
                var targetRect = __instance.m_UIGrp.GetComponentsInChildren<RectTransform>(true).FirstOrDefault(rt => rt.name == "UnitPriceText");
                if (targetRect != null)
                {
                    var localizeComponent = targetRect.GetComponent<I2.Loc.Localize>();
                    if (localizeComponent != null)
                    {
                        localizeComponent.SetTerm("Checks Completed. Total Sold:");
                    }
                }
                __instance.m_UnitPriceText.text = $"{CPlayerData.m_StockSoldList[(int)__instance.m_ItemType]}";
                __instance.m_UnitPriceText.color = UnityEngine.Color.green;
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


    [HarmonyPatch(typeof(RestockItemPanelUI), "OnPressPurchaseButton")]
    public class OnClick
    {
        // Prefix: Runs before the method
        //static bool Prefix(RestockItemPanelUI __instance)
        //{
        //    if (Plugin.m_SessionHandler.hasItem(LicenseMapping.getValueOrEmpty(__instance.m_Index).itemid))
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        PopupTextPatches.ShowCustomText("License Unowned");
        //        return false;
        //    }
            
        //}
    }
    [HarmonyPatch(typeof(RestockItemScreen), "Init")]
    public class InitScreen
    {
        [HarmonyPrefix]
        public static void Prefix(RestockItemScreen __instance)
        {
            CPlayerData.m_RestockSortingType = ERestockSortingType.Default;
            //__instance.m_SortBtnList.ForEach(t => { t.gameObject.SetActive(false); });
        }


    }

    [HarmonyPatch(typeof(RestockItemScreen), "EvaluateRestockItemPanelUI")]
    public class Evaluate
    {
        [HarmonyPrefix]
        public static bool Prefix(RestockItemScreen __instance, int pageIndex)
        {
            if (__instance.m_PageIndex == pageIndex)
            {
                return false;
            }

            __instance.m_PageIndex = pageIndex;

            List<EItemType> list = new List<EItemType>();
            switch (pageIndex)
            {
                case 0:
                    list = Plugin.m_SessionHandler.GetSlotData().pg1IndexMapping.Keys.Cast<EItemType>().ToList();
                    break;
                case 1:
                    list = Plugin.m_SessionHandler.GetSlotData().pg2IndexMapping.Keys.Cast<EItemType>().ToList();
                    break;
                case 2:
                    list = Plugin.m_SessionHandler.GetSlotData().pg3IndexMapping.Keys.Cast<EItemType>().ToList();
                    break;
                case 3:
                    list = Plugin.m_SessionHandler.GetSlotData().ttIndexMapping.Keys.Cast<EItemType>().ToList();
                    break;
            }
            

            for (int j = 0; j < __instance.m_RestockItemPanelUIList.Count; j++)
            {
                __instance.m_RestockItemPanelUIList[j].SetActive(isActive: false);
            }

            __instance.m_CurrentRestockDataIndexList.Clear();
            for (int k = 0; k < list.Count; k++)
            {
                List<int> matches = new List<int>();
                for (int l = 0; l < CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList.Count; l++)
                {
                    
                    if (list[k] == CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList[l].itemType)
                    {
                        matches.Add(l);
                        
                    }
                    
                }
                long id = (long)list[k] == 0 ? 190 : (long)list[k];
                if (Plugin.m_SessionHandler.itemCount(id) < 2 || matches.Count < 2)
                {
                    __instance.m_CurrentRestockDataIndexList.Add(matches[0]);
                }
                else
                {
                    __instance.m_CurrentRestockDataIndexList.Add(matches[1]);
                }
                
            }
            __instance.EvaluateSorting();
            for (int m = 0; m < __instance.m_SortedRestockDataIndexList.Count && m < __instance.m_RestockItemPanelUIList.Count; m++)
            {
                __instance.m_RestockItemPanelUIList[m].Init(__instance, __instance.m_SortedRestockDataIndexList[m]);
                __instance.m_RestockItemPanelUIList[m].SetActive(isActive: true);
                __instance.m_ScrollEndPosParent = __instance.m_RestockItemPanelUIList[m].gameObject;
            }
            return false;
        }

}
    [HarmonyPatch(typeof(RestockItemScreen), "OnPressChangePageButton")]
    public class OnPressChangePageButton
    {
        [HarmonyPrefix]
        
        public static void Prefix(RestockItemScreen __instance)
        {
            __instance.m_CurrentSortingMethod = ERestockSortingType.Default;
        }
    }

    [HarmonyPatch(typeof(RestockItemScreen), "EvaluateSorting")]
    public class Sorting
    {

        [HarmonyPrefix]
        public static void Prefix(RestockItemScreen __instance)
        {
            __instance.m_CurrentSortingMethod = ERestockSortingType.Default;
        }
    }
}
