using ApClient.data;
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
using static System.Collections.Specialized.BitVector32;

namespace ApClient.Archipelago;

public class ArchipelagoHandler : MonoBehaviour
{
    public SlotData slotData;
    private ArchipelagoSession Session { get; set; }
    DeathLinkService deathLinkService = null;
    
    private ConcurrentQueue<long> _locationsToCheck = new ConcurrentQueue<long>();
    public bool disconnecting = false;

    public bool IsConnected => Session?.Socket.Connected ?? false;
    public async Task<SaveHandler> ConnectAsync(string ip, string password, string slot)
    {
        _locationsToCheck = new ConcurrentQueue<long>();
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
            ConnectionMenu.SetState("Connnection Failed", true);
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
                ConnectionMenu.SetState($"AP Requires Mod v{modversion}", false);
                
                return null;
            }
            
            slotData = new SlotData(loginSuccess.SlotData);

            if (slotData.Deathlink)
            {
                deathLinkService = Session.CreateDeathLinkService();
                deathLinkService.EnableDeathLink();
                deathLinkService.OnDeathLinkReceived += (deathLinkObject) =>
                {
                    if (Plugin.EnabledDeathLink())
                    {
                        APLogicUtil.TriggerDeathlinkLogic();
                    }
                };
            }

            return new SaveHandler(Session.RoomState.Seed, slot);
        }

        return null;
    }

    

    public async Task DisconnectAsync()
    {
        if (Session == null || !disconnecting)
            return;
        disconnecting = false;
        await Session.Socket.DisconnectAsync();
        Session = null;
        deathLinkService = null;
        slotData = null;
        APConsole.Instance.Log("Disconnected from Archipelago");
    }


    public ScoutedItemInfo TryScoutLocation(long locationId)
    {
        Plugin.Logger.LogInfo($"Trying to scout location {locationId}");
        return Session?.Locations.ScoutLocationsAsync(locationId)?.Result?.Values.First();
    }

    public void sendDeath()
    {
        if (Plugin.EnabledDeathLink())
        {
            Plugin.Logger.LogInfo("Sent Death!");
            deathLinkService.SendDeathLink(new DeathLink(Plugin.LastUsedSlot.Value, Plugin.LastUsedSlot.Value + " Stunk up the place!"));
        }
    }
    public void Release()
    {
        Session?.SetGoalAchieved();
        Session?.SetClientState(ArchipelagoClientState.ClientGoal);
    }

    public int GetItemCount(long id)
    {
        return Session.Items.AllItemsReceived.Where(i => i.ItemId == id).Count();

    }

    public void CompleteLocationChecks(params long[] ids)
    {
        Session.Locations.CompleteLocationChecks(ids);
    }
    private void ItemReceived(ReceivedItemsHelper helper)
    {
        try
        {
            while (helper.Any())
            {
                var itemIndex = helper.Index;
                var item = helper.DequeueItem();
                Plugin.ItemHandler.HandleItem(itemIndex, item);
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
        //todo:go to main menu
        APConsole.Instance.Log($"Socket closed: {reason}");
        Plugin.ClearSave();
    }

    private void OnHint(Hint[] hints)
    {
        foreach (Hint hint in hints)
        {
            if (hint.FindingPlayer == Session.ConnectionInfo.Slot && hint.Status == HintStatus.Priority)
            {
                Plugin.SaveHandler.GetAchievementHandler().SetHinted(hint.LocationId);
            }
        }
    }

    public HashSet<long> GetCheckedLocations()
    {
        return Session.Locations.AllLocationsChecked.ToHashSet();
    }

    void OnDestroy() { 
        Session!.Socket.ErrorReceived -= OnError;
        Session!.MessageLog.OnMessageReceived -= OnMessageReceived;
        Session!.Socket.ErrorReceived -= OnError;
        Session!.Socket.SocketClosed -= OnSocketClosed;
        Session!.Items.ItemReceived -= ItemReceived;
    }
}
