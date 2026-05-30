using ApClient.Archipelago;
using ApClient.Archipelago.Mapping;
using ApClient.assets;
using ApClient.mapping;
using ApClient.Patches.Functionality;
using ApClient.UI;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using I2.Loc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Profiling.Memory.Experimental;
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
    public static ConfigEntry<KeyCode> LogToggleKey;
    public static ConfigEntry<KeyCode> HistoryToggleKey;
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
        //asset bundle
        string assetsFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        string bundlePath = Path.Combine(assetsFolder, "apinfoui"); // Make sure "myAssetBundle" is the correct file name
        // Load the asset bundle
        myAssetBundle = AssetBundle.LoadFromFile(bundlePath);

        TextAsset metadataText = myAssetBundle.LoadAsset<TextAsset>("metadata");
        BundleMetadata metadata = JsonConvert.DeserializeObject<BundleMetadata>( metadataText.text);

        var modversionSplit = metadata.bundleVersion.Split(".");
        var pluginVersionSplit = ("1.0.0").Split(".");//MyPluginInfo.PLUGIN_VERSION.Split(".");
        if (modversionSplit[0] != pluginVersionSplit[0] || modversionSplit[1] != pluginVersionSplit[1])
        {
            ConnectionMenu.SetState($"Wrong asset version. Needs {modversionSplit[0]}.{modversionSplit[1]}.X", false);
            Logger.LogError($"Loaded asset bundle version {metadata.bundleVersion} does not match plugin major and minor version {MyPluginInfo.PLUGIN_VERSION}");
        }
        else
        {
            Logger.LogInfo($"Loaded asset bundle version {metadata.bundleVersion}");
        }

            _ = ConnectionMenu.Instance;
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
        GameObject myGameObject = myAssetBundle.LoadAsset<GameObject>("APInfo");

        var apinfoobject = Instantiate(myGameObject);

        var infoPanel = apinfoobject.AddComponent<UIInfoPanel>();
        
        SceneLoaded = true;

        UIInfoPanel.setInstance(infoPanel, apinfoobject, myAssetBundle.LoadAsset<GameObject>("Achievement"),
            myAssetBundle.LoadAsset<GameObject>("Product"), SaveHandler.GetAchievementHandler().achievementsByType);

        UIInfoPanel.getInstance().setVisable(false);
        UIInfoPanel.getInstance().InitializeEventGames(!ArchipelagoHandler.slotData.NoFormat, ArchipelagoHandler.slotData.PlayTableChecks);
        List<ECollectionPackType> ownedPacks = new List<ECollectionPackType>();

        for (int i = 0; i < CPlayerData.m_IsItemLicenseUnlocked.Count; i++)
        {
            if (!CPlayerData.m_IsItemLicenseUnlocked[i])
                continue;

            ECollectionPackType packType = InventoryBase.ItemTypeToCollectionPackType(InventoryBase.GetRestockData(i).itemType);

            if (packType != ECollectionPackType.None)
            {
                ownedPacks.Add(packType);
            }
        }

        SaveHandler.GetAchievementHandler().UpdateAvailability(ownedPacks);
        
        ConnectionMenu.setVisable(false);
        ItemHandler.FlushQueue();

        if (ArchipelagoHandler.slotData.StartingEmployeeIndex != -1 && !CPlayerData.GetIsWorkerHired(ArchipelagoHandler.slotData.StartingEmployeeIndex))
        {
            CPlayerData.SetIsWorkerHired(ArchipelagoHandler.slotData.StartingEmployeeIndex, isHired: true);

            CSingleton<WorkerManager>.Instance.ActivateWorker(ArchipelagoHandler.slotData.StartingEmployeeIndex, resetTask: true);
            int num = 0;
            for (int i = 0; i < CPlayerData.m_IsWorkerHired.Count; i++)
            {
                if (CPlayerData.m_IsWorkerHired[i])
                {
                    num++;
                }
            }

            AchievementManager.OnStaffHired(num);
        }

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
        //if (Input.GetKeyDown(KeyCode.F7))
        //{
        //    APLogicUtil.TriggerDeathlinkLogic();
        //}

        // Toggle with hotkey
        if ((Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape)) && IsGameReady())
        {
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
        LogToggleKey = Config.Bind(
            "Hotkeys",
            "Toggle AP Console",
            KeyCode.F7, // Default key
            "Press this key to toggle AP Console Output"
        );
        HistoryToggleKey = Config.Bind(
            "Hotkeys",
            "Toggle AP Console History",
            KeyCode.F6, // Default key
            "Press this key to toggle AP Console History"
        );
        LastUsedIP = Config.Bind("Connection", "LastUsedIP", "", "The last server IP entered.");
        LastUsedPassword = Config.Bind("Connection", "LastUsedPassword", "", "The last server password entered.");
        LastUsedSlot = Config.Bind("Connection", "LastUsedSlot", "", "The last player slot name entered.");
    }

    public static async Task<bool> ConnectAsync(string ip, string password, string slot)
    {
        LastUsedIP.Value = ip;
        LastUsedPassword.Value = password;
        LastUsedSlot.Value = slot;
        SaveHandler = await ArchipelagoHandler.ConnectAsync(ip, password, slot);
        if (SaveHandler != null)
        {
            Util.RunTitleInteractableSaveLogic();
            ConfigRef.Save();
            return true;
        }
        ConfigRef.Save();
        return false;
    }

    public static async Task Disconnect()
    {

        Cleanup();

        if (ArchipelagoHandler != null)
        {
            try
            {
                await ArchipelagoHandler.DisconnectAsync();
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogWarning($"Disconnect error: {ex}");
            }
        }
    }

    public static void Cleanup() {
        ConnectionMenu.setVisable(true);
        ConnectionMenu.SetState("Not Connected", true);
        ClearSave();
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
        Plugin.Logger.LogInfo("Unloading AP Mod");
        //_ = ArchipelagoHandler.DisconnectAsync();
        this.Harmony.UnpatchSelf();
    }
}
