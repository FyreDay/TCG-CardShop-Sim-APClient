using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ApClient.patches;

[HarmonyPatch(typeof(RestockItemBoardGameScreen))]
[HarmonyPatch("EvaluateRestockItemPanelUI")]
public class TableTopPatches
{
    [HarmonyPrefix]
    static bool Prefix(RestockItemBoardGameScreen __instance, int pageIndex)
    {
        if (__instance.m_PageIndex == pageIndex)
        {
            return false;
        }

        __instance.m_PageIndex = pageIndex;

        List<EItemType> list = Plugin.m_SessionHandler.GetSlotData().ttIndexMapping.Keys.Cast<EItemType>().ToList();


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
                else if (list[k] == EItemType.None)
                {
                    __instance.m_CurrentRestockDataIndexList.Add(-1);
                    break;
                }

            }
            long id = (long)list[k == 0 ? 190 : k];
            if (Plugin.m_SessionHandler.itemCount(id) < 2 || matches.Count < 2)
            {
                __instance.m_CurrentRestockDataIndexList.Add(matches[0]);
            }
            else
            {
                __instance.m_CurrentRestockDataIndexList.Add(matches[1]);
            }

        }

        for (int m = 0; m < __instance.m_CurrentRestockDataIndexList.Count && m < __instance.m_RestockItemPanelUIList.Count; m++)
        {
            __instance.m_RestockItemPanelUIList[m].Init(__instance, __instance.m_CurrentRestockDataIndexList[m]);
            __instance.m_RestockItemPanelUIList[m].SetActive(isActive: true);
            __instance.m_ScrollEndPosParent = __instance.m_RestockItemPanelUIList[m].gameObject;
        }
        return false;
    }

}
