using HarmonyLib;
using System;
using System.Collections.Generic;
using BepInEx.Logging;
using System.Text;
using System.Reflection;

namespace ApClient.patches
{
    
    public class ExpansionShopPatches
    {
        [HarmonyPatch(typeof(ExpansionShopPanelUI), "Init")]
        public class Init
        {
            static void Postfix(ExpansionShopPanelUI __instance, ExpansionShopUIScreen expansionShopUIScreen, int index, bool isShopB)
            {
                __instance.m_LevelRequirementText.text = $"{__instance.m_LevelRequirementText.text} and AP Progressive";
            }
        }
        [HarmonyPatch(typeof(ExpansionShopPanelUI), "OnPressButton")]
        public class OnClick
        {
            static bool Prefix()
            {
                NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.UnlockPreviousRoomExpansionFirst);
                return false;
            }
        }
    }
}
