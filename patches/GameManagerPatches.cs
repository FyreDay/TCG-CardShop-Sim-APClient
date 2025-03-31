using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApClient.patches;

[HarmonyPatch(typeof(CGameManager))]
public class CGameManagerPatches
{
    [HarmonyPatch("LoadData")]
    [HarmonyPostfix]
    static void LoadDataPostFix(int ___m_CurrentSaveLoadSlotSelectedIndex)
    {
        Plugin.ProcessCachedItems();
    }
}
