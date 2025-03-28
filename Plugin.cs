using ApClient.mapping;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static System.Net.Mime.MediaTypeNames;

namespace ApClient;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{

    internal static new ManualLogSource Logger;

    private readonly Harmony m_Harmony = new(MyPluginInfo.PLUGIN_GUID);
    private Plugin()
    {
        this.m_Harmony.PatchAll();
    }
    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void OnDestroy()
    {
        //CSaveLoad
        
        this.m_Harmony.UnpatchSelf();
    }

    private bool showGUI = true;
    private string ipporttext = "localhost:38281";
    private string password = "";
    private string slot = "Player1";

    private string state = "Not Connected";

    void OnGUI()
    {
        if (!showGUI) return;

        // Create a GUI window
        GUI.Box(new Rect(10, 10, 200, 300), "AP Client");

        // Set font size and color
        GUIStyle textStyle = new GUIStyle();
        textStyle.fontSize = 12;
        textStyle.normal.textColor = UnityEngine.Color.white;

        // Display text at position (10,10)
        GUI.Label(new Rect(20, 40, 300, 30), "Address:port", textStyle);
        ipporttext = GUI.TextField(new Rect(20, 60, 180, 25), ipporttext, 25);

        GUI.Label(new Rect(20, 90, 300, 30), "Password", textStyle);
        password = GUI.TextField(new Rect(20, 110, 180, 25), password, 25);

        GUI.Label(new Rect(20, 140, 300, 30), "Slot", textStyle);
        slot = GUI.TextField(new Rect(20, 160, 180, 25), slot, 25);

        if (GUI.Button(new Rect(20, 210, 180, 30), "Connect"))
        {
            Debug.Log("Button Pressed!");
            state = "pressed";
            connect();
        }


        GUI.Label(new Rect(20, 240, 300, 30), state, textStyle);
    }
    public static ArchipelagoSession session;
    public static int CardSanity = 0;

    public static int itemCount(long id)
    {
        return session.Items.AllItemsReceived.Where(i => i.ItemId == id).Count();
        
    }

    public static bool hasItem(long id)
    {
        return itemCount(id) > 0;
    }

    private void connect()
    {
        
        state = "Connecting";
        session = ArchipelagoSessionFactory.CreateSession(ipporttext);

        LoginResult result;

        try
        {
            result = session.TryConnectAndLogin("TCG Card Shop Simulator", slot, ItemsHandlingFlags.AllItems,null, null, null, password, true);
        }
        catch (Exception e)
        {
            state = "Connection Failed";
            result = new LoginFailure(e.GetBaseException().Message);
            Debug.Log(e.GetBaseException().Message);
        }

        if (result.Successful)
        {
            state = "Connected";
            var loginSuccess = (LoginSuccessful)result;
            CardSanity = int.Parse(loginSuccess.SlotData.GetValueOrDefault("CardSanity").ToString());

            //callback for item retrieval
            session.Items.ItemReceived += (receivedItemsHelper) => {
                ItemInfo itemReceived = receivedItemsHelper.PeekItem();
                Log(itemReceived.ItemName);
                if(LicenseMapping.getKeyValue((int)itemReceived.ItemId).Key != -1)
                {
                    //update Restock ui
                }

                receivedItemsHelper.DequeueItem();
            };
        }
    }

    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9)) // Press F1 to log scenes
        {
            showGUI = !showGUI;
        }
            //UnityEngine
            //CPlayerData
            //BuyProductPanelUI
            //Resto
            //RestockItemScreen
            //EItemType
     }

    public static void Log(string s)
    {
        Logger.LogInfo(s);
    }
    //private void OnSceneChanged(UnityEngine.SceneManagement.Scene oldScene, UnityEngine.SceneManagement.Scene newScene)
    //{
    //    //    if (enableDebugLogging.Value)
    //    //        logger.LogDebug($"Scene changed to {newScene.name}. Resetting state.");

    //    //    ResetState();
    //    //    TryFindCardOpeningSequence();
    //    //}
    //}
}
