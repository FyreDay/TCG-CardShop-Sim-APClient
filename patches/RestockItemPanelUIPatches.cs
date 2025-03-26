using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApClient.patches
{
    public class RestockItemPanelUIPatches
    {
        [HarmonyPatch(typeof(RestockItemPanelUI), "OnPressButtion")]
        public class OnClick
        {

        }
    }
}
