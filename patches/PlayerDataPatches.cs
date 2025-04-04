using ApClient.mapping;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine.UIElements;

namespace ApClient.patches
{
    //ShopRenamer -> OnPressConfirmShopName() Postfix
    class PlayerDataPatches
    {
        [HarmonyPatch]
        class CreateData
        {
            static MethodBase TargetMethod()
            {
                return AccessTools.Method(typeof(CPlayerData), "CreateDefaultData", null, null);
            }

            [HarmonyPostfix]
            public static void Postfix(CPlayerData __instance)
            {
                //TutorialManager
                //UnityEngine
                //CPlayerData.m_TutorialIndex = 16;
                //CPlayerData.m_HasFinishedTutorial = true;
                Plugin.Log("Postfix executed on CreateDefaultData");
                CPlayerData.m_IsItemLicenseUnlocked[0] = false;
            }
        }

        [HarmonyPatch]
        class AddXp
        {
            static MethodBase TargetMethod()
            {
                return AccessTools.Method(typeof(CPlayerData), "CPlayer_OnAddShopExp", null, null);
            }

            private static int oldLevel;
            [HarmonyPrefix]
            static void Prefix(CEventPlayer_AddShopExp evt)
            {
                //Plugin.Log($"Before Level Up");
                oldLevel = CPlayerData.m_ShopLevel;
            }

            [HarmonyPostfix]
            static void Postfix(CEventPlayer_AddShopExp evt)
            {

                if (oldLevel < CPlayerData.m_ShopLevel && CPlayerData.m_ShopLevel+1 >= 2)
                {
                    Plugin.Log($"Level Up: {oldLevel+1} -> {CPlayerData.m_ShopLevel+1}");
                    Plugin.session.Locations.CompleteLocationChecks(LevelMapping.startValue + CPlayerData.m_ShopLevel);
                    if(Plugin.Goal == 1 && CPlayerData.m_ShopLevel +1 >= Plugin.LevelGoal)
                    {
                        Plugin.session.SetGoalAchieved();
                    }
                }
            }
        }

        [HarmonyPatch]
        class AddLicense
        {
            static MethodBase TargetMethod()
            {
                var type = typeof(CPlayerData); // Singleton class, CPlayerData
                var method = type.GetMethod("SetUnlockItemLicense", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static); // Static method

                if (method == null)
                {
                    Plugin.Log("Static method 'SetUnlockItemLicense' not found!");
                }

                return method;
            }
            // Prefix: Runs before the method
            static void Prefix(int index)
            {
                
                Plugin.Log($"Before adding license: {index}, Type: {InventoryBase.GetRestockData(index).itemType}");
                //Plugin.Log($"id: {LicenseMapping.mapping.GetValueOrDefault(index)}");
                Plugin.session.Locations.CompleteLocationChecks(LicenseMapping.mapping.GetValueOrDefault(index).locid);
                //return Plugin.hasItem(LicenseMapping.mapping.GetValueOrDefault(index).itemid);
            }

            // Postfix: Runs after the method
            static void Postfix(int index)
            {
                Plugin.Log($"After adding license:" );
            }
        }

        [HarmonyPatch]
        class AddCard
        {
            // Dynamically target the method since 'S' might be a static instance
            static MethodBase TargetMethod()
            {
                var type = typeof(CPlayerData); // Singleton class, CPlayerData
                var method = type.GetMethod("AddCard", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static); // Static method

                if (method == null)
                {
                    Plugin.Log("Static method 'AddCard' not found!");
                }

                return method;
            }

            // Prefix: Runs before the method
            static void Prefix(CardData cardData, int addAmount)
            {
                if(Plugin.CardSanity == 0)
                {
                    return;
                }

                ECollectionPackType expansionType = (ECollectionPackType)AccessTools.Field(typeof(CardOpeningSequence), "m_CollectionPackType").GetValue(CSingleton<CardOpeningSequence>.Instance);

                //Plugin.Log($"Is new: {CPlayerData.GetCardAmount(cardData) == 0} and Expansion: {(int)expansionType}");
                if((int)expansionType < Plugin.CardSanity && CPlayerData.GetCardAmount(cardData) == 0)
                {
                    Plugin.session.Locations.CompleteLocationChecks(CardMapping.getId(cardData));
                }
                //Plugin.Log($"Before adding card: {newList[counter]}, Amount: {addAmount}");
            }

            // Postfix: Runs after the method
            static void Postfix(CardData cardData, int addAmount)
            {
                //Plugin.Log($"After adding card: {cardData.monsterType},{cardData.expansionType},{cardData.isDestiny}, Amount: {addAmount}");
            }
        }
    }
}
