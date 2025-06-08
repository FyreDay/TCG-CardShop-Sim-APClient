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
            //index
            Plugin.Log("Start PAnel Init");

            OrderedDictionary orderedDictionary = new OrderedDictionary();

            switch (restockItemScreen.m_PageIndex)
            {
                case 0:
                    orderedDictionary = Plugin.m_SessionHandler.GetSlotData().pg1IndexMapping;
                    Plugin.Log($"0: {orderedDictionary.Count}");
                    break;
                case 1:
                    orderedDictionary = Plugin.m_SessionHandler.GetSlotData().pg2IndexMapping;
                    Plugin.Log($"1 {orderedDictionary.Count}");
                    break;
                case 2:
                    orderedDictionary = Plugin.m_SessionHandler.GetSlotData().pg3IndexMapping;
                    Plugin.Log($"2 {orderedDictionary.Count}");
                    break;
            }
            Plugin.Log("Check Board Game");
            if (restockItemScreen is RestockItemBoardGameScreen)
            {
                orderedDictionary = Plugin.m_SessionHandler.GetSlotData().ttIndexMapping;
                Plugin.Log($"tt {orderedDictionary.Count}");
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

    public static void runLicenseBtnLogic(RestockItemPanelUI __instance, bool hasItem, int index, OrderedDictionary orderedDictionary)
    {

        __instance.m_LevelRequirementText.text = LocalizationManager.GetTranslation(__instance.m_LevelRequirementString).Replace("XXX", __instance.m_LevelRequired.ToString());

        //Plugin.Log($"Item Data: {index} : {__instance.m_LevelRequired}");
        if (hasItem && CPlayerData.m_ShopLevel + 1 >= __instance.m_LevelRequired)
        {
            __instance.m_UIGrp.SetActive(value: true);
            __instance.m_LicenseUIGrp.SetActive(value: false);
            __instance.m_LockPurchaseBtn.gameObject.SetActive(value: false);

            //List<EItemType> list = orderedDictionary.Keys.Cast<EItemType>().ToList();
            //    Plugin.Log("Start get data list");
            //EItemType type = (EItemType)orderedDictionary.Cast<DictionaryEntry>().ElementAt(index).Key;

            //var goals = LicenseMapping.GetLocations(type).Where(i => i.count > CPlayerData.m_StockSoldList[(int)type]);

            //if (goals.Any())
            //{
            //    //Set Text
            //    var targetRect = __instance.m_UIGrp.GetComponentsInChildren<RectTransform>(true).FirstOrDefault(rt => rt.name == "UnitPriceText");
            //    if (targetRect != null)
            //    {
            //        var localizeComponent = targetRect.GetComponent<I2.Loc.Localize>();
            //        if (localizeComponent != null)
            //        {
            //            localizeComponent.SetTerm("Sold Check Progress");
            //        }
            //    }

            //    __instance.m_UnitPriceText.text = $"{CPlayerData.m_StockSoldList[(int)type]}/{goals.OrderBy(x => x.count).FirstOrDefault().count}";
            //    __instance.m_UnitPriceText.color = UnityEngine.Color.blue;
            //}
            //else
            //{
            //    //Set Text
            //    var targetRect = __instance.m_UIGrp.GetComponentsInChildren<RectTransform>(true).FirstOrDefault(rt => rt.name == "UnitPriceText");
            //    if (targetRect != null)
            //    {
            //        var localizeComponent = targetRect.GetComponent<I2.Loc.Localize>();
            //        if (localizeComponent != null)
            //        {
            //            localizeComponent.SetTerm("Checks Completed. Total Sold:");
            //        }
            //    }
            //    __instance.m_UnitPriceText.text = $"{CPlayerData.m_StockSoldList[(int)type]}";
            //    __instance.m_UnitPriceText.color = UnityEngine.Color.green;
            //}
        }
        else if (hasItem)
        {
            __instance.m_UIGrp.SetActive(value: false);
            __instance.m_LicenseUIGrp.SetActive(value: true);
            __instance.m_LockPurchaseBtn.gameObject.SetActive(value: true);
            __instance.m_LevelRequirementText.text = $"Level {__instance.m_LevelRequired} Required. License Found";
            __instance.m_LevelRequirementText.gameObject.SetActive(value: true);
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
            __instance.m_LevelRequirementText.text = $"Level {__instance.m_LevelRequired} Required. License Locked by AP";
            __instance.m_LevelRequirementText.gameObject.SetActive(value: true);
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
                if(Plugin.m_SaveManager.GetItemLevel(list[k]) < 2 || matches.Count < 2)
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
