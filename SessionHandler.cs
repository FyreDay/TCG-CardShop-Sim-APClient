using ApClient.data;
using ApClient.mapping;
using ApClientl;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

    public void SendGoalCompletion()
    {
        session.SetGoalAchieved();
    }

    public void CompleteLocationChecks(params long[] ids)
    {
        session.Locations.CompleteLocationChecks(ids);
    }

    private class ItemCache
    {

        public ItemInfo info { get; set; }
        public int index { get; set; }
    }

    private Queue<ItemCache> cachedItems = new Queue<ItemCache>();
    private LoginResult result = null;

    private List<int> StrToList(string str)
    {
        return str.Trim('[', ']')                 // Remove square brackets
               .Split(',')                      // Split by commas
               .Select(s => s.Trim())           // Trim whitespace and special characters
               .Where(s => int.TryParse(s, out _)) // Ensure valid integers
               .Select(int.Parse)               // Convert to integers
               .ToList();
    }
    private int startingCounter = 200;
    private void addStartingChecks(List<int> mapping, int startingId)
    {
        var item = LicenseMapping.getValueOrEmpty(mapping[0]);
        for (int i = 0; i < 8; i++)
        {
            var minAmount = LicenseMapping.GetKeyValueFromType(item.type).Min(kvp => kvp.Value.count);
            LicenseMapping.mapping.Add(startingCounter + i, (-1, "Unknown", (i + 3) * minAmount, startingId + i, item.type));
        }
        startingCounter += 8;
        Plugin.Log($"Goal Amount for {item.name} is {LicenseMapping.GetKeyValueFromType(item.type).Count()}");
    }


    public void connect(string ip, string password, string slot)
    {
        APGui.state = "Connecting";
        session = ArchipelagoSessionFactory.CreateSession(ip);
        //callback for item retrieval
        //session.Socket.SocketClosed += (reason) => { APGui.showGUI = true; };
        session.Items.ItemReceived += (receivedItemsHelper) => {

            if (!Plugin.isSceneLoaded())
            {
                Plugin.Log($"Not In Scene");
                cachedItems.Enqueue(new ItemCache() { info = receivedItemsHelper.DequeueItem(), index = receivedItemsHelper.Index });

                return;
            }
            Plugin.Log($"{receivedItemsHelper.Index} : {Plugin.m_SaveManager.GetProcessedIndex()}");
            if (Plugin.m_SaveManager.GetProcessedIndex() > receivedItemsHelper.Index)
            {
                return;
            }
            Plugin.m_SaveManager.IncreaseProcessedIndex();


            ItemInfo itemReceived = receivedItemsHelper.DequeueItem();

            Plugin.m_ItemHandler.processNewItem(itemReceived);

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
            Plugin.m_SaveManager.setSeed(session.RoomState.Seed);
            APGui.state = "Connected";
            var loginSuccess = (LoginSuccessful)result;
            slotData.CardSanity = int.Parse(loginSuccess.SlotData.GetValueOrDefault("CardSanity").ToString());
            slotData.Goal = int.Parse(loginSuccess.SlotData.GetValueOrDefault("Goal").ToString());
            slotData.ShopExpansionGoal = int.Parse(loginSuccess.SlotData.GetValueOrDefault("ShopExpansionGoal").ToString());
            slotData.ShopExpansionGoal = int.Parse(loginSuccess.SlotData.GetValueOrDefault("ShopExpansionGoal").ToString());
            slotData.LevelGoal = int.Parse(loginSuccess.SlotData.GetValueOrDefault("LevelGoal").ToString());
            slotData.GhostGoalAmount = int.Parse(loginSuccess.SlotData.GetValueOrDefault("GhostGoalAmount").ToString());
            slotData.TradesAreNew = loginSuccess.SlotData.GetValueOrDefault("BetterTrades").ToString() == "1";
            slotData.FoilInSanity = loginSuccess.SlotData.GetValueOrDefault("FoilInSanity").ToString() == "1";
            slotData.BorderInSanity = int.Parse(loginSuccess.SlotData.GetValueOrDefault("FoilInSanity").ToString());


            slotData.pg1IndexMapping = StrToList(loginSuccess.SlotData.GetValueOrDefault("ShopPg1Mapping").ToString());
            Plugin.Log(string.Join(", ", slotData.pg1IndexMapping));
            slotData.pg2IndexMapping = StrToList(loginSuccess.SlotData.GetValueOrDefault("ShopPg2Mapping").ToString());
            Plugin.Log(string.Join(", ", slotData.pg2IndexMapping));
            slotData.pg3IndexMapping = StrToList(loginSuccess.SlotData.GetValueOrDefault("ShopPg3Mapping").ToString());
            Plugin.Log(string.Join(", ", slotData.pg3IndexMapping));
            slotData.ttIndexMapping = StrToList(loginSuccess.SlotData.GetValueOrDefault("ShopTTMapping").ToString());
            Plugin.Log(string.Join(", ", slotData.ttIndexMapping));

            addStartingChecks(slotData.pg1IndexMapping, LicenseMapping.locs1Starting);
            Plugin.Log($"Mapping is {slotData.pg2IndexMapping.Count}");
            addStartingChecks(slotData.pg2IndexMapping, LicenseMapping.locs2Starting);
            addStartingChecks(slotData.pg3IndexMapping, LicenseMapping.locs3Starting);

            Plugin.RunTitleInteractableSaveLogic();
            Settings.Instance.SaveNewConnectionInfo(ip, password, slot);
        }
    }

    public void ProcessCachedItems()
    {
        CPlayerData.SetUnlockItemLicense(slotData.pg1IndexMapping[0]);
        CPlayerData.SetUnlockItemLicense(slotData.pg2IndexMapping[0]);
        CPlayerData.SetUnlockItemLicense(slotData.pg3IndexMapping[0]);

        while (cachedItems.Any())
        {
            var item = cachedItems.Dequeue();
            Plugin.Log($"{Plugin.m_SaveManager.GetProcessedIndex()} : {item.index}");
            if (Plugin.m_SaveManager.GetProcessedIndex() > item.index)
            {
                return;
            }
            Plugin.m_SaveManager.IncreaseProcessedIndex();
            Plugin.Log($"Item on load {item.info.ItemName}");
            Plugin.m_ItemHandler.processNewItem(item.info);
        }
        cachedItems.Clear();

    }
}
