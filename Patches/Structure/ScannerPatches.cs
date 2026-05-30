using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApClient.Patches.Structure;
[HarmonyPatch(typeof(ScannerRestockScreen), "OnPressUnlockButton")]
public class ScannerPatches
{
    [HarmonyPrefix]
    static bool PreFix()
    {
        if (CPlayerData.m_IsScannerRestockUnlocked)
        {
            //InteractionPlayerController
            return true;
        }
        return false;
    }
}
[HarmonyPatch(typeof(ScannerRestockScreen), "OnOpenScreen")]
public class ScannerAwakePatches
{
    [HarmonyPostfix]
    static void PostFix(ScannerRestockScreen __instance)
    {
        __instance.m_UnlockCostText.text = "Locked By AP";
        //if (!CPlayerData.m_IsScannerRestockUnlocked)
        //{
        //    __instance.m_UnlockCostText.text = "Locked";
        //}
    }
}

[HarmonyPatch(typeof(UI_BarcodeScannerScreen))]
public class BarcodePatches
{
    [HarmonyPatch("GetRestockIndex")]
    [HarmonyPostfix]
    static void PostFix(ref int __result)
    {
        Plugin.Logger.LogInfo($"{__result}");
    }
}
