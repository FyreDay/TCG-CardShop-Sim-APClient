using ApClient.Archipelago;
using ApClient.Archipelago.Mapping;
using ApClient.mapping;
using ApClient.Patches.Functionality;
using ApClient.ui;
using ApClient.UI;
using Archipelago.MultiClient.Net.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Device;
using static System.Collections.Specialized.BitVector32;

namespace ApClient.Core.Archipelago;

public enum ConnectionState
{
    Disconnected,
    Connecting,
    Connected
}

public class ItemWrapper
{
    public int Index;
    public ItemInfo Info;

    public ItemWrapper(int index, ItemInfo info)
    {
        Index = index;
        Info = info;
    }
}

public class ArchipelagoState : MonoBehaviour
{
    public ConnectionState connectionState = ConnectionState.Disconnected;
    public bool IsConnected => connectionState == ConnectionState.Connected;

    public bool InGame { get; private set; } = false;

    public SlotData slotdata { get; private set; }

    private ArchipelagoNet _net;
    private ItemHandler _itemsHandler;

    private void Awake()
    {
        var netObject = new GameObject("ArchipelagoNet");
        netObject.transform.SetParent(transform, false);

        _net = netObject.AddComponent<ArchipelagoNet>();

        var itemObject = new GameObject("ItemHandler");
        itemObject.transform.SetParent(transform, false);
        _itemsHandler = itemObject.AddComponent<ItemHandler>();
    }
    public void onDisconnect(string reason)
    {
        ConnectionMenu.setVisable(true);
        ConnectionMenu.SetState(reason, true);
        CSingleton<ShelfManager>.Instance.SaveInteractableObjectData();
        CSingleton<CGameManager>.Instance.SaveGameData(0);
        CSingleton<CGameManager>.Instance.LoadMainLevelAsync("Title");
    }

    public void onConnect(SlotData slotdata)
    {
        this.slotdata = slotdata;
        Util.RunTitleInteractableSaveLogic();
    }

    public void doDisconnect()
    {
        connectionState = ConnectionState.Disconnected;
        _ = _net.DisconnectAsync();
    }

    public void doConnect(string ip, string password, string slot)
    {
        Plugin.UpdateSavedConnection(ip, password, slot);
        _ = _net.Connect(ip, password, slot);
    }

    public void OnSaveQuit()
    {
        InGame = false;
    }

    public void OnSaveLoaded()
    {
        InGame = true;
    }

    public void doDeathlink()
    {
        if (IsConnected && InGame && Plugin.doDeathlink.Value && slotdata.Deathlink)
        {
            APLogicUtil.TriggerDeathlinkLogic();
        }
    }

    public void sendDeathlink()
    {
        if (IsConnected && InGame && Plugin.doDeathlink.Value && slotdata.Deathlink)
        {
            Plugin.Logger.LogInfo("Sent Death!");
            _net.SendDeath();
        }
    }

    public int GetItemCount(long id)
    {
        return _net.GetItemHelper().AllItemsReceived.Where(i => i.ItemId == id).Count();

    }

    public void CompleteLocationChecks(params long[] ids)
    {
        _net.GetLocationHelper().CompleteLocationChecks(ids);
    }

    public HashSet<long> GetCheckedLocations()
    {
        return _net.GetLocationHelper().AllLocationsChecked.ToHashSet();
    }

    private Queue<ItemWrapper> cachedItems = new Queue<ItemWrapper>();

    public void QueueItem(int index, ItemInfo item)
    {
        cachedItems.Enqueue(new ItemWrapper(index, item));
    }

    public bool hasQueuedItems()
    {
        return cachedItems.Count > 0;
    }

    private void Update()
    {
        if (IsConnected && InGame)
        {
            while (cachedItems.Count > 0)
            {
                var itemWrapper = cachedItems.Dequeue();
                _itemsHandler.ProcessItem(itemWrapper.Index, itemWrapper.Info);
            }
        }
    }
}
