using HarmonyLib;
using System;
using System.Collections.Generic;
using BepInEx.Logging;
using System.Text;
using System.Reflection;
using ApClient.mapping;

namespace ApClient.patches;


public class ExpansionShopPatches
{
    [HarmonyPatch(typeof(ExpansionShopPanelUI), "Init")]
    public class Init
    {

        [HarmonyPostfix]
        static void Postfix(ExpansionShopPanelUI __instance, ExpansionShopUIScreen expansionShopUIScreen, int index, bool isShopB)
        {
            if(!CPlayerData.m_IsWarehouseRoomUnlocked && isShopB)
            {
                __instance.m_LockPurchaseBtn.gameObject.SetActive(value: false);
                __instance.m_PurchasedBtn.gameObject.SetActive(value: false);
            }
            //Plugin.Log($"init expansion Shop {index} is B: {isShopB}");
            //Plugin.Log($"Count of progressive A: {Plugin.itemCount(ExpansionMapping.progressiveA)}");

            bool hasAPItem = false;
            if (isShopB)
            {
                hasAPItem = Plugin.m_SessionHandler.itemCount(ExpansionMapping.progressiveB) > index;
            }
            else
            {
                hasAPItem = Plugin.m_SessionHandler.itemCount(ExpansionMapping.progressiveA) > index;
            }

            bool atLevel = true;// CPlayerData.m_ShopLevel + 1 >= m_LevelRequired;

            if (!hasAPItem && atLevel)
            {
                __instance.m_LevelRequirementText.gameObject.SetActive(value: true);
                __instance.m_LevelRequirementText.text = $"Needs Progressive Shop Expansion {(isShopB ? "B" : "A")}";
            }
            else if (!hasAPItem)
            {
                __instance.m_LevelRequirementText.text = $"AP Progressive {(isShopB ? "B" : "A")}";
            }
            if (isShopB && CPlayerData.m_UnlockWarehouseRoomCount > index)
            {
                __instance.m_LockPurchaseBtn.gameObject.SetActive(value: false);
                __instance.m_PurchasedBtn.gameObject.SetActive(value: true);
            }
            else if (!isShopB && CPlayerData.m_UnlockRoomCount > index)
            {
                __instance.m_LockPurchaseBtn.gameObject.SetActive(value: false);
                __instance.m_PurchasedBtn.gameObject.SetActive(value: true);
            }
            else if (hasAPItem && atLevel)
            {
                __instance.m_LockPurchaseBtn.gameObject.SetActive(value: false);
                __instance.m_PurchasedBtn.gameObject.SetActive(value: false);
            }
            else
            {
                __instance.m_LockPurchaseBtn.gameObject.SetActive(value: true);
                __instance.m_PurchasedBtn.gameObject.SetActive(value: false);
            }
            
            
        }
    }

    [HarmonyPatch(typeof(ExpansionShopPanelUI), "OnPressButton")]
    public class OnClick
    {
        static bool Prefix(ExpansionShopPanelUI __instance)
        {
            FieldInfo fieldInfo = typeof(ExpansionShopPanelUI).GetField("m_Index", BindingFlags.NonPublic | BindingFlags.Instance);

            int index = (int)fieldInfo.GetValue(__instance);

            FieldInfo fieldInfo2 = typeof(ExpansionShopPanelUI).GetField("m_LevelRequired", BindingFlags.NonPublic | BindingFlags.Instance);


            int m_LevelRequired = (int)fieldInfo2.GetValue(__instance);

            FieldInfo m_IsShopBinfo = AccessTools.Field(typeof(ExpansionShopPanelUI), "m_IsShopB");
            
            bool m_IsShopB = (bool)m_IsShopBinfo.GetValue(__instance);

            bool hasAPItem = false;
            if (m_IsShopB)
            {
                hasAPItem = Plugin.m_SessionHandler.itemCount(ExpansionMapping.progressiveB) > index;
            }
            else
            {
                hasAPItem = Plugin.m_SessionHandler.itemCount(ExpansionMapping.progressiveA) > index;
            }
            bool atLevel = true; // (CPlayerData.m_ShopLevel + 1) >= m_LevelRequired;
            //Plugin.Log($"is B? {m_IsShopB} has progressive {hasAPItem} with count {Plugin.itemCount(ExpansionMapping.progressiveA)} at level {atLevel} required {m_LevelRequired}" );
            if (hasAPItem && atLevel)
            {
                
                return true;
            }
            //NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.UnlockPreviousRoomExpansionFirst);
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
            //TODO: SETUP SHOP EXPANSION LOCATIONS
            //Plugin.m_SessionHandler.CompleteLocationChecks(ExpansionMapping.locstartval + index + (isShopB ? 30 : 0));
        }
    }
    [HarmonyPatch]
    class NoUnlockB
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(ExpansionShopUIScreen), "OnPressUnlockShopB", null, null);
        }

        [HarmonyPrefix]
        public static bool Prefix()
        {
            return false;
        }
    }


    [HarmonyPatch]
    class PanelUI
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(ExpansionShopUIScreen), "EvaluateShopPanelUI", null, null);
        }   

        [HarmonyPostfix]
        public static void Postfix(ExpansionShopUIScreen __instance)
        {
            if (__instance.m_IsShopB)
            {
                if (!CPlayerData.m_IsWarehouseRoomUnlocked)
                {
                    __instance.m_ShopB_LevelRequirementText.enabled = true;
                    __instance.m_ShopB_LevelRequirementText.text = "Needs AP Warehouse Key";
                    __instance.m_ShopB_LockPurchaseBtn.SetActive(value: true);
                }
            }
        }
    }
}
