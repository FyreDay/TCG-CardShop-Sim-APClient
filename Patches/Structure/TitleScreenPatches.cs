using ApClient.Archipelago;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ApClient.patches
{
    [HarmonyPatch(typeof(TitleScreen))]
    public class TitleScreenPatches
    {
        [HarmonyPatch("OpenLoadGameSlotScreen")]
        [HarmonyPrefix]
        static bool LoadPrefix(TitleScreen __instance)
        {

            if (!__instance.m_IsOpeningLevel)
            {
                __instance.m_IsOpeningLevel = true;
                CSingleton<CGameManager>.Instance.m_CurrentSaveLoadSlotSelectedIndex = Constants.SAVE_SLOT;
                CSingleton<CGameManager>.Instance.m_IsManualSaveLoad = true;
                CSingleton<CGameManager>.Instance.LoadMainLevelAsync("Start", Constants.SAVE_SLOT);
                ControllerScreenUIExtManager.OnCloseScreen(__instance.m_ControllerScreenUIExtension);
                return true;
            }
            return true;
        }

        [HarmonyPatch("OnPressStartGame")]
        [HarmonyPrefix]
        static bool NewGamePrefix(TitleScreen __instance)
        {
            if (!__instance.m_IsOpeningLevel)
            {

                __instance.m_IsOpeningLevel = true;
                CSingleton<CGameManager>.Instance.LoadMainLevelAsync("Start");
                ControllerScreenUIExtManager.OnCloseScreen(__instance.m_ControllerScreenUIExtension);
            }
            return false;
        }
    }
}
