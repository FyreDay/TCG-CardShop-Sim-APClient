using ApClient.Archipelago;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ApClient.Patches.Structure;

[HarmonyPatch(typeof(CGameManager), "LoadData")]
public class CGameManagerPatches
{
    [HarmonyPostfix]
    static void PostFix()
    {
        if (CPlayerData.m_HasFinishedTutorial)
        {
            CPlayerData.m_TutorialIndex = 16;
            CSingleton<TutorialManager>.Instance.m_TutorialTargetIndicator.SetActive(value: false);
            CSingleton<TutorialManager>.Instance.gameObject.SetActive(value: false);

        }
    }

}

[HarmonyPatch(typeof(InteractionPlayerController), "OnFinishHideLoadingScreen")]
public class ControllerPatches
{
    [HarmonyPostfix]
    static void OnFinishPostFix(CEventPlayer_FinishHideLoadingScreen evt)
    {
        if (CPlayerData.m_HasFinishedTutorial)
        {
            Plugin.SetSceneLoaded();
        }
        else
        {
            Plugin.SaveHandler.HandleNewGame();
        }
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

    [HarmonyPatch("EvaluateShopLevelAndExp")]
    [HarmonyPostfix]
    static void EvaluatePostFix(GameUIScreen __instance)
    {
        if (!Plugin.IsGameReady())
        {
            return;
        }
        int nextlevel = APLogicUtil.GetNextLevel();
        int licenses_required = APLogicUtil.GetRemainingLicenses(nextlevel);
        if (licenses_required > 0 && CPlayerData.m_ShopLevel + 2 == nextlevel)
        {
            var plural = licenses_required > 1 ? "s" : "";
            LevelLocked.text = $"Level Locked. Find {licenses_required} more License{plural} to levelup";
        }
        else
        {
            LevelLocked.text = "";
        }
    }
}

