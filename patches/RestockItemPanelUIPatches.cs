﻿using ApClient.mapping;
using HarmonyLib;
using I2.Loc;
using System;
using System.Collections.Generic;
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
    public static int firstIndex = -1;
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
            List<EItemType> list = new List<EItemType>();
            List<int> index_to_id = new List<int>();
            int[] origionalItems = LicenseMapping.pg1_ids;
            switch (restockItemScreen.m_PageIndex)
            {
                case 0:
                    list = CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownItemType;
                    index_to_id = Plugin.pg1IndexMapping;
                    origionalItems = LicenseMapping.pg1_ids;
                    if (firstIndex == -1)
                    {
                        firstIndex = index;
                        CPlayerData.SetUnlockItemLicense(index);
                    }
                    break;
                case 1:
                    list = CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownAccessoryItemType;
                    index_to_id = Plugin.pg2IndexMapping;
                    origionalItems = LicenseMapping.pg2_ids;
                    break;
                case 2:
                    list = CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownFigurineItemType;
                    index_to_id = Plugin.pg3IndexMapping;
                    origionalItems = LicenseMapping.pg3_ids;
                    break;
            }

            if (restockItemScreen.m_SortedRestockDataIndexList.IndexOf(index) == -1)
            {
                return;
            }

            RestockData restockData = InventoryBase.GetRestockData(origionalItems[index_to_id.IndexOf(index)]);
            //Plugin.Log($"index {index} is at {restockItemScreen.m_SortedRestockDataIndexList.IndexOf(index)} which was {origionalItems[restockItemScreen.m_SortedRestockDataIndexList.IndexOf(index)]}");
            __instance.m_LevelRequired = restockData.licenseShopLevelRequired;
            __instance.m_LicensePriceText.text = GameInstance.GetPriceString(0);

            var value = LicenseMapping.getValueOrEmpty(index);
            if (value.locid == -1)
            {
                Plugin.Log($"Failed to find index: {index}");
                return;
            }

            runLicenseBtnLogic(__instance, Plugin.hasItem(value.itemid), index);


        }

    }

    public static void runLicenseBtnLogic(RestockItemPanelUI __instance, bool hasItem, int index)
    {

        if (hasItem && CPlayerData.m_ShopLevel +1 > __instance.m_LevelRequired || firstIndex == index)
        {
            __instance.m_UIGrp.SetActive(value: true);
            __instance.m_LicenseUIGrp.SetActive(value: false);
            __instance.m_LockPurchaseBtn.gameObject.SetActive(value: false);

            EItemType type = InventoryBase.GetRestockData(index).itemType;
            var goals = LicenseMapping.GetKeyValueFromType(type).Where(i => i.Value.count < CPlayerData.m_StockSoldList[(int)type]); ;
            if (goals.Any()) {
                //Set Text
                var targetRect = __instance.m_UIGrp.GetComponentsInChildren<RectTransform>(true).FirstOrDefault(rt => rt.name == "UnitPriceText");
                if (targetRect != null)
                {
                    var localizeComponent = targetRect.GetComponent<I2.Loc.Localize>();
                    if (localizeComponent != null)
                    {
                        localizeComponent.SetTerm("Sold Check Progress");
                    }
                }

                __instance.m_UnitPriceText.text = $"{CPlayerData.m_StockSoldList[(int)type]}/{goals.OrderBy(x => x.Value.count).FirstOrDefault().Value.count}";
                __instance.m_UnitPriceText.color = Color.blue;
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
                __instance.m_UnitPriceText.text = $"{CPlayerData.m_StockSoldList[(int)type]}";
                __instance.m_UnitPriceText.color = Color.green;
            }
        }
        else if (CPlayerData.m_ShopLevel + 1 >= __instance.m_LevelRequired)
        {
            __instance.m_UIGrp.SetActive(value: false);
            __instance.m_LockPurchaseBtn.gameObject.SetActive(value: true);
            __instance.m_LicenseUIGrp.SetActive(value: true);
            __instance.m_LevelRequirementText.gameObject.SetActive(value: true);
            __instance.m_LevelRequirementText.text = "Level Reached, License Locked by AP";
        }
        else
        {
            __instance.m_UIGrp.SetActive(value: false);
            __instance.m_LicenseUIGrp.SetActive(value: true);
            __instance.m_LockPurchaseBtn.gameObject.SetActive(value: true);
            __instance.m_LevelRequirementText.text = $"Shop Level {__instance.m_LevelRequired} Required and License Locked by AP";
            __instance.m_LevelRequirementText.gameObject.SetActive(value: true);
        }
    }


    [HarmonyPatch(typeof(RestockItemPanelUI), "OnPressPurchaseButton")]
    public class OnClick
    {
        // Prefix: Runs before the method
        static bool Prefix(RestockItemPanelUI __instance)
        {
            return false;
        }
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
            switch (__instance.m_PageIndex)
            {
                case 0:
                    __instance.m_CurrentRestockDataIndexList.Clear();
                    __instance.m_CurrentRestockDataIndexList.AddRange(Plugin.pg1IndexMapping);
                    break;
                case 1:
                    __instance.m_CurrentRestockDataIndexList.Clear();
                    __instance.m_CurrentRestockDataIndexList.AddRange(Plugin.pg2IndexMapping);
                    break;
                case 2:
                    __instance.m_CurrentRestockDataIndexList.Clear();
                    __instance.m_CurrentRestockDataIndexList.AddRange(Plugin.pg3IndexMapping);
                    break;
            }
            Plugin.Log("new");
            Plugin.Log(string.Join(", ", __instance.m_CurrentRestockDataIndexList));
        }
    }
}
