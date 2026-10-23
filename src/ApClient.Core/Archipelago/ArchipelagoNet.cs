using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApClient.Core.Archipelago;

public class ArchipelagoNet
{
    private ArchipelagoState state;

    public ArchipelagoNet(ArchipelagoState state)
    {
        this.state = state;
    }
    private ArchipelagoSession Session { get; set; }
    private DeathLinkService deathLinkService = null;

    public async Task Connect(string ip, string password, string slot, string plugin_version)
    {
        if (Session != null)
        {
            await DisconnectAsync();
        }
        Session = ArchipelagoSessionFactory.CreateSession(ip);
        Session.MessageLog.OnMessageReceived += OnMessageReceived;
        Session.Socket.ErrorReceived += OnError;
        Session.Socket.SocketClosed += OnSocketClosed;
        Session.Items.ItemReceived += ItemReceived;
        Session.Hints.TrackHints(OnHint);

        LoginResult result = null;
        try
        {
            result = Session.TryConnectAndLogin("TCG Card Shop Simulator", slot, ItemsHandlingFlags.AllItems, null, null, null, password, true);
        }
        catch (Exception ex)
        {
            state.QueueNetworkEvent(() => state.OnDisconnect("Failed to Connect"));
            return;
        }

        if (result.Successful)
        {
            var loginSuccess = (LoginSuccessful)result;

            string modversion = loginSuccess.SlotData.GetValueOrDefault("ModVersion").ToString();
            var modversionSplit = modversion.Split(".");
            var pluginVersionSplit = plugin_version.Split(".");
            if (modversionSplit[0] != pluginVersionSplit[0] || modversionSplit[1] != pluginVersionSplit[1])
            {
                Logger.Display($"AP world version {modversion} is not compatible with plugin version {plugin_version}");
                await Session.Socket.DisconnectAsync();
                state.QueueNetworkEvent(() => state.OnDisconnect($"AP Requires Mod v{modversion}"));
                return;
            }

            SlotData slotData = new SlotData(loginSuccess.SlotData);

            if (slotData.Deathlink)
            {
                deathLinkService = Session.CreateDeathLinkService();
                deathLinkService.EnableDeathLink();
                deathLinkService.OnDeathLinkReceived += (deathLinkObject) =>
                {
                    state.QueueNetworkEvent(() => state.OnDeathlink());
                };
            }

            state.QueueNetworkEvent(() => state.OnConnect(slotData));
        }

        return;
    }

    public async Task DisconnectAsync()
    {
        if (Session == null)
            return;
        await Session.Socket.DisconnectAsync();
        Session = null;
        deathLinkService = null;
        Logger.Log("Disconnected from Archipelago");
    }

    public ILocationCheckHelper GetLocationHelper()
    {
        if (Session == null)
            throw new InvalidOperationException("Not connected to Archipelago");
        return Session?.Locations;
    }

    public IReceivedItemsHelper GetItemHelper()
    {
        if (Session == null)
            throw new InvalidOperationException("Not connected to Archipelago");
        return Session?.Items;
    }

    public async Task<Dictionary<long, ScoutedItemInfo>> TryScoutLocation(long locationId)
    {
        if (Session == null)
            throw new InvalidOperationException("Not connected to Archipelago");
        return await Session.Locations.ScoutLocationsAsync(locationId);
    }

    public void SendDeath()
    {
        deathLinkService.SendDeathLink(new DeathLink(Session.Players.ActivePlayer.Name, Session.Players.ActivePlayer.Alias + " Stunk up the place!"));
    }
    public void Release()
    {
        Session?.SetGoalAchieved();
        Session?.SetClientState(ArchipelagoClientState.ClientGoal);
    }

    
    private void ItemReceived(ReceivedItemsHelper helper)
    {
        try
        {
            while (helper.Any())
            {
                var itemIndex = helper.Index;
                var item = helper.DequeueItem();
                state.QueueNetworkEvent(() => state.QueueItem(itemIndex, item));
            }
        }
        catch (Exception ex)
        {
            Logger.Log($"ItemReceived Error: {ex}");
            throw;
        }
    }

    private void OnMessageReceived(LogMessage message)
    {
        state.QueueNetworkEvent(() => state.OnMessageReceived(message));
    }

    private void OnError(Exception ex, string message)
    {
        Logger.Log($"Socket error: {message} - {ex.Message}");
    }

    private void OnSocketClosed(string reason)
    {
        state.QueueNetworkEvent(() => state.OnDisconnect($"Socket Closed: {reason}"));

    }

    private void OnHint(Hint[] hints)
    {
        state.QueueNetworkEvent(() => state.OnHints(hints));
    }

    public int GetMyPlayerSlot()
    {
        return Session.Players.ActivePlayer.Slot;
    }

    public string GetMyPlayerName()
    {
        return Session.Players.ActivePlayer.Name;
    }

    public string GetSeed()
    {
        return Session.RoomState.Seed;
    }

    void OnDestroy()
    {
        Session!.Socket.ErrorReceived -= OnError;
        Session!.MessageLog.OnMessageReceived -= OnMessageReceived;
        Session!.Socket.ErrorReceived -= OnError;
        Session!.Socket.SocketClosed -= OnSocketClosed;
        Session!.Items.ItemReceived -= ItemReceived;
    }
}