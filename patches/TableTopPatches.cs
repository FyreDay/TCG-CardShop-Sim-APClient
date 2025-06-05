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
        __instance.m_CurrentRestockDataIndexList.Clear();
        __instance.m_CurrentRestockDataIndexList.AddRange(Plugin.m_SessionHandler.GetSlotData().ttIndexMapping.Keys.Cast<EItemType>().ToList().Select(i => (int)i));
        __instance.m_CurrentRestockDataIndexList.AddRange([-1, -1, -1]);


        for (int m = 0; m < __instance.m_CurrentRestockDataIndexList.Count && m < __instance.m_RestockItemPanelUIList.Count; m++)
        {
            __instance.m_RestockItemPanelUIList[m].Init(__instance, __instance.m_CurrentRestockDataIndexList[m]);
            __instance.m_RestockItemPanelUIList[m].SetActive(isActive: true);
            __instance.m_ScrollEndPosParent = __instance.m_RestockItemPanelUIList[m].gameObject;
        }
        return false;
    }

}
