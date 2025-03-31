using ApClient.mapping;
using ApClient.patches;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;
using static System.Net.Mime.MediaTypeNames;

namespace ApClient;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{

    internal static new ManualLogSource Logger;
    

    private readonly Harmony m_Harmony = new(MyPluginInfo.PLUGIN_GUID);

    public static APClientSaveManager m_SaveManager = new APClientSaveManager();

    private Plugin()
    {
        this.m_Harmony.PatchAll();
    }
    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        SceneManager.sceneLoaded += this.OnSceneLoad;
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


    public static int processed = 0;
    private static List<ItemInfo> cachedItems = new List<ItemInfo>();
    private LoginResult result = null;
    private void connect()
    {
        
        state = "Connecting";
        session = ArchipelagoSessionFactory.CreateSession(ipporttext);

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

            setTitleInteractable(true);
            
            //callback for item retrieval
            session.Items.ItemReceived += (receivedItemsHelper) => {

                
                if (!SceneLoaded)
                {
                    if (m_SaveManager.getProcessedItems() > processed)
                    {
                        ItemInfo oldItem = receivedItemsHelper.DequeueItem();
                        Log($"Already Processed {oldItem.ItemName}");
                        
                        processed++;
                        return;
                    }
                    cachedItems.Add(receivedItemsHelper.DequeueItem());
                    processed++;
                    return;
                }

                ItemInfo itemReceived = receivedItemsHelper.DequeueItem();

                Log(itemReceived.ItemName);

                processNewItem(itemReceived);
                processed++;
                m_SaveManager.increaseProcessedItems();
            };
        }
    }

    public static void processNewItem(ItemInfo itemReceived)
    {
        if (LicenseMapping.getKeyValue((int)itemReceived.ItemId).Key != -1)
        {
            var itemMapping = LicenseMapping.getKeyValue((int)itemReceived.ItemId, itemCount((int)itemReceived.ItemId));
            RestockItemPanelUI panel = null;
            //update Restock ui
            RestockItemPanelUI[] screen = FindObjectsOfType<RestockItemPanelUI>();
            foreach (RestockItemPanelUI screenItem in screen)
            {
                if(screenItem.GetIndex() == itemMapping.Key)
                {
                    panel = screenItem;
                    break;
                }
            }
            if(panel == null)
            {
                return;
            }

            RestockItemPanelUIPatches.runLicenseBtnLogic(panel, true, itemMapping.Key);
            Log($"Recieved Item While panel was open: {(int)itemReceived.ItemId} and {itemMapping.Key}");
        }
        if ((int)itemReceived.ItemId == ExpansionMapping.progressiveA)
        {
            ExpansionShopUIScreen screen = FindObjectOfType<ExpansionShopUIScreen>();
            if (screen != null)
            {
                FieldInfo field = typeof(ExpansionShopUIScreen).GetField("m_IsShopB", BindingFlags.NonPublic | BindingFlags.Instance);
                bool isB = (bool)field.GetValue(screen);
                if (!isB)
                {
                    ExpansionShopPanelUI panel = screen.m_ExpansionShopPanelUIList[itemCount((int)itemReceived.ItemId) - 1];
                    panel.m_LockPurchaseBtn.gameObject.SetActive(value: false);
                    panel.m_PurchasedBtn.gameObject.SetActive(value: true);
                    Log($"Recieved Progressive A While panel was open: {(int)itemReceived.ItemId}");
                }

            }
        }
        if ((int)itemReceived.ItemId == ExpansionMapping.progressiveB)
        {
            ExpansionShopUIScreen screen = UnityEngine.Object.FindObjectOfType<ExpansionShopUIScreen>();
            if (screen != null)
            {
                FieldInfo field = typeof(ExpansionShopUIScreen).GetField("m_IsShopB", BindingFlags.NonPublic | BindingFlags.Instance);
                bool isB = (bool)field.GetValue(screen);
                if (isB)
                {
                    ExpansionShopPanelUI panel = screen.m_ExpansionShopPanelUIList[itemCount((int)itemReceived.ItemId) - 1];
                    FieldInfo fieldInfo = typeof(ExpansionShopPanelUI).GetField("m_LevelRequired", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (fieldInfo == null)
                    {
                        return;
                    }

                    int m_LevelRequired = (int)fieldInfo.GetValue(panel);
                    bool atLevel = CPlayerData.m_ShopLevel + 1 < m_LevelRequired;
                    if (atLevel)
                    {
                        panel.m_LockPurchaseBtn.gameObject.SetActive(value: false);
                        panel.m_PurchasedBtn.gameObject.SetActive(value: true);
                    }
                    Log($"Recieved Progressive A While panel was open: {(int)itemReceived.ItemId}");
                }
            }
        }
        if (EmployeeMapping.getKeyValue((int)itemReceived.ItemId).Key != -1)
        {
            HireWorkerScreen screen = UnityEngine.Object.FindObjectOfType<HireWorkerScreen>();
            if (screen != null)
            {
                Log("detected Hire Worker Screen");
                HireWorkerPanelUI panel = screen.m_HireWorkerPanelUIList[EmployeeMapping.getKeyValue((int)itemReceived.ItemId).Key];
                Log("Found Hire Worker Panel");
                panel.m_LevelRequirementText.gameObject.SetActive(value: false);
                panel.m_HireFeeText.gameObject.SetActive(value: true);
                panel.m_LockPurchaseBtn.gameObject.SetActive(value: false);
                Log($"Recieved Worker While panel was open: {(int)itemReceived.ItemId}");

                CPlayerData.SetIsWorkerHired(EmployeeMapping.getKeyValue((int)itemReceived.ItemId).Key, isHired: true);
                CSingleton<WorkerManager>.Instance.ActivateWorker(EmployeeMapping.getKeyValue((int)itemReceived.ItemId).Key, resetTask: true);

                SoundManager.PlayAudio("SFX_CustomerBuy", 0.6f);
            }
        }
        if ((int)itemReceived.ItemId == TrashMapping.smallMoney)
        {
            CEventManager.QueueEvent(new CEventPlayer_AddCoin(50));
        }
        if ((int)itemReceived.ItemId == TrashMapping.smallXp)
        {
            CEventManager.QueueEvent(new CEventPlayer_AddShopExp(100));
        }
        if ((int)itemReceived.ItemId == TrashMapping.mediumMoney)
        {
            CEventManager.QueueEvent(new CEventPlayer_AddCoin(150));
        }
        if ((int)itemReceived.ItemId == TrashMapping.mediumXp)
        {
            CEventManager.QueueEvent(new CEventPlayer_AddShopExp(200));
        }
        if ((int)itemReceived.ItemId == TrashMapping.largeMoney)
        {
            CEventManager.QueueEvent(new CEventPlayer_AddCoin(500));
        }
        if ((int)itemReceived.ItemId == TrashMapping.largeXp)
        {
            CEventManager.QueueEvent(new CEventPlayer_AddShopExp(500));
        }
        if ((int)itemReceived.ItemId == TrashMapping.randomcard)
        {
            var packlist = Enum.GetValues(typeof(ECollectionPackType));
            var packType = (ECollectionPackType)packlist.GetValue(UnityEngine.Random.Range(0, CardSanity == 0 ? 8 : CardSanity));
            cardRoller(packType);
        }
    }

    private static bool SceneLoaded = false;

    private void setTitleInteractable(bool interactable)
    {
        if (result != null && result.Successful)
        {
            Log("Has result");
            //set buttons correctly
            if (m_SaveManager.doesSaveExist())
            {
                TitleScreen titleScreen = GameObject.FindFirstObjectByType<TitleScreen>();
                titleScreen.m_LoadGameButton.interactable = true;
            }
            else
            {
                GameObject parentObject = GameObject.Find("NewGameBtn");

                if (parentObject != null)
                {
                    // Look for the Button component in this object or its children
                    Button myButton = parentObject.GetComponentInChildren<Button>();
                    myButton.interactable = true;

                }
            }
        }
        else
        {
            TitleScreen titleScreen = GameObject.FindFirstObjectByType<TitleScreen>();
            titleScreen.m_LoadGameButton.interactable = interactable;


            GameObject parentObject = GameObject.Find("NewGameBtn");

            if (parentObject != null)
            {
                // Look for the Button component in this object or its children
                Button myButton = parentObject.GetComponentInChildren<Button>();
                myButton.interactable = interactable;

            }
        }
    }
    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        Log($" Scene Load: {scene.name}");
        if (scene.name == "Title")
        {
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
            showGUI = false;
            
        }
    }

    public static void ProcessCachedItems()
    {

        SceneLoaded = true;
        foreach (ItemInfo item in cachedItems)
        {
            Log($"Item on load {item.ItemName}");
            processNewItem(item);
            m_SaveManager.increaseProcessedItems();
            processed++;
        }
        cachedItems.Clear();
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

    private static void cardRoller(ECollectionPackType collectionPackType)
    {
        List<EMonsterType> commonlist = new List<EMonsterType>();
        List<EMonsterType> rarelist = new List<EMonsterType>();
        List<EMonsterType> epiclist = new List<EMonsterType>();
        List<EMonsterType> legendlist = new List<EMonsterType>();
        ECardExpansionType cardExpansionType = InventoryBase.GetCardExpansionType(collectionPackType);

        for (int i = 0; i < InventoryBase.GetShownMonsterList(cardExpansionType).Count; i++)
        {
            EMonsterType monsterType = InventoryBase.GetMonsterData(InventoryBase.GetShownMonsterList(cardExpansionType)[i]).MonsterType;
            ERarity rarity = InventoryBase.GetMonsterData(InventoryBase.GetShownMonsterList(cardExpansionType)[i]).Rarity;
            switch (rarity)
            {
                case ERarity.Legendary:
                    legendlist.Add(monsterType);
                    break;
                case ERarity.Epic:
                    epiclist.Add(monsterType);
                    break;
                case ERarity.Rare:
                    rarelist.Add(monsterType);
                    break;
                default:
                    commonlist.Add(monsterType);
                    break;
            }
        }
        System.Random randomGenerator = new System.Random();

        var vb = Enum.GetValues(typeof(ECardBorderType));
        var border = (ECardBorderType)vb.GetValue(randomGenerator.Next(vb.Length));

        var monster = EMonsterType.BatA;

        switch (collectionPackType)
        {
            case ECollectionPackType.BasicCardPack:
                monster = commonlist[UnityEngine.Random.Range(0, commonlist.Count)];
                break;
            case ECollectionPackType.DestinyBasicCardPack:
                monster = commonlist[UnityEngine.Random.Range(0, commonlist.Count)];
                break;
            case ECollectionPackType.RareCardPack:
                monster = rarelist[UnityEngine.Random.Range(0, rarelist.Count)];
                break;
            case ECollectionPackType.DestinyRareCardPack:
                monster = rarelist[UnityEngine.Random.Range(0, rarelist.Count)];
                break;
            case ECollectionPackType.EpicCardPack:
                monster = epiclist[UnityEngine.Random.Range(0, epiclist.Count)];
                break;
            case ECollectionPackType.DestinyEpicCardPack:
                monster = epiclist[UnityEngine.Random.Range(0, epiclist.Count)];
                break;
            case ECollectionPackType.LegendaryCardPack:
                monster = legendlist[UnityEngine.Random.Range(0, legendlist.Count)];
                break;
            case ECollectionPackType.DestinyLegendaryCardPack:
                monster = legendlist[UnityEngine.Random.Range(0, legendlist.Count)];
                break;
            default:
                monster = commonlist[UnityEngine.Random.Range(0, commonlist.Count)];
                break;
        }

        CPlayerData.AddCard(new CardData
        {
            isFoil = randomGenerator.NextDouble() >= 0.5,
            isDestiny = randomGenerator.NextDouble() >= 0.5,
            borderType = border,
            monsterType = monster,
            expansionType = cardExpansionType,
            isChampionCard = false,
            isNew = true
        }, 1);
    }
}
