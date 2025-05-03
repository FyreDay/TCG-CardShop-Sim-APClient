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
}
