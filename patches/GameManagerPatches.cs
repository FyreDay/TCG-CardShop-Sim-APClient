using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.SceneManagement;

namespace ApClient.patches;

[HarmonyPatch(typeof(CGameManager))]
public class CGameManagerPatches
{
    [HarmonyPatch("LoadData")]
    [HarmonyPostfix]
    static void PostFix()
    {
        CPlayerData.m_TutorialIndex = 16;
        if (CPlayerData.PlayerName != "My Card Shop" || CPlayerData.m_TutorialIndex > 0)
        {
            
            CSingleton<TutorialManager>.Instance.m_TutorialTargetIndicator.SetActive(value: false);
            CSingleton<TutorialManager>.Instance.gameObject.SetActive(value: false);
            CPlayerData.m_HasFinishedTutorial = true;
        }
        Plugin.Log("Processing cache Items");
        Plugin.ProcessCachedItems();

    }
}
