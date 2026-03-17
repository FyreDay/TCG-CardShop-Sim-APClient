using ApClient.mapping;
using ApClient.ui;
using Archipelago.MultiClient.Net.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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

        if ((int)item.ItemId == TrashMapping.smallMoney)
        {
            CEventManager.QueueEvent(new CEventPlayer_AddCoin(10 * Math.Min(CPlayerData.m_ShopLevel + 1, 25)));
            return;
        }
        if ((int)item.ItemId == TrashMapping.mediumMoney)
        {
            CEventManager.QueueEvent(new CEventPlayer_AddCoin(20 * Math.Min(CPlayerData.m_ShopLevel + 1, 25)));
            return;
        }
        if ((int)item.ItemId == TrashMapping.largeMoney)
        {
            CEventManager.QueueEvent(new CEventPlayer_AddCoin(40 * Math.Min(CPlayerData.m_ShopLevel + 1, 25)));
            return;
        }
        if ((int)item.ItemId == TrashMapping.smallXp)
        {
            CEventManager.QueueEvent(new CEventPlayer_AddShopExp(Math.Min((int)(CPlayerData.GetExpRequiredToLevelUp() * 0.1), ((CPlayerData.m_ShopLevel + 1) > 20 ? (int)(300 * (CPlayerData.m_ShopLevel + 1) * 0.2) : 400))));
            return;
        }
        if ((int)item.ItemId == TrashMapping.mediumXp)
        {
            CEventManager.QueueEvent(new CEventPlayer_AddShopExp(Math.Min((int)(CPlayerData.GetExpRequiredToLevelUp() * .17), ((CPlayerData.m_ShopLevel + 1) > 20 ? (int)(600 * (CPlayerData.m_ShopLevel + 1) * 0.2) : 800))));
            return;
        }
        if ((int)item.ItemId == TrashMapping.largeXp)
        {
            CEventManager.QueueEvent(new CEventPlayer_AddShopExp(Math.Min((int)(CPlayerData.GetExpRequiredToLevelUp() * 0.25), ((CPlayerData.m_ShopLevel + 1) > 20 ? (int)(1000 * (CPlayerData.m_ShopLevel + 1) * 0.2) : 1500))));
            return;
        }
        if ((int)item.ItemId == TrashMapping.randomcard)
        {
            CPlayerData.AddCard(Plugin.SaveHandler.NewRandomCard(), 1);
            return;
        }
        if ((int)item.ItemId == TrashMapping.ProgressiveCustomerMoney)
        {
            Plugin.SaveHandler.saveData.CustomerMoneyMult+=0.1;
            return;
        }

        if ((int)item.ItemId == TrashMapping.IncreaseCardLuck)
        {
            if (Plugin.SaveHandler.saveData.Luck >= 100)
            {
                CEventManager.QueueEvent(new CEventPlayer_AddCoin(40 * Math.Min(CPlayerData.m_ShopLevel + 1, 25)));
            }
            else
            {
                Plugin.SaveHandler.saveData.Luck += 1;
            }
            return;
        }

        if ((int)item.ItemId == TrashMapping.DecreaseCardLuck)
        {
            Plugin.SaveHandler.saveData.Luck -= Plugin.SaveHandler.saveData.Luck > 0 ? 1 : 0;
            return;
        }
        if ((int)item.ItemId == TrashMapping.CurrencyTrap)
        {
            CSingleton<CGameManager>.Instance.m_CurrencyType = (EMoneyCurrencyType)UnityEngine.Random.Range(0, 8);
            return;
        }

        if ((int)item.ItemId == TrashMapping.stinkTrap)
        {
            FieldInfo cfieldInfo = typeof(CustomerManager).GetField("m_CustomerList", BindingFlags.NonPublic | BindingFlags.Instance);
            if (cfieldInfo == null)
            {
                return;
            }
            List<Customer> list = (List<Customer>)cfieldInfo.GetValue(CSingleton<CustomerManager>.Instance);
            foreach (Customer c in list)
            {
                c.SetSmelly();
            }
            return;
        }
        if ((int)item.ItemId == TrashMapping.lightTrap)
        {

            Util.Instance.StartCoroutine(ToggleLightMultipleTimes());
            return;
        }

        if ((int)item.ItemId == TrashMapping.CreditCardFailure)
        {

            remainingTime += 60f;

            if (!timerRunning)
            {
                cashOnlyCoroutine = Util.Instance.StartCoroutine(CashOnlyTimerCoroutine());
            }
            return;
        }
    }

    private Coroutine cashOnlyCoroutine;
    private float remainingTime = 0f;
    private bool timerRunning = false;

    public bool cashOnly = false;
    private IEnumerator CashOnlyTimerCoroutine()
    {
        timerRunning = true;
        cashOnly = true;

        while (remainingTime > 0f)
        {
            yield return null;
            remainingTime -= Time.deltaTime;
        }

        cashOnly = false;
        timerRunning = false;
    }

    private IEnumerator ToggleLightMultipleTimes()
    {
        int repeats = UnityEngine.Random.Range(1, 6) * 2 - 1;
        for (int i = 0; i < repeats; i++)
        {
            CSingleton<LightManager>.Instance.ToggleShopLight();
            SoundManager.PlayAudio("SFX_ButtonLightTap", 0.6f, 0.5f);
            float delay = UnityEngine.Random.Range(0.5f, 2f); // adjust as needed
            yield return new WaitForSeconds(delay);
        }
    }
}
