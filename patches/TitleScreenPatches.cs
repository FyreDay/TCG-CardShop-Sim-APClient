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
            FieldInfo fieldInfo = typeof(TitleScreen).GetField("m_IsOpeningLevel", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo == null)
            {
                return false;
            }


            bool m_IsOpeningLevel = (bool)fieldInfo.GetValue(__instance);

            if (!m_IsOpeningLevel)
            {
                //SoundManager.GenericLightTap();
                fieldInfo.SetValue(__instance, true);
                //SaveLoadGameSlotSelectScreen.OpenScreen(isSaveState: false);
                //SaveLoadGameSlotSelectScreen
                CSingleton<CGameManager>.Instance.m_CurrentSaveLoadSlotSelectedIndex = 3;
                CSingleton<CGameManager>.Instance.m_IsManualSaveLoad = true;
                CSingleton<CGameManager>.Instance.LoadMainLevelAsync("Start", 3);
                ControllerScreenUIExtManager.OnCloseScreen(__instance.m_ControllerScreenUIExtension);
                return true;
            }
            return true;
        }

        [HarmonyPatch("OnPressStartGame")]
        [HarmonyPrefix]
        static bool NewGamePrefix(TitleScreen __instance)
        {
            FieldInfo fieldInfo = typeof(TitleScreen).GetField("m_IsOpeningLevel", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo == null)
            {
                return false;
            }

            bool m_IsOpeningLevel = (bool)fieldInfo.GetValue(__instance);

            if (!m_IsOpeningLevel)
            {
                
                m_IsOpeningLevel = true;
                CSingleton<CGameManager>.Instance.LoadMainLevelAsync("Start");
                ControllerScreenUIExtManager.OnCloseScreen(__instance.m_ControllerScreenUIExtension);
            }
            return false;
        }
    }
}
