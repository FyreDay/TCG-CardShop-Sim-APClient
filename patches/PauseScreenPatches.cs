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
            Plugin.Log("Start Saving");
            CSingleton<CGameManager>.Instance.m_CurrentSaveLoadSlotSelectedIndex = 3;
            CSingleton<CGameManager>.Instance.m_IsManualSaveLoad = true;
            CSingleton<ShelfManager>.Instance.SaveInteractableObjectData();
            CSingleton<CGameManager>.Instance.SaveGameData(3);
            // SaveLoadGameSlotSelectScreen
            SaveLoadGameSlotSelectScreen saveScreen = GameObject.FindObjectOfType<SaveLoadGameSlotSelectScreen>();

            if (saveScreen != null)
            {
                Plugin.Log("Screen Saving");
                saveScreen.m_SavingGameScreen.SetActive(value: true);
                CSingleton<SaveLoadGameSlotSelectScreen>.Instance.StartCoroutine(DelaySavingGame(saveScreen));
            }
            
            return true;
        }

        private static IEnumerator DelaySavingGame(SaveLoadGameSlotSelectScreen saveScreen)
        {
            Plugin.Log("Delay Saving");


            yield return new WaitForSecondsRealtime(2f);
            saveScreen.m_SavingGameScreen.SetActive(value: false);
            saveScreen.gameObject.SetActive(value: false);
        }
    }
}
