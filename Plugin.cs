using ApClient.mapping;
using ApClient.patches;
using ApClientl;
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
        var go = new GameObject("MyGUI");
        go.AddComponent<APGui>();
        DontDestroyOnLoad(go);
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
        Log($"Luck: {m_SaveManager.GetLuck()}");
        return m_SaveManager.GetLuck();
    }

    public static bool isCashOnly()
    {
        return m_ItemHandler.cashOnly;
    }

    public static CardData getNewCard()
    {
        return m_ItemHandler.RandomNewCard();
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        Log($" Scene Load: {scene.name}");
        if (scene.name == "Title")
        {
            m_SaveManager.Clear();
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

            APGui.showGUI = false;

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
}
