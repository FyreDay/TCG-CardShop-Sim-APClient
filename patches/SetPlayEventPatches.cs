using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApClient.patches;

public class SetPlayEventPatches
{
    [HarmonyPatch(typeof(SetGameEventFormatScreen), nameof(SetGameEventFormatScreen.EvaluateText))]
    public static class setScreen
    {
        [HarmonyPostfix]
        static void Postfix(SetGameEventFormatScreen __instance, EGameEventFormat gameEventFormat)
        {
            if (!Plugin.m_SessionHandler.isEventUnlocked(gameEventFormat)){
                __instance.m_PlayCountRequired.text = "AP Item Not Found";
                __instance.m_LockedGrp.SetActive(value: true);
                __instance.m_ConfirmButton.interactable = false;
            }
        }
    }
}
