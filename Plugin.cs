using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

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
        this.m_Harmony.UnpatchSelf();
    }

    private void Update()
    {
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
