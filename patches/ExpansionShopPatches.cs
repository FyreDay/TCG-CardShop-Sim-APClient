using HarmonyLib;
using System;
using System.Collections.Generic;
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
