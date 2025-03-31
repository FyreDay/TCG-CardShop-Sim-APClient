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
        CSingleton<TutorialManager>.Instance.gameObject.SetActive(value: false);
        Plugin.ProcessCachedItems();
        
    }
}
