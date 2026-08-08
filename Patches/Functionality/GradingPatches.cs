using HarmonyLib;
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
}
