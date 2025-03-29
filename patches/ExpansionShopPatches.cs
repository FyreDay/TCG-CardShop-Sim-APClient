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
                Plugin.Log($"init expansion Shop {index} is B: {isShopB}");
                Plugin.Log($"Count of progressive A: {Plugin.itemCount(ExpansionMapping.progressiveA)}");
                bool hasAPItem = Plugin.itemCount(ExpansionMapping.progressiveA) >= index+1;
                if (hasAPItem)
                {
                    __instance.m_LockPurchaseBtn.gameObject.SetActive(value: true);
                    __instance.m_PurchasedBtn.gameObject.SetActive(value: false);
                }
                else
                {
                    __instance.m_LockPurchaseBtn.gameObject.SetActive(value: true);
                    __instance.m_PurchasedBtn.gameObject.SetActive(value: false);
                }
                __instance.m_LevelRequirementText.text = $"{__instance.m_LevelRequirementText.text} and AP Progressive";
                
            }
        }

        [HarmonyPatch(typeof(ExpansionShopPanelUI), "OnPressButton")]
        public class OnClick
        {
            static bool Prefix(ExpansionShopPanelUI __instance)
            {
                FieldInfo? fieldInfo = typeof(ExpansionShopPanelUI).GetField("m_Index", BindingFlags.NonPublic | BindingFlags.Instance);
                if (fieldInfo == null)
                {
                    return false;
                }

                int index = (int)fieldInfo.GetValue(__instance);

                bool hasAPItem = Plugin.itemCount(ExpansionMapping.progressiveA) >= index + 1;
                if (hasAPItem)
                {
                    return true;
                }
                NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.UnlockPreviousRoomExpansionFirst);
                return false;
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
