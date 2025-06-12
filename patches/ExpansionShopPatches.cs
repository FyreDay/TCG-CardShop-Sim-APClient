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

            if (Plugin.m_SessionHandler.GetSlotData().AutoRenovate)
            {
                __instance.m_LevelRequirementText.gameObject.SetActive(value: true);
                __instance.m_LevelRequirementText.text = $"Auto Renovate Only";
                if(index < Plugin.m_SessionHandler.itemCount(isShopB ? ExpansionMapping.progressiveB : ExpansionMapping.progressiveA))
                {
                    __instance.m_PurchasedBtn.gameObject.SetActive(value: true);
                    __instance.m_LockPurchaseBtn.gameObject.SetActive(value: false);
                }
                else
                {
                    __instance.m_LockPurchaseBtn.gameObject.SetActive(value: true);
                    __instance.m_PurchasedBtn.gameObject.SetActive(value: false);
                }
                return;
            }

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

            if (!hasAPItem)
            {
                __instance.m_LevelRequirementText.gameObject.SetActive(value: true);
                __instance.m_LevelRequirementText.text = $"Needs Progressive Shop Expansion {(isShopB ? "B" : "A")}";
            }
            else
            {
                __instance.m_LevelRequirementText.text = $"";
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

            __instance.m_LevelRequired = 1;
        }
    }

    [HarmonyPatch(typeof(ExpansionShopPanelUI), "OnPressButton")]
    public class OnClick
    {
        static bool Prefix(ExpansionShopPanelUI __instance)
        {

            bool hasAPItem = false;
            if (__instance.m_IsShopB)
            {
                hasAPItem = Plugin.m_SessionHandler.itemCount(ExpansionMapping.progressiveB) > __instance.m_Index;
            }
            else
            {
                hasAPItem = Plugin.m_SessionHandler.itemCount(ExpansionMapping.progressiveA) > __instance.m_Index;
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
