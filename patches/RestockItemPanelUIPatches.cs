using ApClient.mapping;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ApClient.patches
{
    public class RestockItemPanelUIPatches
    {
        [HarmonyPatch(typeof(RestockItemPanelUI), "Init")]
        public class Init
        {
            static void Postfix(RestockItemPanelUI __instance, RestockItemScreen restockItemScreen, int index)
            {

                var value = LicenseMapping.getValueOrEmpty(index);
                if (value.locid == -1)
                {
                    Plugin.Log($"Failed to find index: {index}");
                    return;
                }

                bool hasAPItem = Plugin.itemCount(value.itemid) >= value.count;

                runLicenseBtnLogic(__instance, hasAPItem, index);


            }
                
        }

        public static void runLicenseBtnLogic(RestockItemPanelUI __instance, bool hasItem, int index)
        {
            bool hasCheck = CPlayerData.m_IsItemLicenseUnlocked.Count > index && CPlayerData.GetIsItemLicenseUnlocked(index);

            FieldInfo fieldInfo = typeof(RestockItemPanelUI).GetField("m_LevelRequired", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo == null)
            {
                return;
            }

            int levelRequirement = (int)fieldInfo.GetValue(__instance);

            if (!hasItem && hasCheck)
            {
                //disable buying until AP item
                __instance.m_UIGrp.SetActive(value: false);
                __instance.m_LicenseUIGrp.SetActive(value: true);
                __instance.m_LockPurchaseBtn.gameObject.SetActive(value: true);
                __instance.m_LevelRequirementText.text = "License Locked by AP";
                __instance.m_LevelRequirementText.gameObject.SetActive(value: true);
                //Plugin.Log($"Doesnt have item. {__instance.m_LevelRequirementText.text}");
            }
            else if (hasItem && hasCheck)
            {
                //allow ordering the item
                __instance.m_UIGrp.SetActive(value: true);
                __instance.m_LicenseUIGrp.SetActive(value: false);
                __instance.m_LockPurchaseBtn.gameObject.SetActive(value: false);
                //__instance.m_LevelRequirementText.text = "";
                //__instance.m_LevelRequirementText.gameObject.SetActive(value: false);
                //Plugin.Log($"Has item.");
            }
            else if (hasItem && !hasCheck)
            {
                //remove level requirement
                fieldInfo.SetValue(__instance, 1);
                __instance.m_LockPurchaseBtn.SetActive(value: false);
                __instance.m_LevelRequirementText.text = "AP Removed Level Requirement";
                //Plugin.Log($"remove level Requirement for {index}");
            }
        }


        [HarmonyPatch(typeof(RestockItemPanelUI), "OnPressPurchaseButton")]
        public class OnClick
        {
            // Prefix: Runs before the method
            static bool Prefix(RestockItemPanelUI __instance)
            {
                if (__instance.m_LevelRequirementText.text.Equals("License Locked by AP"))
                {
                    return false;
                }
                var IBase = InventoryBase.Instance;
                Plugin.Log($"Click Button: index {__instance.GetIndex()}, Type: {InventoryBase.GetRestockData((int)__instance.GetIndex()).itemType}");
                return true;
            }

            // Postfix: Runs after the method
            static void Postfix(RestockItemPanelUI __instance)
            {
                FieldInfo fieldInfo = typeof(RestockItemPanelUI).GetField("m_LevelRequired", BindingFlags.NonPublic | BindingFlags.Instance);
                if (fieldInfo != null)
                {
                    int levelRequirement = (int)fieldInfo.GetValue(__instance);
                    if (CPlayerData.m_ShopLevel + 1 < levelRequirement)
                    {
                        return;
                    }
                }
                
                if (!Plugin.hasItem(LicenseMapping.mapping.GetValueOrDefault((int)__instance.GetIndex()).itemid))
                {
                    __instance.m_UIGrp.SetActive(value: false);
                    __instance.m_LicenseUIGrp.SetActive(value: true);
                    __instance.m_LockPurchaseBtn.gameObject.SetActive(value: true);
                    __instance.m_LevelRequirementText.text = "License Locked by AP";
                    __instance.m_LevelRequirementText.gameObject.SetActive(value: true);
                    Plugin.Log($"Doesnt have item. {__instance.m_LevelRequirementText.text}");
                }
                
            }
        }
    }
}
