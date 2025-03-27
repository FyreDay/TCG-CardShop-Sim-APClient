using HarmonyLib;
using System;
using System.Collections.Generic;
using BepInEx.Logging;
using System.Text;

namespace ApClient.patches
{
    
    public class ExpansionShopPatches
    {
        [HarmonyPatch(typeof(ExpansionShopPanelUI), "OnPressButtion")]
        public class OnClick
        {
            
        }
    }
}
