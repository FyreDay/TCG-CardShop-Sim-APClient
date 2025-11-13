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
using System.IO;
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

    private static AssetBundle myAssetBundle;
    private static GameObject apinfoobject;
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
        //asset bundle
        string assetsFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        string bundlePath = Path.Combine(assetsFolder, "apinfoui"); // Make sure "myAssetBundle" is the correct file name
        Plugin.Log(assetsFolder);
        Plugin.Log(bundlePath);
        // Load the asset bundle
        myAssetBundle = AssetBundle.LoadFromFile(bundlePath);

        // Example: Load a GameObject from the bundle
        GameObject myGameObject = myAssetBundle.LoadAsset<GameObject>("APInfo");

        // Do something with myGameObject, e.g., instantiate it
        apinfoobject = Instantiate(myGameObject);

        var infoPanel = apinfoobject.AddComponent<UIInfoPanel>();
        infoPanel.Setup(apinfoobject, myAssetBundle.LoadAsset<GameObject>("Achievement"));
        infoPanel.SetLevelMax(50);
        infoPanel.SetStoredXP(12345);
        infoPanel.SetLicensesToLevel(3);
        infoPanel.UpdateList(new List<CardLocation>
        {
            new CardLocation
            {
                IsHinted = true,
                CurrentNum = 0,
                AchievementData = new data.AchievementData
                {
                    name = "Open 100 Destiny\r\n Legendary Cards",
                    threshold = 100
                }
            },
            new CardLocation
            {
                IsHinted = false,
                CurrentNum = 2,
                AchievementData = new data.AchievementData
                {
                    name = "Open 10 Destiny\r\n Rare Cards",
                    threshold = 10
                }
            },
        });

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

    public static float getNumLuckItems()
    {
        return m_SaveManager.GetLuck();
    }

    public static bool isCashOnly()
    {
        return m_ItemHandler.cashOnly;
    }

    public static EGameEventFormat getFormat()
    {
        return CPlayerData.m_GameEventFormat;
    }

    public static CardData getNewCard()
    {
        int maxborder = 5;
        bool uniqueFoil = true;
        if (m_SessionHandler.GetSlotData().CardSanity > 0 && m_SessionHandler.GetSlotData().CardOpeningCheckDifficulty > 0)
        {
            uniqueFoil = m_SessionHandler.GetSlotData().CardSanity == 2;

            maxborder = m_SessionHandler.maxBorder();
        }

         return m_SaveManager.GenerateUnopenedCard(maxborder, uniqueFoil);
           
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
