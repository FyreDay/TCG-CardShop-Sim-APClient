using ApClient.Archipelago;
using ApClient.Archipelago.Mapping;
using ApClient.Core.Archipelago;
using ApClient.mapping;
using ApClient.Patches.Functionality;
using ApClient.ui;
using Archipelago.MultiClient.Net.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ApClient;

public class ItemHandler
{
    ItemEffect scanner = new ItemEffect(
        (id) => { return id == GenericItemMapping.ITEMS_BLOCKERS_OFFSET + 1; },
        (id) =>
        {
            if (CPlayerData.m_IsScannerRestockUnlocked)
            {
                return;
            }
            CPlayerData.m_IsScannerRestockUnlocked = true;
            ScannerRestockScreen screen = FindObjectOfType<ScannerRestockScreen>();
            if (screen != null)
            {

                for (int i = 0; i < screen.m_LockedDisableList.Count; i++)
                {
                    screen.m_LockedDisableList[i].SetActive(value: true);
                }

                screen.EvaluatePanelUIPage(screen.m_PageIndex);
                screen.UpdateTotalCostAndBoxCount();
                screen.m_UnlockScreenAnim.Play();
            }

            CEventManager.QueueEvent(new CEventPlayer_ScannerRestockUnlocked());
            return;
        });

    ItemEffect ghosts = new ItemEffect(
        (id) => { return id >= GenericItemMapping.ITEMS_GHOST_OFFSET && id < GenericItemMapping.ITEMS_GHOST_OFFSET + 10; },
        (id) =>
        {
            addRandomGhosts(id - GenericItemMapping.ITEMS_GHOST_OFFSET);
        });

    ItemEffect licenses = new ItemEffect(
        (id) => { return id - GenericItemMapping.ITEMS_LICENSE_OFFSET < (int)EItemType.Max; },
        (id) =>
        {
            ProcessLicense((EItemType)(id - GenericItemMapping.ITEMS_LICENSE_OFFSET));
        });

    ItemEffect progressivelicenses = new ItemEffect(
        (id) => { return id - GenericItemMapping.ITEMS_PROGRESSIVE_LICENSE_OFFSET < (int)EItemType.Max; },
        (id) =>
        {
            throw new NotImplementedException("Progressive licenses are not implemented yet.");
        // ProcessLicense((EItemType)(id - GenericItemMapping.ITEMS_LICENSE_OFFSET));
        });

    ItemEffect formats = new ItemEffect(
        (id) => { return (int)PlayTableMapping.GetFormatFromInt((int)id) != -1; },
        (id) =>
        {
            UIInfoPanel.getInstance().UpdateFormatAvailability(PlayTableMapping.GetFormatFromInt((int)item.ItemId));
        });
    ItemEffect DIY = new ItemEffect(
        (id) => { return FurnatureMapping.Furnature.Where(f => f.id == item.ItemId).Any(); },
        (id) =>
        {
            var screen = FindObjectOfType<FurnitureShopUIScreen>();
            if (screen != null)
            {
                screen.Init();

            }

            if (FurnatureMapping.getIdFromType(EObjectType.CardShelf) == item.ItemId)
            {
                Plugin.SaveHandler.GetAchievementHandler().UpdateAvailability(APLogicUtil.GetAllAvailablePacks());
            }

            if (FurnatureMapping.getIdFromType(EObjectType.PlayTable) == item.ItemId)
            {
                UIInfoPanel.getInstance().CheckAllFormatsAvailability();
            }
            return;
        });

    ItemEffect employees = new ItemEffect(
        (id) => { return EmployeeMapping.getindexFromId((int)id - GenericItemMapping.ITEMS_WORKER_OFFSET) != -1; },
        (id) =>
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
        });

    ItemEffect shopExpansion = new ItemEffect(
        (id) => { return id == GenericItemMapping.ITEMS_EXPANSION_OFFSET + 0; },
        (id) =>
        {
            if (state.SlotData.AutoRenovate)
            {
                CSingleton<UnlockRoomManager>.Instance.StartUnlockNextRoom();
            }
            return;
        });

    ItemEffect warehouseExpansion = new ItemEffect(
        (id) => { return id == GenericItemMapping.ITEMS_EXPANSION_OFFSET + 1; },
        (id) =>
        {
            if (!CPlayerData.m_IsWarehouseRoomUnlocked)
            {
                CSingleton<UnlockRoomManager>.Instance.SetUnlockWarehouseRoom(true);

                SoundManager.PlayAudio("SFX_CustomerBuy", 0.6f);
                return;
            }
            if (state.SlotData.AutoRenovate)
            {
                CSingleton<UnlockRoomManager>.Instance.StartUnlockNextWarehouseRoom();

            }
            if (CPlayerData.m_IsWarehouseRoomUnlocked)
            {
                CSingleton<UnlockRoomManager>.Instance.EvaluateWarehouseRoomOpenClose();

            }
            return;
        });

    ItemEffect filler = new ItemEffect(
        (id) => { return id >= GenericItemMapping.ITEMS_FILLER_OFFSET && id < GenericItemMapping.ITEMS_FILLER_OFFSET + 999; },
        (id) =>
        {
            switch (id - GenericItemMapping.ITEMS_FILLER_OFFSET)
            {
                case 0:
                    CEventManager.QueueEvent(new CEventPlayer_AddCoin(10 * Math.Min(CPlayerData.m_ShopLevel + 1, 25)));
                    return;
                case 1:
                    CEventManager.QueueEvent(new CEventPlayer_AddCoin(20 * Math.Min(CPlayerData.m_ShopLevel + 1, 25)));
                    return;
                case 2:
                    CEventManager.QueueEvent(new CEventPlayer_AddCoin(40 * Math.Min(CPlayerData.m_ShopLevel + 1, 25)));
                    return;
                case 3:
                    CEventManager.QueueEvent(new CEventPlayer_AddShopExp(Math.Min((int)(CPlayerData.GetExpRequiredToLevelUp() * 0.1), CPlayerData.m_ShopLevel + 1 > 20 ? (int)(300 * (CPlayerData.m_ShopLevel + 1) * 0.2) : 400)));
                    return;
                case 4:
                    CEventManager.QueueEvent(new CEventPlayer_AddShopExp(Math.Min((int)(CPlayerData.GetExpRequiredToLevelUp() * .17), CPlayerData.m_ShopLevel + 1 > 20 ? (int)(600 * (CPlayerData.m_ShopLevel + 1) * 0.2) : 800)));
                    return;
                case 5:
                    CEventManager.QueueEvent(new CEventPlayer_AddShopExp(Math.Min((int)(CPlayerData.GetExpRequiredToLevelUp() * 0.25), CPlayerData.m_ShopLevel + 1 > 20 ? (int)(1000 * (CPlayerData.m_ShopLevel + 1) * 0.2) : 1500)));
                    return;
                case 6:
                    CPlayerData.AddCard(Plugin.SaveHandler.NewRandomCard(), 1);
                    return;

                case 7:
                    Plugin.SaveHandler.GetSaveData().CustomerMoneyMult += 0.1f;
                    return;
                case 8:
                    if (Plugin.SaveHandler.GetSaveData().Luck >= 100)
                    {
                        CEventManager.QueueEvent(new CEventPlayer_AddCoin(400 * Math.Min(CPlayerData.m_ShopLevel + 1, 25)));
                    }
                    else
                    {
                        Plugin.SaveHandler.GetSaveData().Luck += 1;
                    }
                    return;

            }
        });

    ItemEffect traps = new ItemEffect(
        (id) => { return id >= GenericItemMapping.ITEMS_TRAP_OFFSET && id < GenericItemMapping.ITEMS_TRAP_OFFSET + 999; },
        (id) =>
        {
            switch (id - GenericItemMapping.ITEMS_TRAP_OFFSET)
            {
                case 0:
                    List<Customer> list = CSingleton<CustomerManager>.Instance.m_CustomerList;
                    foreach (Customer c in list)
                    {
                        c.SetSmelly();
                    }
                    return;
                case 1:
                    StartCoroutine(ToggleLightMultipleTimes());
                    return;
                case 2:
                    remainingTime += 60f;

                    if (!timerRunning)
                    {
                        cashOnlyCoroutine = StartCoroutine(CashOnlyTimerCoroutine());
                    }
                    return;
                case 3:
                    Plugin.SaveHandler.GetSaveData().Luck -= Plugin.SaveHandler.GetSaveData().Luck > 0 ? 1 : 0;
                    return;
                case 4:
                    CEventManager.QueueEvent(new CEventPlayer_AddShopExp(Math.Min((int)(CPlayerData.GetExpRequiredToLevelUp() * .17), CPlayerData.m_ShopLevel + 1 > 20 ? (int)(600 * (CPlayerData.m_ShopLevel + 1) * 0.2) : 800)));
                    return;
                case 5:
                    //make this check to make sure you are not in a transaction
                    //CSingleton<CGameManager>.Instance.m_CurrencyType = (EMoneyCurrencyType)UnityEngine.Random.Range(0, 8);
                    return;
                case 6:
                    CPlayerData.AddCard(Plugin.SaveHandler.NewRandomCard(), 1);
                    return;

                case 7:
                    Plugin.SaveHandler.GetSaveData().CustomerMoneyMult += 0.1f;
                    return;
                case 8:
                    if (Plugin.SaveHandler.GetSaveData().Luck >= 100)
                    {
                        CEventManager.QueueEvent(new CEventPlayer_AddCoin(400 * Math.Min(CPlayerData.m_ShopLevel + 1, 25)));
                    }
                    else
                    {
                        Plugin.SaveHandler.GetSaveData().Luck += 1;
                    }
                    return;

            }
        });

    public static void ProcessLicense(EItemType type)
    {
        int num = state.GetItemCount(item.ItemId);
        var list = InventoryBase.GetRestockDataUsingItemType(type);
        var indexList = InventoryBase.GetRestockDataIndexList(type);
        if (num == 1 && list.First().licenseShopLevelRequired <= CPlayerData.m_ShopLevel + 1)
        {
            ItemData itemData = InventoryBase.GetItemData(type);
            PopupTextPatches.ShowCustomText($"{itemData.GetName()} Now Available");
        }
        else if (num > 1 && indexList.Count > 1)
        {
            if (CPlayerData.GetIsItemLicenseUnlocked(indexList[0]))
            {
                CPlayerData.SetUnlockItemLicense(indexList[1]);
            }
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

        if (state.SlotData.Goal == 1)
        {
            Plugin.SaveHandler.CheckForCardCollectionGoal();
        }
    }

    public void registerItemEffects(ArchipelagoState state)
    {
        state.RegisterItemEffect(filler);
        state.RegisterItemEffect(traps);

    }

    private void addRandomGhosts(long count)
    {
        for (int i = 0; i < count; i++)
        {
            List<EMonsterType> availabletypes = InventoryBase.GetShownMonsterList(ECardExpansionType.Ghost);
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

    private static IEnumerator ToggleLightMultipleTimes()
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
