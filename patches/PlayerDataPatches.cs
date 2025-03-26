using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ApClient.patches
{

    [HarmonyPatch]
    class PlayerDataPatches
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
            Plugin.Log($"Before adding card: {cardData.isNew}, Amount: {addAmount}");
        }

        // Postfix: Runs after the method
        static void Postfix(CardData cardData, int addAmount)
        {
            Plugin.Log($"After adding card: {cardData.monsterType},{cardData.expansionType},{cardData.isDestiny}, Amount: {addAmount}");
        }
    }
}
