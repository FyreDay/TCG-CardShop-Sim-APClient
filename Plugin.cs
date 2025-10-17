using ApClient.mapping;
using ApClient.patches;
using ApClient.ui;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.XR;
using static System.Net.Mime.MediaTypeNames;

namespace ApClient;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    
    private readonly Harmony m_Harmony = new(MyPluginInfo.PLUGIN_GUID);

    public static APClientSaveManager m_SaveManager = new APClientSaveManager();
    public static ItemHandler m_ItemHandler = new ItemHandler();
    public static SessionHandler m_SessionHandler = new SessionHandler();
    public static CardHelper m_CardHelper = new CardHelper();

    private Plugin()
    {
        
        this.m_Harmony.PatchAll();
    }

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Settings.Instance.Load(this);
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        SceneManager.sceneLoaded += this.OnSceneLoad;
       
    }
    void Start()
    {
        APConsole.Create();
        
        GameObject ui = new GameObject("ConnectionMenu");
        ConnectionMenu menu = ui.AddComponent<ConnectionMenu>();
        DontDestroyOnLoad(ui);

        Log("Setting up infomenu");
        GameObject infoui = new GameObject("APinfoMenu");
        APinfoMenu infomenu = infoui.AddComponent<APinfoMenu>();
        DontDestroyOnLoad(infoui);
        
    }

    private void OnDestroy()
    {
        this.m_Harmony.UnpatchSelf();
    }

    private static bool SceneLoaded = false;
    public static bool isSceneLoaded()
    {
        return SceneLoaded;
    }

    public static void onSceneLoadLogic()
    {
        SceneLoaded = true;
        m_SessionHandler.ProcessCachedItems();
    }

    public static bool OverrideTrades()
    {
        return m_SessionHandler.GetSlotData().TradesAreNew;
    }

    public static float getNumLuckItems()
    {
        return m_SaveManager.GetLuck();
    }

    public static bool isCashOnly()
    {
        return m_ItemHandler.cashOnly;
    }

    public static CardData getNewCard()
    {
        try
        {
            int expansion_limit = 8;
            int border_sanity = 5;
            bool foil_sanity = true;
            if (m_SessionHandler.GetSlotData().CardSanity != 0)
            {
                border_sanity = m_SessionHandler.GetSlotData().BorderInSanity;
                foil_sanity = m_SessionHandler.GetSlotData().FoilInSanity;
                expansion_limit = m_SessionHandler.GetSlotData().CardSanity;
            }


            ECardExpansionType expansion = UnityEngine.Random.Range(0, expansion_limit) > 4 ? ECardExpansionType.Destiny : ECardExpansionType.Tetramon;

            HashSet<ERarity> desiredRarities = new HashSet<ERarity>();

            if (expansion == ECardExpansionType.Destiny)
            {
                for (int i = 0; i < expansion_limit - 4; i++)
                {
                    desiredRarities.Add((ERarity)i);
                }
            }

            if (expansion == ECardExpansionType.Tetramon)
            {
                for (int i = 0; i < expansion_limit && i < 4; i++)
                {
                    desiredRarities.Add((ERarity)i);
                }
            }
            return m_CardHelper.RandomNewCard(expansion, desiredRarities, border_sanity, foil_sanity);
        }
        catch (Exception e)
        {
            Log(e.ToString());
            APConsole.Instance.Log("Error in New Card Randomization");
            return m_CardHelper.CardRoller(ECollectionPackType.DestinyLegendaryCardPack);
        }
    }
    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        Log($" Scene Load: {scene.name}");
        if (scene.name == "Title")
        {
            m_SaveManager = new APClientSaveManager();
            setTitleInteractable(false);

            //GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

            //foreach (GameObject obj in allObjects)
            //{
            //    Debug.Log($"Object: {obj.name} | Active: {obj.activeSelf} | Tag: {obj.tag} | Layer: {obj.layer}");
            //}
            SceneLoaded = false;
        }
        if (scene.name == "Start")
        {

            //CSingleton<PhoneManager>.Instance.m_RentBillScreen.m_DueDayMax = 4;
            
        }
    }
    public static void RunTitleInteractableSaveLogic()
    {
        //set buttons correctly
        TitleScreen titleScreen = GameObject.FindFirstObjectByType<TitleScreen>();
        GameObject parentObject = GameObject.Find("NewGameBtn");
        UnityEngine.UI.Button newGame = null;
        if (parentObject != null)
        {
            // Look for the Button component in this object or its children
            newGame = parentObject.GetComponentInChildren<UnityEngine.UI.Button>();

        }
        if (m_SaveManager.doesSaveExist())
        {
            titleScreen.m_LoadGameButton.interactable = true;
            newGame.interactable = false;
        }
        else
        {
            titleScreen.m_LoadGameButton.interactable = false;
            newGame.interactable = true;

        }
    }
    public static void setTitleInteractable(bool interactable)
    {
        {
            TitleScreen titleScreen = GameObject.FindFirstObjectByType<TitleScreen>();
            titleScreen.m_LoadGameButton.interactable = interactable;


            GameObject parentObject = GameObject.Find("NewGameBtn");

            if (parentObject != null)
            {
                // Look for the Button component in this object or its children
                UnityEngine.UI.Button myButton = parentObject.GetComponentInChildren<UnityEngine.UI.Button>();
                myButton.interactable = interactable;

            }
        }
    }
    public static void Log(string s)
    {
        Logger.LogInfo(s);
    }

    //remove me
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.B))
        {
            //foreach (UI_PhoneScreen.)
            //{
            //    PhoneManager

            //}
        }
            if (Input.GetKeyDown(KeyCode.H))
        {
        }
    }
}
