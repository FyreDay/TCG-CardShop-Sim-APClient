using ApClient.data;
using ApClient.mapping;
using ApClient.patches;
using ApClient.ui;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Models;
using I2.Loc.SimpleJSON;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using UnityEngine;
using static UnityEngine.UIElements.StylePropertyAnimationSystem;

namespace ApClient;

public class SessionHandler
{
    private ArchipelagoSession session;
    private SlotData slotData = new SlotData();

    public SlotData GetSlotData()
    {
        return slotData;
    }

    public int maxBorder()
    {
        int maxborder = 5;

        switch (GetSlotData().CardOpeningCheckDifficulty)
        {
            case 1:
                maxborder = 0;
                break;
            case 2:
                maxborder = 2;
                break;
            case 3:
                maxborder = 3;
                break;
            case 4:
                break;
        }

        return maxborder;
    }

    public int itemCount(long id)
    {
        return session.Items.AllItemsReceived.Where(i => i.ItemId == id).Count();

    }

    public bool hasItem(long id)
    {
        if(id == 0)
        {
            id = 190;
        }
        return itemCount(id) > 0;
    }

    public bool isEventUnlocked(EGameEventFormat format)
    {
        return hasItem(PlayTableMapping.FormatStartingId + (int)format);
    }

    public void SendGoalCompletion()
    {
        session.SetGoalAchieved();
    }

    public void CompleteLocationChecks(params long[] ids)
    {
        session.Locations.CompleteLocationChecks(ids);
    }

    public string getLocationName(long id)
    {
        return session.Locations.GetLocationNameFromId(id);
    }

    public bool isStartingItem(int id)
    {
        return slotData.startingItems.Contains(id);
    }

    public int[] startingids()
    {
        return slotData.startingItems.ToArray();
    }

    public int GetRemainingLicenses(int currentLevelStart)
    {

        var allLicenses = new Dictionary<EItemType, int>();
        foreach (DictionaryEntry entry in slotData.pg1IndexMapping)
        {
            EItemType itemId = (EItemType)entry.Key;
            int level = (int)entry.Value;

            if (level < currentLevelStart && !allLicenses.ContainsKey(itemId))
            {
                allLicenses[itemId] = level;
            }
        }
        foreach (DictionaryEntry entry in slotData.pg2IndexMapping)
        {
            EItemType itemId = (EItemType)entry.Key;
            int level = (int)entry.Value;

            if (level < currentLevelStart && !allLicenses.ContainsKey(itemId))
            {
                allLicenses[itemId] = level;
            }
        }
        foreach (DictionaryEntry entry in slotData.pg3IndexMapping)
        {
            EItemType itemId = (EItemType)entry.Key;
            int level = (int)entry.Value;

            if (level < currentLevelStart && !allLicenses.ContainsKey(itemId))
            {
                allLicenses[itemId] = level;
            }
        }
        foreach (DictionaryEntry entry in slotData.ttIndexMapping)
        {
            EItemType itemId = (EItemType)entry.Key;
            int level = (int)entry.Value;

            if (level < currentLevelStart && !allLicenses.ContainsKey(itemId))
            {
                allLicenses[itemId] = level;
            }
        }

        if (allLicenses.Count == 0)
            return 0; // no requirements, so zero remaining

        int required = slotData.RequiredLicenses;
        int sect_1 = currentLevelStart;
        int sect_2 = 0;
        int sect_3 = 0;
        if (currentLevelStart > 25)
        {
            sect_1 = 25; 
            sect_2 = currentLevelStart - 25;
        }

        if (currentLevelStart > 50)
        {
            sect_2 = 25;
            sect_3 = currentLevelStart - 50;
        }
        // Calculate how many licenses are required at this level
        int requiredCount = (sect_1 / 5) * slotData.RequiredLicenses;
        requiredCount += (sect_2 / 5) * 3;
        requiredCount += (sect_3 / 5) * 2;

        // Count how many licenses the player currently owns
        int ownedCount = allLicenses.Keys.Count(itemId => hasItem((int)itemId));

        int remaining = requiredCount - ownedCount;
        return remaining > 0 ? remaining : 0; // return 0 if none remaining
    }

    private class ItemCache
    {

        public ItemInfo info { get; set; }
        public int index { get; set; }
    }

    private Queue<ItemCache> cachedItems = new Queue<ItemCache>();
    private LoginResult result = null;

    private OrderedDictionary PgStrToDict(string str)
    {
        var jObj = JObject.Parse(str);
        var ordered = new OrderedDictionary();
        foreach (var prop in jObj.Properties())
        {
            try
            {
                int name = int.Parse(prop.Name);
                EItemType key = (EItemType)(name == 190 ? 0 : name);
                int value = (int)prop.Value;
                ordered.Add(key, value);
                //Plugin.Log($"{key} : {value}");
            }
            catch
            {
                Plugin.Log($" FAILED {prop.Name} : {prop.Value}");
            }
        }

        return ordered;
    }
    private List<int> StrToList(string str)
    {
        return str.Trim('[', ']')                 // Remove square brackets
               .Split(',')                      // Split by commas
               .Select(s => s.Trim())           // Trim whitespace and special characters
               .Where(s => int.TryParse(s, out _)) // Ensure valid integers
               .Select(int.Parse)               // Convert to integers
               .ToList();
    }

    DeathLinkService deathLinkService = null;
    public void sendDeath()
    {
        if (slotData.Deathlink)
        {
            Plugin.Log("Sent Death!");
            deathLinkService.SendDeathLink(new DeathLink(Settings.Instance.LastUsedSlot.Value, Settings.Instance.LastUsedSlot.Value + " Died to Not Paying Bills."));
        }
    }
    private bool isConnected = false;
    public bool GetIsConnected()
    {
        return isConnected;
    }
    public void connect(string ip, string password, string slot)
    {
        ConnectionMenu.state = "Connecting";
        session = ArchipelagoSessionFactory.CreateSession(ip);

        
        session.MessageLog.OnMessageReceived += OnMessageReceived;
        session.Items.ItemReceived += (receivedItemsHelper) => {

            if (!Plugin.isSceneLoaded())
            {
                Plugin.Log($"Not In Scene");
                cachedItems.Enqueue(new ItemCache() { info = receivedItemsHelper.DequeueItem(), index = receivedItemsHelper.Index });
                Plugin.Log("Enqueue");
                return;
            }
            //Plugin.Log($"{receivedItemsHelper.Index} : {Plugin.m_SaveManager.GetProcessedIndex()}");
            if (Plugin.m_SaveManager.GetProcessedIndex() > receivedItemsHelper.Index)
            {
                return;
            }
            Plugin.m_SaveManager.IncreaseProcessedIndex();

            ItemInfo itemReceived = receivedItemsHelper.DequeueItem();

            Plugin.m_ItemHandler.processNewItem(itemReceived);

            CSingleton<CGameManager>.Instance.SaveGameData(3);
        };
        try
        {
            result = session.TryConnectAndLogin("TCG Card Shop Simulator", slot, ItemsHandlingFlags.AllItems, null, null, null, password, true);
        }
        catch (Exception e)
        {
            ConnectionMenu.state = "Connection Failed";
            result = new LoginFailure(e.GetBaseException().Message);
            Plugin.Log(e.GetBaseException().Message);
        }

        if (result.Successful)
        {
            isConnected = true;
            Plugin.m_SaveManager.setConnectionData(session.RoomState.Seed, slot);

            //callback for item retrieval
            session.Socket.SocketClosed += (reason) => {
                isConnected = false;
                ConnectionMenu.Instance.setVisable(true);
                ConnectionMenu.state = "AP Disconnected";
                APConsole.Instance.Log("Connection Closed");
            };

            var loginSuccess = (LoginSuccessful)result;

            string modversion = loginSuccess.SlotData.GetValueOrDefault("ModVersion").ToString();
            if (!modversion.Equals(MyPluginInfo.PLUGIN_VERSION))
            {
                ConnectionMenu.state = $"AP Expects Mod v{modversion}";
            }
            else
            {
                ConnectionMenu.state = "Connected";
                
                Plugin.RunTitleInteractableSaveLogic();
            }

            
            slotData.MaxLevel = int.Parse(loginSuccess.SlotData.GetValueOrDefault("MaxLevel").ToString());
            slotData.RequiredLicenses = int.Parse(loginSuccess.SlotData.GetValueOrDefault("RequiredLicenses").ToString());
            slotData.Goal = int.Parse(loginSuccess.SlotData.GetValueOrDefault("Goal").ToString());
            slotData.CollectionGoalPercent = int.Parse(loginSuccess.SlotData.GetValueOrDefault("CollectionGoalPercent").ToString());
            slotData.GhostGoalAmount = int.Parse(loginSuccess.SlotData.GetValueOrDefault("GhostGoalAmount").ToString());
            
            slotData.OpenAchievementData = JsonConvert.DeserializeObject<List<AchievementData>>(loginSuccess.SlotData.GetValueOrDefault("OpenAchievements").ToString());
            slotData.SellAchievementData = JsonConvert.DeserializeObject<List<AchievementData>>(loginSuccess.SlotData.GetValueOrDefault("SellAchievements").ToString());
            slotData.GradeAchievementData = JsonConvert.DeserializeObject<List<AchievementData>>(loginSuccess.SlotData.GetValueOrDefault("GradeAchievements").ToString());
            foreach (var a in slotData.OpenAchievementData)
            {
                Plugin.Log($"Achievement: {a.name} with difficulty {a.difficulty}");
            }

            foreach (var a in slotData.SellAchievementData)
            {
                Plugin.Log($"Achievement: {a.name} with difficulty {a.difficulty}");
            }
            if (slotData.SellAchievementData != null)
            {
                APinfoMenu.Instance.setCardSellList((slotData.SellAchievementData ?? new List<AchievementData>()).Select(ach => new CardLocation
                {
                    IsHinted = false,
                    CurrentNum = 0,
                    Status = CardStatus.Unavailable,
                    AchievementData = ach
                }).ToList());
            }

            if (slotData.GradeAchievementData != null)
            {
                APinfoMenu.Instance.setCardGradeList((slotData.GradeAchievementData).Select(ach => new CardLocation
                {
                    IsHinted = false,
                    CurrentNum = 0,
                    Status = CardStatus.Unavailable,
                    AchievementData = ach
                }).ToList());
            }
            if (slotData.OpenAchievementData != null)
            {
                APinfoMenu.Instance.setCardOpenList((slotData.OpenAchievementData ?? new List<AchievementData>()).Select(ach => new CardLocation
                {
                    IsHinted = false,
                    CurrentNum = 0,
                    Status = CardStatus.Unavailable,
                    AchievementData = ach
                }).ToList());
            }

            slotData.AutoRenovate = loginSuccess.SlotData.GetValueOrDefault("AutoRenovate").ToString() == "1";
            slotData.ExtraStartingItemChecks = int.Parse(loginSuccess.SlotData.GetValueOrDefault("ExtraStartingItemChecks").ToString());
            slotData.SellCheckAmount = int.Parse(loginSuccess.SlotData.GetValueOrDefault("SellCheckAmount").ToString());
            slotData.CardOpeningCheckDifficulty = int.Parse(loginSuccess.SlotData.GetValueOrDefault("CardOpeningCheckDifficulty").ToString());
            slotData.CardSellingCheckDifficulty = int.Parse(loginSuccess.SlotData.GetValueOrDefault("CardSellingCheckDifficulty").ToString());
            slotData.CardGradingCheckDifficulty = int.Parse(loginSuccess.SlotData.GetValueOrDefault("CardGradingCheckDifficulty").ToString());
            slotData.PlayTableChecks = int.Parse(loginSuccess.SlotData.GetValueOrDefault("PlayTableChecks").ToString());


            slotData.Deathlink = loginSuccess.SlotData.GetValueOrDefault("Deathlink").ToString() == "1";

            slotData.CardSanity = int.Parse(loginSuccess.SlotData.GetValueOrDefault("CardSanity").ToString());

            if (slotData.Deathlink)
            {
                deathLinkService = session.CreateDeathLinkService();
                deathLinkService.EnableDeathLink();
                deathLinkService.OnDeathLinkReceived += (deathLinkObject) =>
                {
                    if (slotData.Deathlink && Plugin.isSceneLoaded())
                    {
                        APConsole.Instance.Log($"{deathLinkObject.Cause}");
                        CSingleton<LightManager>.Instance.m_HasDayEnded = true;
                        CSingleton<LightManager>.Instance.m_TimeHour = 21;
                        CSingleton<LightManager>.Instance.EvaluateTimeClock();
                        CEventManager.QueueEvent(new CEventPlayer_OnDayEnded());
                        //CoroutineRunner.RunOnMainThread(() => EndOfDayReportScreen.OpenScreen());
                    }
                };
            }

            slotData.pg1IndexMapping = PgStrToDict(loginSuccess.SlotData.GetValueOrDefault("ShopPg1Mapping").ToString());

            slotData.pg2IndexMapping = PgStrToDict(loginSuccess.SlotData.GetValueOrDefault("ShopPg2Mapping").ToString());
            slotData.pg3IndexMapping = PgStrToDict(loginSuccess.SlotData.GetValueOrDefault("ShopPg3Mapping").ToString());
            slotData.ttIndexMapping = PgStrToDict(loginSuccess.SlotData.GetValueOrDefault("ShopTTMapping").ToString());
            slotData.startingItems = StrToList(loginSuccess.SlotData.GetValueOrDefault("StartingIds").ToString());

            Settings.Instance.SaveNewConnectionInfo(ip, password, slot);
        }
        else
        {
            var failure = (LoginFailure)result;
            var errorMessage = $"Failed to Connect to {ip} as {slot}:";
            errorMessage = failure.Errors.Aggregate(errorMessage, (current, error) => current + $"\n    {error}");
            errorMessage = failure.ErrorCodes.Aggregate(errorMessage, (current, error) => current + $"\n    {error}");
            APConsole.Instance.Log(errorMessage);
        }
    }

    static void OnMessageReceived(LogMessage message)
    {
        switch (message)
        {
            case HintItemSendLogMessage hintLogMessage:
                if (!hintLogMessage.IsReceiverTheActivePlayer)
                {
                    break;
                }
                var hintreceiver = hintLogMessage.Receiver;
                var hintsender = hintLogMessage.Sender;
                var hintnetworkItem = hintLogMessage.Item;
                var hintfound = hintLogMessage.IsFound;
                APConsole.Instance.Log("OH WOW THIS IS RELATED TO ME!!!!");
                //APinfoMenu.Instance.HintAchievement(Plugin.m_SessionHandler.getLocationName(hintLogMessage.Item.LocationId));
                break;
            case ItemSendLogMessage itemSendLogMessage:
                var receiver = itemSendLogMessage.Receiver;
                var sender = itemSendLogMessage.Sender;
                var networkItem = itemSendLogMessage.Item;
                break;
        }
        APConsole.Instance.Log(message.ToString() ?? string.Empty);
    }

    public void ProcessCachedItems()
    {
        //Plugin.Log($"Cache {cachedItems.Count()}");
        while (cachedItems.Any())
        {
            var item = cachedItems.Dequeue();
            if (Plugin.m_SaveManager.GetProcessedIndex() > item.index)
            {
                //Plugin.Log($"Item Processed previously");
                continue;
            }
            Plugin.m_SaveManager.IncreaseProcessedIndex();
            Plugin.Log($"Item on load {item.info.ItemName}");
            Plugin.m_ItemHandler.processNewItem(item.info);
        }
        cachedItems.Clear();
        CSingleton<CGameManager>.Instance.SaveGameData(3);
        
    }

    public long GetLocationId(string name)
    {
        return session.Locations.GetLocationIdFromName(session.ConnectionInfo.Game, name);
    }
}
