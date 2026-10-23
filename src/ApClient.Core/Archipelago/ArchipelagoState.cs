using ApClient.Core.Save;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


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

public class ArchipelagoState
{
    public ConnectionState connectionState = ConnectionState.Disconnected;
    public bool IsConnected => connectionState == ConnectionState.Connected;

    

    public SlotData SlotData { get; private set; }

    public ArchipelagoNet _net {get; private set; }
    public SaveHandler _save { get; private set; }

    private static readonly Queue<Action> _networkEventQueue = new Queue<Action>();
    private Queue<ItemWrapper> cachedItems = new Queue<ItemWrapper>();
    private List<ItemEffect> itemEffects { get; } = new List<ItemEffect>();

    public ArchipelagoState()
    {
        _net = new ArchipelagoNet(this);
    }

    public void QueueItem(int index, ItemInfo item)
    {
        cachedItems.Enqueue(new ItemWrapper(index, item));
    }

    public bool hasQueuedItems()
    {
        return cachedItems.Count > 0;
    }

    public void QueueNetworkEvent(Action action)
    {
        lock (_networkEventQueue)
        {
            _networkEventQueue.Enqueue(action);
        }
    }

    public void Update(bool inGame)
    {
        while (_networkEventQueue.Count > 0)
        {
            var action = _networkEventQueue.Dequeue();
            action.Invoke();
        }

        if (IsConnected && inGame)
        {
            while (cachedItems.Count > 0)
            {
                var itemWrapper = cachedItems.Dequeue();
                if (itemWrapper.Index < _save.GetSaveData().NextExpectedIndex)
                {
                    Logger.Log($"Item {itemWrapper.Index} already processed (current: {_save.GetSaveData().NextExpectedIndex})");
                    return;
                }

                _save.GetSaveData().NextExpectedIndex++;
                foreach (ItemEffect itemEffect in itemEffects)
                {
                    if (itemEffect.ValidId(itemWrapper.Info.ItemId))
                        itemEffect.Effect(itemWrapper.Info.ItemId);
                }
            }
        }
    }

    public void RegisterItemEffect(ItemEffect itemEffect)
    {
        itemEffects.Add(itemEffect);
    }

    private Action<SlotData> onConnect;
    private Action<string> onDisconnect;
    private Action onDeathlink;
    private Action<Hint[]> onHints;
    private Action<LogMessage> onMessage;
    public void DoConnect(string ip, string password, string slot, string sem_ver, Action<SlotData> onConnect, Action<string> onDisconnect, Action onDeathlink, Action<Hint[]> onHints, Action<LogMessage> onLogMessage)
    {
        connectionState = ConnectionState.Connecting;
        _ = _net.Connect(ip, password, slot, sem_ver);
        this.onConnect = onConnect;
        this.onDisconnect = onDisconnect;
        this.onDeathlink = onDeathlink;
        this.onHints = onHints;
        this.onMessage = onLogMessage;
    }
    public void OnDisconnect(string reason)
    {
        connectionState = ConnectionState.Disconnected;
        onDisconnect(reason);
        
        onConnect = null;
        onDisconnect = null;
        onDeathlink = null;
        onHints = null;
        onMessage = null;
        SlotData = null;
    }

    public void OnConnect(SlotData slotdata)
    {
        connectionState = ConnectionState.Connected;
        _save = new SaveHandler(_net.GetSeed(), _net.GetMyPlayerName());
        this.SlotData = slotdata;
        onConnect(slotdata);
    }

    public void DoDisconnect()
    {
        _ = _net.DisconnectAsync();
    }
    
    public void OnDeathlink()
    {
        onDeathlink();
    }

    public void OnMessageReceived(LogMessage message)
    {
        onMessage(message);
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

    public void OnHints(Hint[] hints)
    {
        onHints(hints);
    }
}
