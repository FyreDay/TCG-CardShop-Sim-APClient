using HarmonyLib;
using I2.Loc.SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace ApClient.patches;

public class SaveLoadPatches
{
    [HarmonyPatch]
    public class SavePatch {

        static MethodBase TargetMethod()
        {
            var type = typeof(CSaveLoad); // Singleton class, CPlayerData
            var method = type.GetMethod("Save", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static); // Static method

            if (method == null)
            {
                Plugin.Log("Static method 'Save' not found!");
            }

            return method;
        }

        [HarmonyPrefix]
        static bool SavePrefix(int saveSlotIndex, bool skipJSONSave = false)
        {
            Plugin.m_SaveManager.Save(saveSlotIndex);
            return false;
        }
    }
    [HarmonyPatch]
    public class LoadPatch
    {

        static MethodBase TargetMethod()
        {
            var type = typeof(CSaveLoad); // Singleton class, CPlayerData
            var method = type.GetMethod("Load", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static); // Static method

            if (method == null)
            {
                Plugin.Log("Static method 'Load' not found!");
            }

            return method;
        }

        [HarmonyPrefix]
        static bool LoadPrefix(ref bool __result, int slotIndex)
        {
            Plugin.Log("Loading AP save data");
            if (!Plugin.m_SaveManager.Load())
            {
                Plugin.Log("Cannot load Saves for AP Client");
                __result = false;
            }
            __result = true;
            return false;
        }
    }
}
