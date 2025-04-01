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
        
        Plugin.Log("Processing cache Items");
        CSingleton<TutorialManager>.Instance.m_TutorialTargetIndicator.SetActive(value: false);
        CSingleton<TutorialManager>.Instance.gameObject.SetActive(value: false);
        CPlayerData.m_HasFinishedTutorial = true;
        //CPlayerData.m_TutorialIndex = 16;
        Plugin.ProcessCachedItems();
        
    }
}
