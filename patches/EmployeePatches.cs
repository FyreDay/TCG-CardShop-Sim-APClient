using ApClient.mapping;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ApClient.patches
{
    public class EmployeePatches
    {
        [HarmonyPatch(typeof(HireWorkerPanelUI), "Init")]
        public class Init
        {
            static void Postfix(HireWorkerPanelUI __instance, HireWorkerScreen hireWorkerScreen, int index)
            {
                var val = EmployeeMapping.mapping.GetValueOrDefault<int, (int itemid, string name, int locid)>(index, (-1, "", -1));
                __instance.m_LevelRequirementText.text = $"{__instance.m_LevelRequirementText.text} Or AP Progressive";
                if (val.locid != -1 && Plugin.hasItem(val.itemid))
                {
                    __instance.m_LevelRequirementText.gameObject.SetActive(value: false);
                    __instance.m_HireFeeText.gameObject.SetActive(value: true);
                    __instance.m_LockPurchaseBtn.gameObject.SetActive(value: false);
                }
                else {
                    __instance.m_LevelRequirementText.gameObject.SetActive(value: true);
                    __instance.m_HireFeeText.gameObject.SetActive(value: false);
                    __instance.m_LockPurchaseBtn.gameObject.SetActive(value: true);
                }
                
            }
        }

        [HarmonyPatch(typeof(HireWorkerPanelUI), "OnPressHireButton")]
        public class OnClick
        {
            static bool Prefix(HireWorkerPanelUI __instance)
            {
                FieldInfo? fieldInfo = typeof(HireWorkerPanelUI).GetField("m_Index", BindingFlags.NonPublic | BindingFlags.Instance);
                if (fieldInfo == null)
                {
                    return false;
                }

                int index = (int)fieldInfo.GetValue(__instance);
                var val = EmployeeMapping.mapping.GetValueOrDefault<int, (int itemid, string name, int locid)>(index, (-1, "", -1));
                if (val.locid != -1 && Plugin.hasItem(val.itemid))
                {
                    return true;
                }
                return false;
            }
        }
    }
}
