using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApClient.Patches.Functionality;

public class PLayerCardUICatches
{

    [HarmonyPatch(typeof(PlayCardSetUI))]
    public static class PlayCardSetUIPatch
    {
        [HarmonyFinalizer]
        [HarmonyPatch("Update")]
        [HarmonyPatch("LateUpdate")]
        public static Exception Finalizer(Exception __exception)
        {
            if (__exception is NullReferenceException)
            {
                return null;
            }

            return __exception; 
        }
    }
}
