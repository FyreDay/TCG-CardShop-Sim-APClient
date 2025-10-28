using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApClient.patches;

//public static GameEventData GetGameEventData(EGameEventFormat gameEventFormat)
public class InventoryBasePatches
{
    [HarmonyPatch(typeof(InventoryBase), nameof(InventoryBase.GetGameEventData))]
    public static class GetFormatData
    {
        static void Postfix(ref GameEventData __result, EGameEventFormat gameEventFormat)
        {
            // Modify the returned list
            __result.unlockPlayCountRequired = 0;
        }
    }

    [HarmonyPatch(typeof(InventoryBase), nameof(InventoryBase.GetRestockData))]
    public static class GetRestockData
    {
        static void Prefix(ref RestockData __result, int index)
        {
            // Modify the returned list
            __result = CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList[index];
        }
    }
}
