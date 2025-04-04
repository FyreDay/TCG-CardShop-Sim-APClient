﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine.SceneManagement;

namespace ApClient.patches
{
    public class TutorialPatches
    {
        [HarmonyPatch(typeof(ShopRenamer))]
        public class ShopRenamerPatches
        {
            [HarmonyPatch("OnPressConfirmShopName")]
            [HarmonyPostfix]
            static void PostFix(ShopRenamer __instance)
            {

                CPlayerData.m_TutorialIndex = 1;
                
                CSingleton<TutorialManager>.Instance.m_TutorialTargetIndicator.SetActive(value: false);
                CSingleton<TutorialManager>.Instance.gameObject.SetActive(value: false);
                CPlayerData.m_TutorialIndex = 16;
                CPlayerData.m_HasFinishedTutorial = true;
                Plugin.ProcessCachedItems();

            }
        }
    }
}
