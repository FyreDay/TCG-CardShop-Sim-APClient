using ApClient.mapping;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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
        static bool Prefix(CEventPlayer_AddShopExp evt)
        {
            //Plugin.Log($"Before Level Up");
            int nextlevel = (((CPlayerData.m_ShopLevel + 1) + 4) / 5) * 5;
            int licenses_required = Plugin.m_SessionHandler.GetRemainingLicenses(nextlevel);
            int maxLevel = nextlevel;
            if (licenses_required <= 0)
            {
                maxLevel = nextlevel + 5;
            }
            oldLevel = CPlayerData.m_ShopLevel;
            if (oldLevel + 2 >= maxLevel)
            {
                if (CPlayerData.m_ShopExpPoint + evt.m_ExpValue >= CPlayerData.GetExpRequiredToLevelUp())
                {
                    int xptonext = CPlayerData.GetExpRequiredToLevelUp() - CPlayerData.m_ShopExpPoint;
                    Plugin.m_SaveManager.IncreaseStoredXP(evt.m_ExpValue);
                    CEventManager.QueueEvent(new CEventPlayer_AddShopExp(xptonext-1));
                    return false;
                }
                return true;
                
            }

            if(Plugin.m_SaveManager.TotalStoredXP() > 1000)
            {
                CEventManager.QueueEvent(new CEventPlayer_AddShopExp(Plugin.m_SaveManager.GetStoredXP(CPlayerData.GetExpRequiredToLevelUp())));
            }
            return true;
        }

        [HarmonyPostfix]
        static void Postfix(CEventPlayer_AddShopExp evt)
        {

            if (oldLevel < CPlayerData.m_ShopLevel && CPlayerData.m_ShopLevel+1 >= 2)
            {
                //Plugin.Log($"Level Up: {oldLevel+1} -> {CPlayerData.m_ShopLevel+1}");
                Plugin.m_SessionHandler.CompleteLocationChecks(LevelMapping.startValue + CPlayerData.m_ShopLevel);
                if(Plugin.m_SessionHandler.GetSlotData().Goal == 0 && CPlayerData.m_ShopLevel +1 >= Plugin.m_SessionHandler.GetSlotData().MaxLevel)
                {
                    Plugin.m_SessionHandler.SendGoalCompletion();
                    PopupTextPatches.ShowCustomText("Congrats! Your Shop Has Leveled To Your Goal!");
                }
            }
        }
    }

    [HarmonyPatch]
    class ReduceCoin
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(CPlayerData), "CPlayer_OnReduceCoin", null, null);
        }
        [HarmonyPrefix]
        static void CPlayer_OnReduceCoin(CEventPlayer_ReduceCoin evt)
        {
            //if(CPlayerData.m_CoinAmount >= 0 && CPlayerData.m_CoinAmount - evt.m_CoinValue < 0)
            //{
            //    Plugin.m_SessionHandler.sendDeath();
            //    Plugin.Log("Died to negative money");
            //}
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
            if (cardData.expansionType != ECardExpansionType.Tetramon && cardData.expansionType != ECardExpansionType.Destiny)
            {
                return;
            }
            Plugin.m_SaveManager.AddOpenedCard(cardData);
        }

        // Postfix: Runs after the method
        static void Postfix(CardData cardData, int addAmount)
        {
            //Plugin.Log($"After adding card: {cardData.monsterType},{cardData.expansionType},{cardData.isDestiny}, Amount: {addAmount}");
        }
    }
}
