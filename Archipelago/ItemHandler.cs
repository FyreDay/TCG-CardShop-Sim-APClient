using ApClient.Archipelago.Mapping;
using ApClient.mapping;
using ApClient.Patches.Functionality;
using ApClient.ui;
using Archipelago.MultiClient.Net.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        if (index < Plugin.SaveHandler.GetSaveData().ProcessedIndex)
        {
            APConsole.Instance.DebugLog($"Item {index} already processed (current: {Plugin.SaveHandler.GetSaveData().ProcessedIndex})");
            return;
        }
        Util.RunOnMainThread(() =>
        {
            Plugin.SaveHandler.GetSaveData().ProcessedIndex++;

            switch (item.ItemId)
            {
                case GenericItemMapping.SMALL_MONEY:
                    CEventManager.QueueEvent(new CEventPlayer_AddCoin(10 * Math.Min(CPlayerData.m_ShopLevel + 1, 25)));
                    return;
                case GenericItemMapping.MEDIUM_MONEY:
                    CEventManager.QueueEvent(new CEventPlayer_AddCoin(20 * Math.Min(CPlayerData.m_ShopLevel + 1, 25)));
                    return;
                case GenericItemMapping.LARGE_MONEY:
                    CEventManager.QueueEvent(new CEventPlayer_AddCoin(40 * Math.Min(CPlayerData.m_ShopLevel + 1, 25)));
                    return;
                case GenericItemMapping.SMALL_XP:
                    CEventManager.QueueEvent(new CEventPlayer_AddShopExp(Math.Min((int)(CPlayerData.GetExpRequiredToLevelUp() * 0.1), ((CPlayerData.m_ShopLevel + 1) > 20 ? (int)(300 * (CPlayerData.m_ShopLevel + 1) * 0.2) : 400))));
                    return;
                case GenericItemMapping.MEDIUM_XP:
                    CEventManager.QueueEvent(new CEventPlayer_AddShopExp(Math.Min((int)(CPlayerData.GetExpRequiredToLevelUp() * .17), ((CPlayerData.m_ShopLevel + 1) > 20 ? (int)(600 * (CPlayerData.m_ShopLevel + 1) * 0.2) : 800))));
                    return;
                case GenericItemMapping.LARGE_XP:
                    CEventManager.QueueEvent(new CEventPlayer_AddShopExp(Math.Min((int)(CPlayerData.GetExpRequiredToLevelUp() * 0.25), ((CPlayerData.m_ShopLevel + 1) > 20 ? (int)(1000 * (CPlayerData.m_ShopLevel + 1) * 0.2) : 1500))));
                    return;
                case GenericItemMapping.RANDOM_CARD:
                    CPlayerData.AddCard(Plugin.SaveHandler.NewRandomCard(), 1);
                    return;

                case GenericItemMapping.PROGRESSIVE_CUSTOMER_MONEY:
                    Plugin.SaveHandler.GetSaveData().CustomerMoneyMult += 0.1f;
                    return;
                case GenericItemMapping.INCREASE_CARD_LUCK:
                    if (Plugin.SaveHandler.GetSaveData().Luck >= 100)
                    {
                        CEventManager.QueueEvent(new CEventPlayer_AddCoin(400 * Math.Min(CPlayerData.m_ShopLevel + 1, 25)));
                    }
                    else
                    {
                        Plugin.SaveHandler.GetSaveData().Luck += 1;
                    }
                    return;
                case GenericItemMapping.DECREASE_CARD_LUCK_TRAP:
                    Plugin.SaveHandler.GetSaveData().Luck -= Plugin.SaveHandler.GetSaveData().Luck > 0 ? 1 : 0;
                    return;
                case GenericItemMapping.CURRENCY_TRAP:
                    //make this check to make sure you are not in a transaction
                    //CSingleton<CGameManager>.Instance.m_CurrencyType = (EMoneyCurrencyType)UnityEngine.Random.Range(0, 8);
                    return;
                case GenericItemMapping.STINK_TRAP:
                    List<Customer> list = CSingleton<CustomerManager>.Instance.m_CustomerList;
                    foreach (Customer c in list)
                    {
                        c.SetSmelly();
                    }
                    return;
                case GenericItemMapping.LIGHT_TRAP:
                    Util.Instance.StartCoroutine(ToggleLightMultipleTimes());
                    return;
                case GenericItemMapping.CREDIT_CARD_FAILURE_TRAP:
                    remainingTime += 60f;

                    if (!timerRunning)
                    {
                        cashOnlyCoroutine = Util.Instance.StartCoroutine(CashOnlyTimerCoroutine());
                    }
                    return;
                case GenericItemMapping.SHOP_EXPANSION_A:
                    if (Plugin.ArchipelagoHandler.slotData.AutoRenovate)
                    {
                        CSingleton<UnlockRoomManager>.Instance.StartUnlockNextRoom();
                    }
                    return;
                case GenericItemMapping.SHOP_EXPANSION_B:
                    if (!CPlayerData.m_IsWarehouseRoomUnlocked)
                    {
                        CSingleton<UnlockRoomManager>.Instance.SetUnlockWarehouseRoom(true);

                        SoundManager.PlayAudio("SFX_CustomerBuy", 0.6f);
                        return;
                    }
                    if (Plugin.ArchipelagoHandler.slotData.AutoRenovate)
                    {
                        CSingleton<UnlockRoomManager>.Instance.StartUnlockNextWarehouseRoom();

                    }
                    if (CPlayerData.m_IsWarehouseRoomUnlocked)
                    {
                        CSingleton<UnlockRoomManager>.Instance.EvaluateWarehouseRoomOpenClose();

                    }
                    return;
                case GenericItemMapping.SCANNER:
                    CPlayerData.m_IsScannerRestockUnlocked = true;
                    CEventManager.QueueEvent(new CEventPlayer_ScannerRestockUnlocked());
                    return;
                case GenericItemMapping.FIVE_GHOST_CARD:
                    addRandomGhosts(5);
                    return;
                case GenericItemMapping.FOUR_GHOST_CARD:
                    addRandomGhosts(4);
                    return;
                case GenericItemMapping.THREE_GHOST_CARD:
                    addRandomGhosts(3);
                    return;
                case GenericItemMapping.TWO_GHOST_CARD:
                    addRandomGhosts(2);
                    return;
                case GenericItemMapping.ONE_GHOST_CARD:
                    addRandomGhosts(1);
                    return;
            }

            if((int)PlayTableMapping.GetFormatFromInt((int)item.ItemId) != -1)
            {
                UIInfoPanel.getInstance().UpdateFormatAvailability(PlayTableMapping.GetFormatFromInt((int)item.ItemId));
            }


            if (item.ItemId == LicenseMapping.BASIC_CARD_PACK_ID || item.ItemId < (int)EItemType.Max)
            {
                EItemType type = item.ItemId == LicenseMapping.BASIC_CARD_PACK_ID ? EItemType.BasicCardPack : (EItemType)item.ItemId;
                int num = Plugin.ArchipelagoHandler.GetItemCount(item.ItemId);
                var list = InventoryBase.GetRestockDataUsingItemType(type);
                var indexList = InventoryBase.GetRestockDataIndexList(type);
                if (num == 1 && list.First().licenseShopLevelRequired <= CPlayerData.m_ShopLevel + 1)
                {
                    ItemData itemData = InventoryBase.GetItemData(type);
                    PopupTextPatches.ShowCustomText($"{itemData.GetName()} Now Available");
                }
                else if (num > 1 && indexList.Count > 1)
                {
                    CPlayerData.SetUnlockItemLicense(indexList[1]);
                }

                var screen = FindObjectOfType<RestockItemScreen>();
                if (screen != null)
                {
                    screen.OnPressChangePageButton(screen.m_PageIndex);

                }

                screen = FindObjectOfType<RestockItemBoardGameScreen>();
                if (screen != null)
                {
                    screen.OnPressChangePageButton(screen.m_PageIndex);

                }
                return;
            }


            if (EmployeeMapping.getindexFromId((int)item.ItemId) != -1)
            {
                int employeeIndex = EmployeeMapping.getindexFromId((int)item.ItemId);
                //cannot run uless level fully loaded
                var screen = FindObjectOfType<HireWorkerScreen>();

                HireWorkerPanelUI[] allpanels = FindObjectsOfType<HireWorkerPanelUI>();
                HireWorkerPanelUI panel = null;
                foreach (HireWorkerPanelUI screenItem in allpanels)
                {
                    if (screenItem.m_Index == employeeIndex)
                    {
                        panel = screenItem;
                        break;
                    }
                }
                if (panel == null)
                {
                    return;
                }
                panel.m_HiredText.SetActive(value: false);
                panel.m_PurchaseBtn.SetActive(value: true);
                return;
            }

            if (FurnatureMapping.Furnature.Where(f => f.id == item.ItemId).Any())
            {
                var screen = UnityEngine.Object.FindObjectOfType<FurnitureShopUIScreen>();
                if (screen != null)
                {
                    screen.Init();

                }
                
                if (FurnatureMapping.getIdFromType(EObjectType.CardShelf) == item.ItemId)
                {
                    Plugin.Logger.LogInfo("Card shelf unlocked, updating achievement availability");
                    Plugin.SaveHandler.GetAchievementHandler().UpdateAvailability(APLogicUtil.GetAllAvailablePacks());
                }

                if (FurnatureMapping.getIdFromType(EObjectType.PlayTable) == item.ItemId)
                {
                    Plugin.Logger.LogInfo("Play Table Unlocked");
                    UIInfoPanel.getInstance().UpdateFormatAvailability(EGameEventFormat.Standard);
                    UIInfoPanel.getInstance().UpdateFormatAvailability(EGameEventFormat.MAX);
                }
                return;
            }
        });
    }

    private void addRandomGhosts(int count)
    {
        for (int i = 0; i < count; i++)
        {
            List<EMonsterType> availabletypes = InventoryBase.GetShownMonsterList(ECardExpansionType.Ghost);
            Plugin.Logger.LogInfo($"Available ghost monster types: {string.Join(", ", availabletypes)}");
            CPlayerData.AddCard(new CardData
            {
                isFoil = UnityEngine.Random.Range(0F, 1F) > 0.5,
                isDestiny = UnityEngine.Random.Range(0F, 1F) > 0.5,
                borderType = ECardBorderType.Base,
                monsterType = InventoryBase.GetMonsterData(availabletypes[UnityEngine.Random.Range(0, availabletypes.Count())]).MonsterType,
                expansionType = ECardExpansionType.Ghost,
                isChampionCard = false,
                isNew = true
            }, 1);
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
