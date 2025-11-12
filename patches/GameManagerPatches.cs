using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;
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
    }
    
}

[HarmonyPatch(typeof(InteractionPlayerController))]
public class ControllerPatches
{
    [HarmonyPatch("OnFinishHideLoadingScreen")]
    [HarmonyPostfix]
    static void OnFinishPostFix(CEventPlayer_FinishHideLoadingScreen evt)
    {
        Plugin.Log("Processing cache Items");
        Plugin.onSceneLoadLogic();

    }
}

 [HarmonyPatch(typeof(GameUIScreen))]
public class GameUIScreenPatches
{
    private static TextMeshProUGUI LevelLocked;
    
    [HarmonyPatch("Awake")]
    [HarmonyPostfix]
    static void AwakePostFix(GameUIScreen __instance)
    {
        
        LevelLocked = GameObject.Instantiate(__instance.m_ShopLevelText, __instance.m_ShopLevelText.transform.parent);
        LevelLocked.rectTransform.anchoredPosition += new Vector2(-400, 0);
        LevelLocked.color = Color.red;
        LevelLocked.text = "";
        LevelLocked.enableWordWrapping = false;
        LevelLocked.overflowMode = TextOverflowModes.Overflow;
        LevelLocked.enableAutoSizing = false;

        

    }
    public static int ExactMaxLevel = 5;

    [HarmonyPatch("EvaluateShopLevelAndExp")]
    [HarmonyPostfix]
    static void EvaluatePostFix(GameUIScreen __instance)
    {
        int nextlevel = (((CPlayerData.m_ShopLevel + 1) + 4) / 5) *5;
        int licenses_required = Plugin.m_SessionHandler.GetRemainingLicenses(nextlevel);
        if (licenses_required > 0 && CPlayerData.m_ShopLevel + 2 == nextlevel)
        {
            LevelLocked.text = $"Level Locked. Find {licenses_required} more Licenses to levelup";
        }
        else
        {
            while (licenses_required <= 0 && !(nextlevel > Plugin.m_SessionHandler.GetSlotData().MaxLevel))
            {
                nextlevel += 5;
                licenses_required = Plugin.m_SessionHandler.GetRemainingLicenses(nextlevel);
                ExactMaxLevel = nextlevel;
            }

            LevelLocked.text = "";
        }
    }
}

