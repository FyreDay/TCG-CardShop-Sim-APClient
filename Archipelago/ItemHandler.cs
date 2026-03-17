using ApClient.ui;
using Archipelago.MultiClient.Net.Models;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ApClient.Archipelago;

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
public class ItemHandler : MonoBehaviour
{
    private Queue<ItemWrapper> cachedItems = new Queue<ItemWrapper>();

    public void HandleItem(int index, ItemInfo item, bool save = true)
    {
        try
        {
            if (!Plugin.IsGameReady())
            {
                APConsole.Instance.DebugLog($"Game not ready, caching item: {item.ItemName} (index {index})");
                cachedItems.Enqueue(new ItemWrapper(index, item));
                return;
            }

            if (cachedItems.Count > 0)
            {
                APConsole.Instance.DebugLog($"Processing {cachedItems.Count} cached items...");
                FlushQueue();
            }

            ProcessItem(index, item);
        }
        catch (Exception ex)
        {
            APConsole.Instance.DebugLog($"HandleItem Error: {ex}");
        }
    }

    public void FlushQueue()
    {
        if (!Plugin.IsGameReady())
        {
            APConsole.Instance.DebugLog("Attempted to flush queue but game is not ready");
            return;
        }

        int processedCount = 0;
        while (cachedItems.Count > 0)
        {
            var itemWrapper = cachedItems.Dequeue();
            ProcessItem(itemWrapper.Index, itemWrapper.Info);
            processedCount++;
        }

        APConsole.Instance.DebugLog($"Flushed {processedCount} cached items");
        if (processedCount > 0)
            Plugin.SaveHandler.Save(Constants.SAVE_SLOT);
    }


    private void ProcessItem(int index, ItemInfo item)
    {
        if (index < Plugin.SaveHandler.saveData.ProcessedIndex)
        {
            APConsole.Instance.DebugLog($"Item {index} already processed (current: {Plugin.SaveHandler.saveData.ProcessedIndex})");
            return;
        }

        Plugin.SaveHandler.saveData.ProcessedIndex++;
    }
}
