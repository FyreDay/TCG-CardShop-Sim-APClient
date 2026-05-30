using HarmonyLib;
using I2.Loc.SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace ApClient.Patches.Structure;

public class SaveLoadPatches
{
    [HarmonyPatch(typeof(CSaveLoad), "Save")]
    public class SavePatch {

        [HarmonyPrefix]
        static bool SavePrefix(int saveSlotIndex, bool skipJSONSave = false)
        {
            Plugin.SaveHandler.Save(saveSlotIndex);
            return false;
        }
    }
    [HarmonyPatch(typeof(CSaveLoad), "Load")]
    public class LoadPatch
    {
        [HarmonyPrefix]
        static bool LoadPrefix(ref bool __result, int slotIndex)
        {
            if (!Plugin.SaveHandler.Load())
            {
                Plugin.Logger.LogError("Failed to load save. Trying backup File");
                if (!Plugin.SaveHandler.LoadBackup())
                {
                    Plugin.Logger.LogError("Failed to load backup save. Giving up");
                    __result = false;
                    return false;
                }
            }
            //Plugin.SaveHandler.checkWithServer(Plugin.ArchipelagoHandler.GetCheckedLocations());
            __result = true;
            return false;
        }
    }
}
