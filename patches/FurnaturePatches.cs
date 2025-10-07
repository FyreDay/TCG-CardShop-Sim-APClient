using ApClient.mapping;
using HarmonyLib;
using I2.Loc;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine.UIElements;

namespace ApClient.patches;

public class FurnaturePatches
{
    
    [HarmonyPatch(typeof(FurnitureShopPanelUI), "Init")]
    public class Init
    {

        [HarmonyPostfix]
        static void Postfix(FurnitureShopPanelUI __instance, FurnitureShopUIScreen furnitureShopUIScreen, int index)
        {
            if (index == 0)
            {
                return;
            }

            
            try
            {
                var value = FurnatureMapping.mapping[__instance.m_Index];

                bool hasAPItem = Plugin.m_SessionHandler.itemCount(value.itemid) >= value.count;

                runFurnatureBtnLogic(__instance, hasAPItem, __instance.m_Index);
            }
            catch
            {
                Plugin.Log($"Failed to find index: {__instance.m_Index}");
            }
        }

    }
    public static void runFurnatureBtnLogic(FurnitureShopPanelUI __instance, bool hasItem, int index)
    {

        

        if (!hasItem)
        {
            //disable buying until AP item
            __instance.m_LevelRequirementText.text = "License Locked by AP";
            __instance.m_LevelRequirementText.gameObject.SetActive(value: true);
            __instance.m_CompanyTitle.gameObject.SetActive(value: false);
            __instance.m_LockPurchaseBtn.gameObject.SetActive(value: true);
            //Plugin.Log($"Doesnt have item furnature. {__instance.m_LevelRequirementText.text}");
        }
        else
        {
            //remove level requirement
            EnableFurnature(__instance, index);
            __instance.m_LevelRequirementText.text = "";
            //Plugin.Log($"remove level Requirement for furnature {index}");
        }
    }

    public static void EnableFurnature(FurnitureShopPanelUI __instance, int index)
    {
        __instance.m_LevelRequired = 1;
        __instance.m_LevelRequirementText.gameObject.SetActive(value: false);
        __instance.m_CompanyTitle.gameObject.SetActive(value: true);
        __instance.m_LockPurchaseBtn.gameObject.SetActive(value: false);
    }


    [HarmonyPatch(typeof(FurnitureShopPanelUI), "OnPressButton")]
    public class OnClick
    {
        // Prefix: Runs before the method
        static bool Prefix(FurnitureShopPanelUI __instance)
        {
            FieldInfo fieldInfoi = typeof(FurnitureShopPanelUI).GetField("m_Index", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfoi == null)
            {
                return false;
            }

            int index = (int)fieldInfoi.GetValue(__instance);

            if (__instance.m_LevelRequirementText.text.Equals("License Locked by AP"))
            {
                return false;
            }

            //Plugin.Log($"Click Furnature Button: index {index}, Type: {InventoryBase.GetFurniturePurchaseData(index).GetName()}");
            return true;
        }
    }
}
