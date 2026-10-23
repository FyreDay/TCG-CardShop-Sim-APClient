using ApClient.Archipelago;
using ApClient.Core.Save;
using ApClient.mapping;
using ApClient.ui;
using ApClient.UI;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.MessageLog.Parts;
using Archipelago.MultiClient.Net.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ApClient.Core.Archipelago;

public class ArchipelagoNet : MonoBehaviour
{
    private ArchipelagoState state;

    public ArchipelagoNet(ArchipelagoState state)
    {
        this.state = state;
    }
    private ArchipelagoSession Session { get; set; }
    private DeathLinkService deathLinkService = null;

    public async Task Connect(string ip, string password, string slot)
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
            Plugin.RunOnMainThread(() => state.onDisconnect("Failed to Connect"));
            return;
        }

        if (result.Successful)
        {
            var loginSuccess = (LoginSuccessful)result;

            string modversion = loginSuccess.SlotData.GetValueOrDefault("ModVersion").ToString();
            var modversionSplit = modversion.Split(".");
            var pluginVersionSplit = MyPluginInfo.PLUGIN_VERSION.Split(".");
            if (modversionSplit[0] != pluginVersionSplit[0] || modversionSplit[1] != pluginVersionSplit[1])
            {
                Plugin.Logger.LogError($"AP world version {modversion} is not compatible with plugin version {MyPluginInfo.PLUGIN_VERSION}");
                await Session.Socket.DisconnectAsync();
                Plugin.RunOnMainThread(() => state.onDisconnect($"AP Requires Mod v{modversion}"));
                return;
            }

            SlotData slotData = new SlotData(loginSuccess.SlotData);

            if (slotData.Deathlink)
            {
                deathLinkService = Session.CreateDeathLinkService();
                deathLinkService.EnableDeathLink();
                deathLinkService.OnDeathLinkReceived += (deathLinkObject) =>
                {
                    Plugin.RunOnMainThread(() =>
                    {
                        state.doDeathlink();
                    });
                };
            }

            state.onConnect(slotData);
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
        APConsole.Instance.Log("Disconnected from Archipelago");
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
        deathLinkService.SendDeathLink(new DeathLink(Plugin.LastUsedSlot.Value, Plugin.LastUsedSlot.Value + " Stunk up the place!"));
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
                Plugin.RunOnMainThread(() => state.QueueItem(itemIndex, item));
            }
        }
        catch (Exception ex)
        {
            APConsole.Instance.Log($"ItemReceived Error: {ex}");
            throw;
        }
    }

    private void OnMessageReceived(LogMessage message)
    {
        string messageStr;
        if (message.Parts.Any(x => x.Type == MessagePartType.Player) &&
            Plugin.FilterLog != null &&
            Plugin.FilterLog.Value &&
            !message.Parts.Any(x => x.Text.Contains(Session!.Players.GetPlayerName(Session.ConnectionInfo.Slot))))
            return;
        if (message.Parts.Length == 1)
        {
            messageStr = message.Parts[0].Text;
        }
        else
        {
            var builder = new StringBuilder();
            foreach (var part in message.Parts)
            {
                builder.Append($"{part.Text}");
            }

            messageStr = builder.ToString();
        }
        APConsole.Instance.Log(messageStr);
    }

    private void OnError(Exception ex, string message)
    {
        APConsole.Instance.Log($"Socket error: {message} - {ex.Message}");
    }

    private void OnSocketClosed(string reason)
    {
        Plugin.RunOnMainThread(() => state.onDisconnect($"Socket Closed: {reason}"));

    }

    private void OnHint(Hint[] hints)
    {
        Plugin.RunOnMainThread(() =>
        {
            foreach (Hint hint in hints)
            {
                if (hint.FindingPlayer == Session.ConnectionInfo.Slot && hint.Status == HintStatus.Priority)
                {
                    Plugin.SaveHandler.GetAchievementHandler().SetHinted(hint.LocationId);
                }
            }
        });
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