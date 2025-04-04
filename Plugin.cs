﻿using ApClient.mapping;
using ApClient.patches;
using ApClientl;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using BepInEx;
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

    

    public static ArchipelagoSession session;
    
    public static bool ControlTrades()
    {
        return TradesAreNew;
    }

    public static int itemCount(long id)
    {
        return session.Items.AllItemsReceived.Where(i => i.ItemId == id).Count();
        
    }

    public static bool hasItem(long id)
    {
        return itemCount(id) > 0;
    }

    private class ItemCache
    {

        public ItemInfo info {  get; set; }
        public int index { get; set; }
    }
    
    private static Queue<ItemCache> cachedItems = new Queue<ItemCache>();
    private static LoginResult result = null;
    public static int CardSanity = 0;
    public static bool TradesAreNew = false;
    public static int Goal = 0;
    public static int ShopExpansionGoal = 0;
    public static int LevelGoal = 0;
    public static int GhostGoalAmount = 0;

    public static List<int> pg1IndexMapping = new List<int>();
    public static List<int> pg2IndexMapping = new List<int>();
    public static List<int> pg3IndexMapping = new List<int>();
    public static List<int> ttIndexMapping = new List<int>();

    private static List<int> StrToList(string str)
    {
        return str.Trim('[', ']')                 // Remove square brackets
               .Split(',')                      // Split by commas
               .Select(s => s.Trim())           // Trim whitespace and special characters
               .Where(s => int.TryParse(s, out _)) // Ensure valid integers
               .Select(int.Parse)               // Convert to integers
               .ToList();
    }

    private static void addStartingChecks(List<int> mapping, int startingId)
    {
        var item = LicenseMapping.getValueOrEmpty(mapping[0]);
        for (int i = 0; i < 8; i++)
        {
            var minAmount = LicenseMapping.GetKeyValueFromType(item.type).Min(kvp => kvp.Value.count);
            LicenseMapping.mapping.Add(startingId + i, (-1, "Unknown", (i + 3) * minAmount, startingId + i, item.type));
        }
    }
       

    public static void connect(string ip, string password, string slot)
    {
        
        APGui.state = "Connecting";
        session = ArchipelagoSessionFactory.CreateSession(ip);
        //callback for item retrieval
        //session.Socket.SocketClosed += (reason) => { APGui.showGUI = true; };
        session.Items.ItemReceived += (receivedItemsHelper) => {

            
            if (!SceneLoaded)
            {
                Log($"Not In Scene");
                cachedItems.Enqueue(new ItemCache() { info =  receivedItemsHelper.DequeueItem(),index = receivedItemsHelper.Index });
                
                return;
            }
            Log($"{receivedItemsHelper.Index} : {m_SaveManager.getProcessedIndex()}");
            if (m_SaveManager.getProcessedIndex() > receivedItemsHelper.Index)
            {
                return;
            }
            m_SaveManager.increaseProcessedIndex();

            
            ItemInfo itemReceived = receivedItemsHelper.DequeueItem();

            processNewItem(itemReceived);
            
        };
        try
        {
            result = session.TryConnectAndLogin("TCG Card Shop Simulator", slot, ItemsHandlingFlags.AllItems,null, null, null, password, true);
        }
        catch (Exception e)
        {
            APGui.state = "Connection Failed";
            result = new LoginFailure(e.GetBaseException().Message);
            Debug.Log(e.GetBaseException().Message);
        }

        if (result.Successful)
        {
            APGui.state = "Connected";
            var loginSuccess = (LoginSuccessful)result;
            CardSanity = int.Parse(loginSuccess.SlotData.GetValueOrDefault("CardSanity").ToString());
            Goal = int.Parse(loginSuccess.SlotData.GetValueOrDefault("Goal").ToString());
            ShopExpansionGoal = int.Parse(loginSuccess.SlotData.GetValueOrDefault("ShopExpansionGoal").ToString());
            LevelGoal = int.Parse(loginSuccess.SlotData.GetValueOrDefault("LevelGoal").ToString());
            GhostGoalAmount = int.Parse(loginSuccess.SlotData.GetValueOrDefault("GhostGoalAmount").ToString());
            TradesAreNew = loginSuccess.SlotData.GetValueOrDefault("BetterTrades").ToString() == "1";
            pg1IndexMapping = StrToList(loginSuccess.SlotData.GetValueOrDefault("ShopPg1Mapping").ToString());
            Plugin.Log(string.Join(", ", pg1IndexMapping));
            pg2IndexMapping = StrToList(loginSuccess.SlotData.GetValueOrDefault("ShopPg2Mapping").ToString());
            Plugin.Log(string.Join(", ", pg2IndexMapping));
            pg3IndexMapping = StrToList(loginSuccess.SlotData.GetValueOrDefault("ShopPg3Mapping").ToString());
            Plugin.Log(string.Join(", ", pg3IndexMapping));
            ttIndexMapping = StrToList(loginSuccess.SlotData.GetValueOrDefault("ShopTTMapping").ToString());
            Plugin.Log(string.Join(", ", ttIndexMapping));

            addStartingChecks(pg1IndexMapping, LicenseMapping.locs1Starting);
            addStartingChecks(pg2IndexMapping, LicenseMapping.locs2Starting);
            addStartingChecks(pg3IndexMapping, LicenseMapping.locs3Starting);

            setTitleInteractable(true);
        }
    }

    public static void processNewItem(ItemInfo itemReceived)
    {
        Log(itemReceived.ItemName);
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
                    panel.m_PurchasedBtn.gameObject.SetActive(value: false);
                    //Log($"Recieved Progressive A While panel was open: {(int)itemReceived.ItemId}");
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
                        panel.m_PurchasedBtn.gameObject.SetActive(value: false);
                    }
                    //Log($"Recieved Progressive B While panel was open: {(int)itemReceived.ItemId}");
                }
            }
        }
        //Log($"Before Employee check: {EmployeeMapping.getKeyValue((int)itemReceived.ItemId).Key}");
        if (EmployeeMapping.getKeyValue((int)itemReceived.ItemId).Key != -1)
        {
            var itemMapping = EmployeeMapping.getKeyValue((int)itemReceived.ItemId);
            Log($"worker recieved id: {EmployeeMapping.getKeyValue((int)itemReceived.ItemId).Key}");
            //cannot run uless level fully loaded
            var screen = FindObjectOfType<HireWorkerScreen>();

            HireWorkerPanelUI[] allpanels = FindObjectsOfType<HireWorkerPanelUI>();
            HireWorkerPanelUI panel = null;
            foreach (HireWorkerPanelUI screenItem in allpanels)
            {
                FieldInfo fieldInfo = typeof(HireWorkerPanelUI).GetField("m_Index", BindingFlags.NonPublic | BindingFlags.Instance);
                if (fieldInfo == null)
                {
                    return;
                }

                int m_Index = (int)fieldInfo.GetValue(screenItem);
                if (m_Index == itemMapping.Key)
                {
                    panel = screenItem;
                    break;
                }
            }
            if (panel == null)
            {
                return;
            }
            Log("detected Hire Worker Screen");
            
            Log("Found Hire Worker Panel");
            panel.m_HiredText.SetActive(value: false);
            panel.m_PurchaseBtn.SetActive(value: true);
            //panel.Init(screen, itemMapping.Key);
            //EmployeePatches.HireEmployee(panel, itemMapping.Key);
            Log($"Recieved Worker While panel was open: {(int)itemReceived.ItemId}");

            //SoundManager.PlayAudio("SFX_CustomerBuy", 0.6f);
            
        }

        if(FurnatureMapping.getKeyValue((int)itemReceived.ItemId).Key != -1)
        {
            var itemMapping = FurnatureMapping.getKeyValue((int)itemReceived.ItemId, itemCount((int)itemReceived.ItemId));
            FurnitureShopPanelUI panel = null;
            //update Restock ui
            FurnitureShopPanelUI[] screen = FindObjectsOfType<FurnitureShopPanelUI>();
            foreach (FurnitureShopPanelUI screenItem in screen)
            {
                FieldInfo fieldInfo = typeof(FurnitureShopPanelUI).GetField("m_Index", BindingFlags.NonPublic | BindingFlags.Instance);
                if (fieldInfo == null)
                {
                    return;
                }

                int m_Index = (int)fieldInfo.GetValue(screenItem);
                if (m_Index == itemMapping.Key)
                {
                    panel = screenItem;
                    break;
                }
            }
            if (panel == null)
            {
                return;
            }
            FurnaturePatches.EnableFurnature(panel, itemMapping.Key);
        }
        if((int)itemReceived.ItemId == CardMapping.ghostProgressive)
        {
            bool isDestiny = false;
            int total = 0;
            List<bool> collectedlist = CPlayerData.GetIsCardCollectedList(ECardExpansionType.Ghost, isDestiny);
            total = collectedlist.FindAll(i => i == true).Count;
            if (total >=36)
            {
                isDestiny = true;
                collectedlist = CPlayerData.GetIsCardCollectedList(ECardExpansionType.Ghost, isDestiny);
                total += collectedlist.FindAll(i => i == true).Count;
            }

            
            if(Goal == 2 && GhostGoalAmount <= total)
            {
                session.SetGoalAchieved();
            }
            var list = InventoryBase.GetShownMonsterList(ECardExpansionType.Ghost);
            
            bool isFoil = false;
            int index = 0;
            for(int i = 0; i < list.Count; i++)
            {
                int dataindex = (int)(i * CPlayerData.GetCardAmountPerMonsterType(ECardExpansionType.Ghost) + ECardBorderType.FullArt);
                if (collectedlist[dataindex])
                {
                    dataindex += CPlayerData.GetCardAmountPerMonsterType(ECardExpansionType.Ghost, includeFoilCount: false);
                    if (!collectedlist[dataindex])
                    {
                        isFoil = true;
                        index = i;
                        break;
                    }
                }
                else
                {
                    index = i;
                    break;
                }
            }


            CPlayerData.AddCard(new CardData
            {
                isFoil = isFoil,
                isDestiny = isDestiny,
                borderType = ECardBorderType.FullArt,
                monsterType = list[index],
                expansionType = ECardExpansionType.Ghost,
                isChampionCard = false,
                isNew = true
            },1);
            
        }

        if ((int)itemReceived.ItemId == TrashMapping.smallMoney)
        {
            CEventManager.QueueEvent(new CEventPlayer_AddCoin(10*(CPlayerData.m_ShopLevel+1)));
        }
        if ((int)itemReceived.ItemId == TrashMapping.mediumMoney)
        {
            CEventManager.QueueEvent(new CEventPlayer_AddCoin(30 * (CPlayerData.m_ShopLevel + 1)));
        }
        if ((int)itemReceived.ItemId == TrashMapping.largeMoney)
        {
            CEventManager.QueueEvent(new CEventPlayer_AddCoin(50 * (CPlayerData.m_ShopLevel + 1)));
        }
        if ((int)itemReceived.ItemId == TrashMapping.smallXp)
        {
            CEventManager.QueueEvent(new CEventPlayer_AddShopExp((int)(CPlayerData.GetExpRequiredToLevelUp() * 0.1)));
        }
        if ((int)itemReceived.ItemId == TrashMapping.mediumXp)
        {
            CEventManager.QueueEvent(new CEventPlayer_AddShopExp((int)(CPlayerData.GetExpRequiredToLevelUp() * 0.2)));
        }
        if ((int)itemReceived.ItemId == TrashMapping.largeXp)
        {
            CEventManager.QueueEvent(new CEventPlayer_AddShopExp((int)(CPlayerData.GetExpRequiredToLevelUp() * 0.5)));
        }
        if ((int)itemReceived.ItemId == TrashMapping.randomcard)
        {
            var packlist = Enum.GetValues(typeof(ECollectionPackType));
            var packType = (ECollectionPackType)packlist.GetValue(UnityEngine.Random.Range(0, CardSanity == 0 ? 8 : CardSanity));
            CPlayerData.AddCard(cardRoller(packType),1);
        }
        if ((int)itemReceived.ItemId == TrashMapping.randomNewCard)
        {
            CPlayerData.AddCard(RandomNewCard(),1);
            //InteractableCashierCounter
            
        }
        if ((int)itemReceived.ItemId == TrashMapping.stinkTrap)
        {
            FieldInfo cfieldInfo = typeof(CustomerManager).GetField("m_CustomerList", BindingFlags.NonPublic | BindingFlags.Instance);
            if (cfieldInfo == null)
            {
                return;
            }
            List<Customer> list = (List<Customer>)cfieldInfo.GetValue(CSingleton<CustomerManager>.Instance);
            foreach(Customer c in list)
            {
                c.SetSmelly();
            }
        }
        if ((int) itemReceived.ItemId == TrashMapping.lightTrap)
        {
            SoundManager.PlayAudio("SFX_ButtonLightTap", 0.6f, 0.5f);
            //CSingleton<LightManager>.Instance.m_ShoplightGrp.SetActive(true);
            CSingleton<LightManager>.Instance.ToggleShopLight();
        }
    }

    private static bool SceneLoaded = false;

    private static void setTitleInteractable(bool interactable)
    {
        if (result != null && result.Successful)
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
        else
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
            
            APGui.showGUI = false;
            
        }
    }
    public static void ProcessCachedItems()
    {
        CPlayerData.SetUnlockItemLicense(pg1IndexMapping[0]);
        CPlayerData.SetUnlockItemLicense(pg2IndexMapping[0]);
        CPlayerData.SetUnlockItemLicense(pg3IndexMapping[0]);

        setRandomTypesForSanity();
        SceneLoaded = true;

        while (cachedItems.Any())
        {
            var item = cachedItems.Dequeue();
            Log($"{m_SaveManager.getProcessedIndex()} : {item.index}");
            if (m_SaveManager.getProcessedIndex() > item.index)
            {
                return;
            }
            m_SaveManager.increaseProcessedIndex();
            Log($"Item on load {item.info.ItemName}");
            processNewItem(item.info);
        }
        cachedItems.Clear();

    }

    public static void Log(string s)
    {
        Logger.LogInfo(s);
    }


    private static List<EMonsterType> m_MonsterTypes = new List<EMonsterType>();
    private static List<EMonsterType> m_DestinyMonsterTypes = new List<EMonsterType>();

    private static void setRandomTypesForSanity()
    {
        List<EMonsterType> typeList = new List<EMonsterType>();
        ECardExpansionType[] cardExpansionTypes = [ECardExpansionType.Tetramon, ECardExpansionType.Destiny];

        foreach( ECardExpansionType cardExpansionType in cardExpansionTypes ) {
            for (int i = 0; i < InventoryBase.GetShownMonsterList(cardExpansionType).Count; i++)
            {
                EMonsterType mType = InventoryBase.GetMonsterData(InventoryBase.GetShownMonsterList(cardExpansionType)[i]).MonsterType;
                ERarity rarity = InventoryBase.GetMonsterData(InventoryBase.GetShownMonsterList(cardExpansionType)[i]).Rarity;
                switch (rarity)
                {
                    case ERarity.Legendary:
                        if ((CardSanity >= 8 && cardExpansionType == ECardExpansionType.Destiny)
                            || (CardSanity >= 4 && cardExpansionType == ECardExpansionType.Tetramon) )
                        {
                            typeList.Add(mType);
                        }
                        break;
                    case ERarity.Epic:
                        if ((CardSanity >= 7 && cardExpansionType == ECardExpansionType.Destiny)
                            || (CardSanity >= 3 && cardExpansionType == ECardExpansionType.Tetramon))
                        {
                            typeList.Add(mType);
                        }
                        break;
                    case ERarity.Rare:
                        if ((CardSanity >= 6 && cardExpansionType == ECardExpansionType.Destiny)
                            || (CardSanity >= 2 && cardExpansionType == ECardExpansionType.Tetramon))
                        {
                            typeList.Add(mType);
                        }
                        break;
                    default:
                        if ((CardSanity >= 5 && cardExpansionType == ECardExpansionType.Destiny)
                            || (CardSanity >= 1 && cardExpansionType == ECardExpansionType.Tetramon))
                        {
                            typeList.Add(mType);
                        }
                        break;
                }
            }
            if(cardExpansionType == ECardExpansionType.Tetramon)
            {
                m_MonsterTypes = typeList;
            }
            else
            {
                m_DestinyMonsterTypes = typeList;
            }
            
        }
    }
    public static CardData RandomNewCard()
    {
        Log("Random New Card Generating");
        System.Random rand = new System.Random();

        ECardExpansionType expansion = (CardSanity < 5 || rand.NextDouble() >= 0.5) ? ECardExpansionType.Tetramon : ECardExpansionType.Destiny;
        List<bool> boolList = CPlayerData.GetIsCardCollectedList(expansion, false);

        // Allowed types as an enum list
        List<EMonsterType> allowedTypes = expansion == ECardExpansionType.Tetramon ? m_MonsterTypes : m_DestinyMonsterTypes;
        // Convert enum list to integer indices
        List<int> allowedIndices = allowedTypes.Select(type => (int)type).ToList();

        // Collect valid False indices within allowed type ranges
        List<int> falseIndices = new List<int>();
        foreach (int typeIndex in allowedIndices)
        {
            int start = typeIndex * 12;
            int end = start + 12;

            for (int i = start; i < end && i < 1452; i++)
            {
                if (!boolList[i])
                    falseIndices.Add(i);
            }
        }

        if (falseIndices.Count == 0)
        {
            expansion = ECardExpansionType.Tetramon == expansion ? ECardExpansionType.Destiny : ECardExpansionType.Tetramon;
            boolList = CPlayerData.GetIsCardCollectedList(expansion, false);

            // Allowed types as an enum list
            allowedTypes = expansion == ECardExpansionType.Tetramon ? m_MonsterTypes : m_DestinyMonsterTypes;
            // Convert enum list to integer indices
            allowedIndices = allowedTypes.Select(type => (int)type).ToList();

            // Collect valid False indices within allowed type ranges
            falseIndices = new List<int>();
            foreach (int typeIndex in allowedIndices)
            {
                int start = typeIndex * 12;
                int end = start + 12;

                for (int i = start; i < end && i < 1452; i++)
                {
                    if (!boolList[i])
                        falseIndices.Add(i);
                }
            }
            if (falseIndices.Count == 0)
            {
                Log("You have collected all Cards");
                return cardRoller(ECollectionPackType.DestinyLegendaryCardPack);
            }
        }

        // Randomly pick an index

        int selectedIndex = falseIndices[rand.Next(falseIndices.Count)];
        int type = selectedIndex % 12;
        var monsterType = CPlayerData.GetMonsterTypeFromCardSaveIndex(selectedIndex, expansion);
        Log($"Randomly selected False index: {selectedIndex} which is a {monsterType}");

        return new CardData
        {
            isFoil = type > 5,
            isDestiny = expansion == ECardExpansionType.Destiny,
            borderType = CPlayerData.GetCardBorderType(type % 6, expansion),
            monsterType = monsterType,
            expansionType = expansion,
            isChampionCard = false,
            isNew = true
        };
    }
    private static CardData cardRoller(ECollectionPackType collectionPackType)
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

        return new CardData
        {
            isFoil = randomGenerator.NextDouble() >= 0.5,
            isDestiny = randomGenerator.NextDouble() >= 0.5,
            borderType = border,
            monsterType = monster,
            expansionType = cardExpansionType,
            isChampionCard = false,
            isNew = true
        };
    }
}
