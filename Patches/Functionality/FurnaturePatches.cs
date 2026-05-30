using ApClient.Archipelago.Mapping;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ApClient.Patches.Functionality;

public class FurnaturePatches
{
    [HarmonyPatch(typeof(FurnitureShopPanelUI), "Init")]
    public class Init
    {

        [HarmonyPostfix]
        static void Postfix(FurnitureShopPanelUI __instance, FurnitureShopUIScreen furnitureShopUIScreen, int index)
        {
            if (!Plugin.IsGameReady())
            {
                return;
            }
            if (FurnatureMapping.Furnature.Count <= index)
            {
                return;
            }
            int numowned = Plugin.ArchipelagoHandler.GetItemCount(FurnatureMapping.Furnature[index].id);
            bool hasAPItem = numowned >= FurnatureMapping.Furnature[index].ProgressiveNum;
            
            if (!hasAPItem)
            {
                __instance.m_LevelRequirementText.text = "License Locked by AP";
                __instance.m_LevelRequirementText.gameObject.SetActive(value: true);
                __instance.m_CompanyTitle.gameObject.SetActive(value: false);
                __instance.m_LockPurchaseBtn.gameObject.SetActive(value: true);
            }
            else
            {
                __instance.m_LevelRequirementText.gameObject.SetActive(value: false);
                __instance.m_CompanyTitle.gameObject.SetActive(value: true);
                __instance.m_LockPurchaseBtn.gameObject.SetActive(value: false);
                __instance.m_LevelRequirementText.text = "";
            }
        }
    }

    [HarmonyPatch(typeof(FurnitureShopPanelUI), "OnPressButton")]
    public class OnClick
    {
        [HarmonyPrefix]
        static bool Prefix(FurnitureShopPanelUI __instance)
        {
            return !__instance.m_LevelRequirementText.text.Equals("License Locked by AP");
        }
    }
}