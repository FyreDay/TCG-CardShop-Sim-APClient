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
            Plugin.Logger.LogInfo("Saving AP save data");
            Plugin.SaveHandler.Save(saveSlotIndex);
            CEventManager.QueueEvent(new CEventPlayer_OnSaveStatusUpdated(isSuccess: true, isAutosaving: false));
            Plugin.Logger.LogInfo("Finished saving AP save data");
            return false;
        }
    }
    [HarmonyPatch(typeof(CSaveLoad), "Load")]
    public class LoadPatch
    {
        [HarmonyPrefix]
        static bool LoadPrefix(ref bool __result, int slotIndex)
        {
            Plugin.Logger.LogInfo("Loading AP save data");
            if (!Plugin.SaveHandler.Load())
            {
                Plugin.Logger.LogError("Cannot load Saves for AP Client");
                __result = false;
                return false;
            }
            //Plugin.SaveHandler.checkWithServer(Plugin.ArchipelagoHandler.GetCheckedLocations());
            __result = true;
            return false;
        }
    }
}
