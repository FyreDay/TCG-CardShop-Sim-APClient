using ApClient.data;
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
using UnityEngine;
using static System.Collections.Specialized.BitVector32;

namespace ApClient.Archipelago;

public class ArchipelagoHandler : MonoBehaviour
{
    public SlotData slotData;
    private ArchipelagoSession? Session { get; set; }
    DeathLinkService deathLinkService = null;
    
    private ConcurrentQueue<long> _locationsToCheck = new ConcurrentQueue<long>();
    public bool IsConnected => Session?.Socket.Connected ?? false;
    public SaveHandler connect(string ip, string password, string slot)
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
            ConnectionMenu.Instance.SetState("Connnection Failed");
        }

        if (result.Successful)
        {
            var loginSuccess = (LoginSuccessful)result;

            string modversion = loginSuccess.SlotData.GetValueOrDefault("ModVersion").ToString();
            if (!modversion.Equals(MyPluginInfo.PLUGIN_VERSION))
            {
                ConnectionMenu.Instance.SetState($"AP Expects Mod v{modversion}");
            }
            
            slotData = new SlotData(loginSuccess.SlotData);

            if (slotData.Deathlink)
            {
                deathLinkService = Session.CreateDeathLinkService();
                deathLinkService.EnableDeathLink();
                deathLinkService.OnDeathLinkReceived += (deathLinkObject) =>
                {
                    if (slotData.Deathlink && Plugin.IsGameReady())
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

            return new SaveHandler(Session.RoomState.Seed, slot);
        }

        return null;
    }

    

    public async void DisconnectAsync()
    {
        if (Session == null)
            return;
        await Session.Socket.DisconnectAsync();
        Session = null;
        APConsole.Instance.Log("Disconnected from Archipelago");
    }


    public ScoutedItemInfo? TryScoutLocation(long locationId)
    {
        return Session?.Locations.ScoutLocationsAsync(locationId)?.Result?.Values.First();
    }

    public void sendDeath()
    {
        if (slotData.Deathlink)
        {
            Plugin.Logger.LogInfo("Sent Death!");
            deathLinkService.SendDeathLink(new DeathLink(Plugin.LastUsedSlot.Value, Plugin.LastUsedSlot.Value + " Died to Not Paying Bills."));
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
    }

    private void OnError(Exception ex, string message)
    {
        APConsole.Instance.Log($"Socket error: {message} - {ex.Message}");
    }

    private void OnSocketClosed(string reason)
    {
        StopAllCoroutines();
        Plugin.ClearSave();
        //todo:go to main menu
        APConsole.Instance.Log($"Socket closed: {reason}");
        ConnectionMenu.Instance.setVisable(true);
        ConnectionMenu.Instance.SetState("Not Connected");
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

    void OnDestroy() { 
        Session!.Socket.ErrorReceived -= OnError;
        Session!.MessageLog.OnMessageReceived -= OnMessageReceived;
        Session!.Socket.ErrorReceived -= OnError;
        Session!.Socket.SocketClosed -= OnSocketClosed;
        Session!.Items.ItemReceived -= ItemReceived;
    }
}
