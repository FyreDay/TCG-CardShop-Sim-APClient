using ApClient.data;
using ApClient.mapping;
using ApClient.patches;
using ApClientl;
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

    public int itemCount(long id)
    {
        return session.Items.AllItemsReceived.Where(i => i.ItemId == id).Count();

    }

    public bool ControlTrades()
    {
        return slotData.TradesAreNew;
    }

    public bool hasItem(long id)
    {
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

    public bool isStartingItem(int id)
    {
        return Plugin.m_SessionHandler.GetSlotData().startingItems.Contains(id);
    }

    public int[] startingids()
    {
        return Plugin.m_SessionHandler.GetSlotData().startingItems.ToArray();
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
        Plugin.Log("Start Dict");
        foreach (var prop in jObj.Properties())
        {
            try
            {
                int name = int.Parse(prop.Name);
                EItemType key = (EItemType)(name == 190 ? 0 : name);
                int value = (int)prop.Value;
                ordered.Add(key, value);
                Plugin.Log($"{key} : {value}");
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
        APGui.state = "Connecting";
        session = ArchipelagoSessionFactory.CreateSession(ip);

        
        session.MessageLog.OnMessageReceived += OnMessageReceived;
        session.Items.ItemReceived += (receivedItemsHelper) => {

            if (!Plugin.isSceneLoaded())
            {
                Plugin.Log($"Not In Scene");
                cachedItems.Enqueue(new ItemCache() { info = receivedItemsHelper.DequeueItem(), index = receivedItemsHelper.Index });

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
            APGui.state = "Connection Failed";
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
                APGui.showGUI = true;
                APGui.state = "AP Disconnected";
                APConsole.Instance.Log("Connection Closed");
            };

            var loginSuccess = (LoginSuccessful)result;

            string modversion = loginSuccess.SlotData.GetValueOrDefault("ModVersion").ToString();
            if (!modversion.Equals(MyPluginInfo.PLUGIN_VERSION))
            {
                APGui.state = $"AP Expects Mod v{modversion}";
            }
            else
            {
                APGui.state = "Connected";
                
                Plugin.RunTitleInteractableSaveLogic();
            }

            
            slotData.MaxLevel = int.Parse(loginSuccess.SlotData.GetValueOrDefault("MaxLevel").ToString());
            slotData.RequiredLicenses = int.Parse(loginSuccess.SlotData.GetValueOrDefault("RequiredLicenses").ToString());
            slotData.Goal = int.Parse(loginSuccess.SlotData.GetValueOrDefault("Goal").ToString());
            slotData.CollectionGoalPercent = int.Parse(loginSuccess.SlotData.GetValueOrDefault("CollectionGoalPercent").ToString());
            slotData.GhostGoalAmount = int.Parse(loginSuccess.SlotData.GetValueOrDefault("GhostGoalAmount").ToString());

            slotData.TradesAreNew = loginSuccess.SlotData.GetValueOrDefault("BetterTrades").ToString() == "1";
            slotData.ExtraStartingItemChecks = int.Parse(loginSuccess.SlotData.GetValueOrDefault("ExtraStartingItemChecks").ToString());
            slotData.SellCheckAmount = int.Parse(loginSuccess.SlotData.GetValueOrDefault("SellCheckAmount").ToString());
            slotData.ChecksPerPack = int.Parse(loginSuccess.SlotData.GetValueOrDefault("ChecksPerPack").ToString());
            slotData.CardCollectPercentage = int.Parse(loginSuccess.SlotData.GetValueOrDefault("CardCollectPercentage").ToString());
            slotData.NumberOfGameChecks = int.Parse(loginSuccess.SlotData.GetValueOrDefault("PlayTableChecks").ToString());
            slotData.GamesPerCheck = int.Parse(loginSuccess.SlotData.GetValueOrDefault("GamesPerCheck").ToString());
            slotData.NumberOfSellCardChecks = int.Parse(loginSuccess.SlotData.GetValueOrDefault("NumberOfSellCardChecks").ToString());
            slotData.SellCardsPerCheck = int.Parse(loginSuccess.SlotData.GetValueOrDefault("SellCardsPerCheck").ToString());

            slotData.Deathlink = loginSuccess.SlotData.GetValueOrDefault("Deathlink").ToString() == "1";

            slotData.CardSanity = int.Parse(loginSuccess.SlotData.GetValueOrDefault("CardSanity").ToString());
            slotData.FoilInSanity = loginSuccess.SlotData.GetValueOrDefault("FoilInSanity").ToString() == "1";
            slotData.BorderInSanity = int.Parse(loginSuccess.SlotData.GetValueOrDefault("BorderInSanity").ToString());

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
        APConsole.Instance.Log(message.ToString() ?? string.Empty);
    }

    public void ProcessCachedItems()
    {
        foreach(int id in slotData.startingItems)
        {
            CPlayerData.SetUnlockItemLicense(id);
        }

        while (cachedItems.Any())
        {
            var item = cachedItems.Dequeue();
            if (Plugin.m_SaveManager.GetProcessedIndex() > item.index)
            {
                return;
            }
            Plugin.m_SaveManager.IncreaseProcessedIndex();
            Plugin.Log($"Item on load {item.info.ItemName}");
            Plugin.m_ItemHandler.processNewItem(item.info);
        }
        cachedItems.Clear();
        CSingleton<CGameManager>.Instance.SaveGameData(3);

    }
}
