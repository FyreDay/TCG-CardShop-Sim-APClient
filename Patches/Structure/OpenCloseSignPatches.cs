using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApClient.patches;

public class OpenCloseSignPatches
{
    [HarmonyPatch(typeof(InteractableOpenCloseSign))]
    public class OverrideTutorialCheck
    {
        [HarmonyPatch("OnMouseButtonUp")]
        [HarmonyPrefix]
        static bool Prefix(InteractableOpenCloseSign __instance)
        {
            if (!__instance.m_IsSwapping)
            {
                CPlayerData.m_IsShopOpen = !CPlayerData.m_IsShopOpen;
                if (!CPlayerData.m_IsShopOnceOpen && CPlayerData.m_IsShopOpen)
                {
                    CPlayerData.m_IsShopOnceOpen = true;
                }

                __instance.m_IsSwapping = true;
                __instance.m_Anim.Play();
                __instance.StartCoroutine(__instance.DelaySwapMesh());
                SoundManager.GenericPop();
            }
            return false;
        }
    }
}
