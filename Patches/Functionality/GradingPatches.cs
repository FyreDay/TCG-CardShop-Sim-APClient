using ApClient.mapping;
using HarmonyLib;
using I2.Loc;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApClient.Patches.Functionality;

public class GradingPatches
{
    [HarmonyPatch(typeof(MonsterData_ScriptableObject), "GetGradeCardServiceData")]
    public class ChangeGradingDays
    {
        [HarmonyPostfix]
        static void Postfix(MonsterData_ScriptableObject __instance, GradeCardServiceData __result)
        {
            __result.m_ServiceDays = 1;
        }
    }

    

    [HarmonyPatch(typeof(GradeCardWebsiteUIScreen), "OnOpenScreen")]
    public class OpenScreen
    {
        [HarmonyPostfix]
        static void Postfix(GradeCardWebsiteUIScreen __instance)
        {
            if (__instance is GradeCardWebsiteUIScreen && 
                Plugin.ArchipelagoHandler.slotData.GradingLocked && 
                Plugin.ArchipelagoHandler.GetItemCount(GenericItemMapping.GRADING_UNLOCK) == 0)
            {
                PopupTextPatches.ShowCustomText($"Need to find the 'Grading Unlock' Item");
                Util.RunOnMainThread(() => __instance.CloseScreen());
                return;

            }
        }
    }
}
