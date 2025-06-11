using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApClient.patches
{
    
    [HarmonyPatch(typeof(ShopBuyDecoUIScreen))]
    public class DecoScreenPatches
    {
        //[HarmonyPatch("OnFinishHideLoadingScreen")]
        //[HarmonyPostfix]
        //static void OnFinishPostFix(CEventPlayer_FinishHideLoadingScreen evt)
        //{
        //    Plugin.Log("Processing cache Items");
        //    Plugin.onSceneLoadLogic();

        //}
    }

    [HarmonyPatch(typeof(ShopDecoPanelUI))]
    public class DecoItemPatches
    {
    }

}
