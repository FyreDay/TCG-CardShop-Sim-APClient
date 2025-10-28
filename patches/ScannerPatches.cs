using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApClient.patches;
[HarmonyPatch(typeof(ScannerRestockScreen))]
public class ScannerPatches
{
    [HarmonyPatch("OnPressUnlockButton")]
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
[HarmonyPatch(typeof(UI_BarcodeScannerScreen))]
public class BarcodePatches
{
    [HarmonyPatch("GetRestockIndex")]
    [HarmonyPostfix]
    static void PostFix(ref int __result)
    {
        Plugin.Log($"{__result}");
    }
}
