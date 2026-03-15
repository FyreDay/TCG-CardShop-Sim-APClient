using ApClient.Archipelago;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ApClient.patches
{
    [HarmonyPatch(typeof(PauseScreen))]
    public class PauseScreenPatches
    {
        [HarmonyPatch("OpenSaveGameSlotScreen")]
        [HarmonyPrefix]
        static bool SavePrefix()
        {
            Plugin.Logger.LogInfo("Start Saving");
            CSingleton<CGameManager>.Instance.m_CurrentSaveLoadSlotSelectedIndex = SaveHandler.SaveSlot;
            CSingleton<CGameManager>.Instance.m_IsManualSaveLoad = true;
            CSingleton<ShelfManager>.Instance.SaveInteractableObjectData();
            CSingleton<CGameManager>.Instance.SaveGameData(SaveHandler.SaveSlot);
            // SaveLoadGameSlotSelectScreen
            SaveLoadGameSlotSelectScreen saveScreen = GameObject.FindObjectOfType<SaveLoadGameSlotSelectScreen>();

            if (saveScreen != null)
            {
                Plugin.Logger.LogInfo("Screen Saving");
                saveScreen.m_SavingGameScreen.SetActive(value: true);
                CSingleton<SaveLoadGameSlotSelectScreen>.Instance.StartCoroutine(DelaySavingGame(saveScreen));
            }
            
            return true;
        }

        private static IEnumerator DelaySavingGame(SaveLoadGameSlotSelectScreen saveScreen)
        {
            Plugin.Logger.LogInfo("Delay Saving");


            yield return new WaitForSecondsRealtime(2f);
            saveScreen.m_SavingGameScreen.SetActive(value: false);
            saveScreen.gameObject.SetActive(value: false);
        }
    }
}
