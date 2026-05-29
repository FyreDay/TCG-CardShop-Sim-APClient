using ApClient.Archipelago;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ApClient.Patches.Structure;

[HarmonyPatch(typeof(PauseScreen))]
public class PauseScreenPatches
{
    [HarmonyPatch("OnPressBackMenu")]
    [HarmonyPostfix]
    static void MainMenuPostfix()
    {
        Plugin.ArchipelagoHandler.disconnecting = true;
        _ = Plugin.Disconnect();
    }


    [HarmonyPatch("OpenSaveGameSlotScreen")]
    [HarmonyPrefix]
    static bool SavePrefix()
    {
        Plugin.Logger.LogInfo("AP Save");
        CSingleton<CGameManager>.Instance.m_CurrentSaveLoadSlotSelectedIndex = Constants.SAVE_SLOT;
        CSingleton<CGameManager>.Instance.m_IsManualSaveLoad = true;
        CSingleton<ShelfManager>.Instance.SaveInteractableObjectData();
        CSingleton<CGameManager>.Instance.SaveGameData(Constants.SAVE_SLOT);
        // SaveLoadGameSlotSelectScreen
        SaveLoadGameSlotSelectScreen saveScreen = GameObject.FindObjectOfType<SaveLoadGameSlotSelectScreen>();

        if (saveScreen != null)
        {
            saveScreen.m_SavingGameScreen.SetActive(value: true);
            CSingleton<SaveLoadGameSlotSelectScreen>.Instance.StartCoroutine(DelaySavingGame(saveScreen));
        }
            
        return true;
    }

    private static IEnumerator DelaySavingGame(SaveLoadGameSlotSelectScreen saveScreen)
    {
        yield return new WaitForSecondsRealtime(2f);
        saveScreen.m_SavingGameScreen.SetActive(value: false);
        saveScreen.gameObject.SetActive(value: false);
    }
}

