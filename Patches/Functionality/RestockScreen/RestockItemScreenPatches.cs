using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApClient.Patches.Functionality.RestockScreen;

public class RestockItemScreenPatches
{
    [HarmonyPatch(typeof(RestockItemScreen), "EvaluateRestockItemPanelUI")]
    public class Evaluate
    {
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

        [HarmonyPrefix]
        static bool EvaluateRestockItemPanelUI(RestockItemScreen __instance, int pageIndex)
        {
            if (__instance.m_PageIndex == pageIndex)
            {
                return false;
            }

            __instance.m_PageIndex = pageIndex;
            for (int i = 0; i < __instance.m_PageButtonHighlightList.Count; i++)
            {
                __instance.m_PageButtonHighlightList[i].SetActive(value: false);
            }

            __instance.m_PageButtonHighlightList[__instance.m_PageIndex].SetActive(value: true);
            List<EItemType> list = new List<EItemType>();
            switch (pageIndex)
            {
                case 0:
                    list = CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownItemType;
                    break;
                case 1:
                    list = CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownAccessoryItemType;
                    break;
                case 2:
                    list = CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownFigurineItemType;
                    break;
                case 3:
                    list = CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownAllItemType;
                    break;
            }

            for (int j = 0; j < __instance.m_RestockItemPanelUIList.Count; j++)
            {
                __instance.m_RestockItemPanelUIList[j].SetActive(isActive: false);
            }

            __instance.m_CurrentRestockDataIndexList.Clear();
            for (int k = 0; k < list.Count; k++)
            {
                for (int l = 0; l < CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList.Count; l++)
                {
                    if (list[k] == CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList[l].itemType)
                    {
                        if (InventoryBase.ItemTypeToCollectionPackType(list[k]) != ECollectionPackType.None)
                        {
                            if (Plugin.ArchipelagoHandler.itemCount((int)list[k]) > 1)
                            {
                                if (CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList[l].isBigBox)
                                {
                                    __instance.m_CurrentRestockDataIndexList.Add(l);
                                }
                                
                            }
                            else
                            {
                                if (!CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList[l].isBigBox)
                                {
                                    __instance.m_CurrentRestockDataIndexList.Add(l);
                                }
                            }
                            continue;
                            
                        }
                        Plugin.Logger.LogInfo($"{CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList[l].isBigBox} : {list[k]}");
                        __instance.m_CurrentRestockDataIndexList.Add(l);
                    }
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
}
