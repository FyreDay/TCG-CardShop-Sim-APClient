using ApClient.Archipelago;
using ApClient.Patches.Functionality;
using ApClient.UI;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using I2.Loc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ApClient;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public static new ManualLogSource Logger;
    public static ArchipelagoHandler ArchipelagoHandler;
    
    public static SaveHandler SaveHandler
    {
        get;
        private set;
    }
    public static ItemHandler ItemHandler;

    private static ConfigFile ConfigRef;

    public static ConfigEntry<float> MessageInTime;
    public static ConfigEntry<bool> FilterLog;
    public static ConfigEntry<float> MessageHoldTime;
    public static ConfigEntry<float> MessageOutTime;
    public static ConfigEntry<bool> EnableDebugLogging;
    public static ConfigEntry<KeyCode> ConnectionHotKey;
    //public static ConfigEntry<KeyCode> ConsoleHotkey;
    public static ConfigEntry<bool> doDeathlink;
    public static ConfigEntry<string> LastUsedIP;
    public static ConfigEntry<string> LastUsedPassword;
    public static ConfigEntry<string> LastUsedSlot;

    private readonly Harmony Harmony = new(MyPluginInfo.PLUGIN_GUID);


    private static AssetBundle myAssetBundle;


    public static bool SceneLoaded { get; private set; }

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Application.logMessageReceived += HandleUnityLog;
        Harmony.PatchAll();
        DontDestroyOnLoad(gameObject);
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        SceneManager.sceneLoaded += this.OnSceneLoad;
        bindConfig();
        ArchipelagoHandler = gameObject.AddComponent<ArchipelagoHandler>();
        ItemHandler = gameObject.AddComponent<ItemHandler>();
    }

    void Start()
    {
        //APConsole.Create();

        GameObject ui = new GameObject("ConnectionMenu");
        ConnectionMenu menu = ui.AddComponent<ConnectionMenu>();
        DontDestroyOnLoad(ui);

        //asset bundle
        string assetsFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        string bundlePath = Path.Combine(assetsFolder, "apinfoui"); // Make sure "myAssetBundle" is the correct file name
        // Load the asset bundle
        myAssetBundle = AssetBundle.LoadFromFile(bundlePath);

    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Title")
        {
            
            Util.SetTitleInteractable(false);

            SceneLoaded = false;
        }
        if (scene.name == "Start")
        {
            
            //CSingleton<PhoneManager>.Instance.m_RentBillScreen.m_DueDayMax = 4;

        }
    }

    public static void SetSceneLoaded()
    {
        Logger.LogInfo("start AP scene load");
        GameObject myGameObject = myAssetBundle.LoadAsset<GameObject>("APInfo");

        var apinfoobject = Instantiate(myGameObject);

        var infoPanel = apinfoobject.AddComponent<UIInfoPanel>();
        
        SceneLoaded = true;

        UIInfoPanel.setInstance(infoPanel, apinfoobject, myAssetBundle.LoadAsset<GameObject>("Achievement"),
            myAssetBundle.LoadAsset<GameObject>("Product"), SaveHandler.GetAchievementHandler().achievementsByType);

        UIInfoPanel.getInstance().setVisable(false);
        SaveHandler.UpdateSanityUI();
        ItemHandler.FlushQueue();
        Logger.LogInfo("Finish AP scene load");
    }

    public static bool IsGameReady()
    {
        return ArchipelagoHandler.IsConnected && SceneLoaded;
    }

    public static bool EnabledDeathLink()
    {
        return ArchipelagoHandler.IsConnected && IsGameReady() && ArchipelagoHandler.slotData.Deathlink && doDeathlink.Value;
    }

    private void Update()
    {
        if (Input.GetKeyDown(ConnectionHotKey.Value))
        {
            ConnectionMenu.Instance.toggleVisability();
        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
            APLogicUtil.TriggerDeathlinkLogic();
        }

        // Toggle with hotkey
        if ((Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape)) && IsGameReady())
        {
            Logger.LogInfo("close custom window");
            UIInfoPanel.getInstance().DelaySetVisable(false);

        }
    }

    private void bindConfig()
    {
        ConfigRef = Config;
        EnableDebugLogging = Config.Bind(
                "Logging",
                "EnableDebugLogging",
                false,
                "Enables or disables debug logging in the Archipelago Console."
            );
        FilterLog = Config.Bind(
               "Logging",
               "FilterLog",
               false,
               "Filter the archipelago log to only show messages relevant to you."
           );

        MessageInTime = Config.Bind(
            "Logging",
            "MessageInTime",
            0.25f,
            "How long messages take to animate in."
        );

        MessageHoldTime = Config.Bind(
            "Logging",
            "MessageHoldTime",
            3f,
            "How long messages stay in the log before animating out."
        );

        MessageOutTime = Config.Bind(
            "Logging",
            "MessageOutTime",
            0.5f,
            "How long messages stay in the log before animating out."
        );

        doDeathlink = Config.Bind(
           "GamePlay",
           "Enable Deathlink",
           true,
           "Enable sending and receiving deathlinks. Overrides YAML."
       );

        ConnectionHotKey = Config.Bind(
            "Hotkeys",
            "Toggle Connection Window",
            KeyCode.F8, // Default key
            "Press this key to toggle AP Connection GUI"
        );
        //ConsoleHotkey = Config.Bind(
        //    "2. Hotkeys",
        //    "Toggle AP Console",
        //    KeyCode.F9, // Default key
        //    "Press this key to toggle AP Console Output"
        //);
        LastUsedIP = Config.Bind("Connection", "LastUsedIP", "", "The last server IP entered.");
        LastUsedPassword = Config.Bind("Connection", "LastUsedPassword", "", "The last server password entered.");
        LastUsedSlot = Config.Bind("Connection", "LastUsedSlot", "", "The last player slot name entered.");
    }

    public static void Connect(string ip, string password, string slot)
    {
        LastUsedIP.Value = ip;
        LastUsedPassword.Value = password;
        LastUsedSlot.Value = slot;
        SaveHandler = ArchipelagoHandler.connect(ip, password, slot);
        if (SaveHandler != null)
        {
            Util.RunTitleInteractableSaveLogic();
        }
        ConfigRef.Save();
    }

    public static void ClearSave()
    {
        if(SaveHandler != null && IsGameReady())
        {
            SaveHandler.Save(Constants.SAVE_SLOT);
        }
        SaveHandler = null;
    }


    void HandleUnityLog(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Warning &&
            condition.Contains("Parent of RectTransform is being set with parent property"))
        {
            return; // swallow it
        }

        // otherwise log normally if you want
        Debug.unityLogger.Log(type, condition);
    }

    private void OnDestroy()
    {
        Plugin.Logger.LogInfo("WHAT IS HAPPENING AHHHHHHHHHHA");
        ArchipelagoHandler.DisconnectAsync();
        this.Harmony.UnpatchSelf();
    }
}
