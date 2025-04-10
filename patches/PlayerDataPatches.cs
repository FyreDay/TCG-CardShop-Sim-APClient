﻿using ApClient.mapping;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine.UIElements;

namespace ApClient.patches;

class PlayerDataPatches
{
    public static List<int> GetValidTypeIdsForSanity()
    {   
        if(Plugin.m_SessionHandler.GetSlotData().CardSanity == 0)
        {
            return Enumerable.Range(1, 121).ToList();
        }

        HashSet<int> uniqueIds = new HashSet<int>();
        ECardExpansionType[] cardExpansionTypes = [ECardExpansionType.Tetramon, ECardExpansionType.Destiny];

        foreach (ECardExpansionType cardExpansionType in cardExpansionTypes)
        {
            for (int i = 0; i < InventoryBase.GetShownMonsterList(cardExpansionType).Count; i++)
            {
                EMonsterType mType = InventoryBase.GetMonsterData(InventoryBase.GetShownMonsterList(cardExpansionType)[i]).MonsterType;
                ERarity rarity = InventoryBase.GetMonsterData(InventoryBase.GetShownMonsterList(cardExpansionType)[i]).Rarity;
                switch (rarity)
                {
                    case ERarity.Legendary:
                        if ((Plugin.m_SessionHandler.GetSlotData().CardSanity >= 8 && cardExpansionType == ECardExpansionType.Destiny)
                            || (Plugin.m_SessionHandler.GetSlotData().CardSanity >= 4 && cardExpansionType == ECardExpansionType.Tetramon))
                        {
                            uniqueIds.Add((int)mType);
                        }
                        break;
                    case ERarity.Epic:
                        if ((Plugin.m_SessionHandler.GetSlotData().CardSanity >= 7 && cardExpansionType == ECardExpansionType.Destiny)
                            || (Plugin.m_SessionHandler.GetSlotData().CardSanity >= 3 && cardExpansionType == ECardExpansionType.Tetramon))
                        {
                            uniqueIds.Add((int)mType);
                        }
                        break;
                    case ERarity.Rare:
                        if ((Plugin.m_SessionHandler.GetSlotData().CardSanity >= 6 && cardExpansionType == ECardExpansionType.Destiny)
                            || (Plugin.m_SessionHandler.GetSlotData().CardSanity >= 2 && cardExpansionType == ECardExpansionType.Tetramon))
                        {
                            uniqueIds.Add((int)mType);
                        }
                        break;
                    default:
                        if ((Plugin.m_SessionHandler.GetSlotData().CardSanity >= 5 && cardExpansionType == ECardExpansionType.Destiny)
                            || (Plugin.m_SessionHandler.GetSlotData().CardSanity >= 1 && cardExpansionType == ECardExpansionType.Tetramon))
                        {
                            uniqueIds.Add((int)mType);
                        }
                        break;
                }
            }
        }
        return uniqueIds.ToList();
    }

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
            Plugin.m_SaveManager.setIncompleteCards(GetValidTypeIdsForSanity());
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
                Plugin.m_SessionHandler.CompleteLocationChecks(LevelMapping.startValue + CPlayerData.m_ShopLevel);
                if(Plugin.m_SessionHandler.GetSlotData().Goal == 1 && CPlayerData.m_ShopLevel +1 >= Plugin.m_SessionHandler.GetSlotData().LevelGoal)
                {
                    Plugin.m_SessionHandler.SendGoalCompletion();
                    PopupTextPatches.ShowCustomText("Congrats! Your Shop Has Leveled To Your Goal!");
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
            
            //Plugin.Log($"Before adding license: {index}, Type: {InventoryBase.GetRestockData(index).itemType}");
            //Plugin.Log($"id: {LicenseMapping.mapping.GetValueOrDefault(index)}");
            //Plugin.session.Locations.CompleteLocationChecks(LicenseMapping.mapping.GetValueOrDefault(index).locid);
            //return Plugin.hasItem(LicenseMapping.mapping.GetValueOrDefault(index).itemid);
        }

        // Postfix: Runs after the method
        static void Postfix(int index)
        {
            //Plugin.Log($"After adding license:" );
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
            if(Plugin.m_SessionHandler.GetSlotData().CardSanity == 0)
            {
                return;
            }

            ECollectionPackType expansionType = (ECollectionPackType)AccessTools.Field(typeof(CardOpeningSequence), "m_CollectionPackType").GetValue(CSingleton<CardOpeningSequence>.Instance);

            //Plugin.Log($"Is new: {CPlayerData.GetCardAmount(cardData) == 0} and Expansion: {(int)expansionType}");
            if((int)expansionType < Plugin.m_SessionHandler.GetSlotData().CardSanity 
                && CPlayerData.GetCardAmount(cardData) == 0 
                && (int)cardData.borderType <= Plugin.m_SessionHandler.GetSlotData().BorderInSanity
                && (!cardData.isFoil || Plugin.m_SessionHandler.GetSlotData().FoilInSanity))
            {
                Plugin.m_SessionHandler.CompleteLocationChecks(CardMapping.getId(cardData));
            }
        }

        // Postfix: Runs after the method
        static void Postfix(CardData cardData, int addAmount)
        {
            //Plugin.Log($"After adding card: {cardData.monsterType},{cardData.expansionType},{cardData.isDestiny}, Amount: {addAmount}");
        }
    }
}
