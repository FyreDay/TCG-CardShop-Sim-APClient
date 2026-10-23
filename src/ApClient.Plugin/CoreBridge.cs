using ApClient.Core.Archipelago;
using ApClient.Core.Save;
using ApClient.Patches.Functionality;
using ApClient.ui;
using ApClient.UI;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.MessageLog.Parts;
using Archipelago.MultiClient.Net.Models;
using I2.Loc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ApClient;

public class CoreBridge : MonoBehaviour
{
    static readonly float SHOPLIFT_CHANCE = .1f;

    public ArchipelagoState state;

    public bool InGame { get; private set; } = false;

    private void Awake()
    {
        
        state = new ArchipelagoState();
    }

    public void OnSaveQuit()
    {
        state.DoDisconnect();
    }

    public void OnSaveLoaded()
    {
        InGame = true;
    }

    public void OnConnect(SlotData slotdata)
    {
        Util.RunTitleInteractableSaveLogic();
    }
    private void onDisconnect(string reason)
    {
        ConnectionMenu.setVisable(true);
        ConnectionMenu.SetState(reason, true);
        if (InGame)
        {
            CSingleton<ShelfManager>.Instance.SaveInteractableObjectData();
            CSingleton<CGameManager>.Instance.SaveGameData(0);
            InGame = false;
            CSingleton<CGameManager>.Instance.LoadMainLevelAsync("Title");
        }
    }
    public void Connect(string ip, string password, string slot)
    {
        Plugin.UpdateSavedConnection(ip, password, slot);
        state.DoConnect(ip, password, slot, MyPluginInfo.PLUGIN_VERSION, OnConnect, onDisconnect, onDeathlink, OnHints, OnMessageReceived);
    }

    public void onDeathlink()
    {
        if (state.IsConnected && InGame && Plugin.doDeathlink.Value && state.SlotData.Deathlink)
        {
            PopupTextPatches.ShowCustomText("Deathlink Has Caused Customers to Leave!");
            foreach (Customer c in CSingleton<CustomerManager>.Instance.m_CustomerList)
            {
                if (!(UnityEngine.Random.Range(0, 1f) < SHOPLIFT_CHANCE))
                {
                    continue;
                }
                if (c.m_IsAtPayingPosition)
                    continue;
                List<string> list = new List<string>();
                if (c.m_ItemInBagList.Count > 0 || c.m_CardInBagList.Count > 0)
                {
                    list.Add(LocalizationManager.GetTranslation("Im Going to Shoplift"));
                }
                else
                {
                    list.Add(LocalizationManager.GetTranslation("Im leaving"));
                }
                c.PopupText(list, 100);
                c.m_CustomerTournamentData.m_IsTournamentCustomer = false;
                c.m_ItemInBagList.Clear();
                c.m_CardInBagList.Clear();
                c.ExitShop();

            }
        }
    }

    public void OnHints(Hint[] hints)
    {
        foreach (Hint hint in hints)
        {
            if (hint.FindingPlayer == state._net.GetMyPlayerSlot() && hint.Status == HintStatus.Priority)
            {
                Plugin.SaveHandler.GetAchievementHandler().SetHinted(hint.LocationId);
            }
        }
    }

    private void OnMessageReceived(LogMessage message)
    {
        string messageStr;
        if (message.Parts.Any(x => x.Type == MessagePartType.Player) &&
            Plugin.FilterLog != null &&
            Plugin.FilterLog.Value &&
            !message.Parts.Any(x => x.Text.Contains(Plugin.LastUsedSlot.Value)))
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

    public void sendDeathlink()
    {
        if (state.IsConnected && InGame && Plugin.doDeathlink.Value && state.SlotData.Deathlink)
        {
            state._net.SendDeath();
        }
    }

    private void Update()
    {
        state.Update(InGame);
    }
}