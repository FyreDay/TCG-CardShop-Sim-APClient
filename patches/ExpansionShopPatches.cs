using HarmonyLib;
using System;
using System.Collections.Generic;
using BepInEx.Logging;
using System.Text;
using System.Reflection;
using ApClient.mapping;

namespace ApClient.patches
{
    
    public class ExpansionShopPatches
    {
        [HarmonyPatch(typeof(ExpansionShopPanelUI), "Init")]
        public class Init
        {
            static void Postfix(ExpansionShopPanelUI __instance, ExpansionShopUIScreen expansionShopUIScreen, int index, bool isShopB)
            {
                __instance.m_LevelRequirementText.text = $"{__instance.m_LevelRequirementText.text} and AP Progressive";
                __instance.m_LockPurchaseBtn.gameObject.SetActive(value: true);
            }
        }

        [HarmonyPatch(typeof(ExpansionShopPanelUI), "OnPressButton")]
        public class OnClick
        {
            static bool Prefix(ExpansionShopPanelUI __instance)
            {
                NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.UnlockPreviousRoomExpansionFirst);
                return true;
            }
        }

        [HarmonyPatch]
        class CreateData
        {
            static MethodBase TargetMethod()
            {
                return AccessTools.Method(typeof(ExpansionShopUIScreen), "EvaluateCartCheckout", null, null);
            }

            [HarmonyPrefix]
            public static void Prefix(float totalCost, int index, bool isShopB)
            {
                Plugin.session.Locations.CompleteLocationChecks(ExpansionMapping.locstartval + index + (isShopB ? 30 : 0));
                Plugin.Log("Prefix executed on EvaluateCartCheckout for Expansion");
            }
        }
    }
}
