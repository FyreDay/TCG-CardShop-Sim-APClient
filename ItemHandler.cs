using ApClient.data;
using ApClient.mapping;
using ApClient.patches;
using Archipelago.MultiClient.Net.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static System.Collections.Specialized.BitVector32;

namespace ApClient;

public class ItemHandler
{

    public void processNewItem(ItemInfo itemReceived)
    {
        //Plugin.Log(itemReceived.ItemName);

        if ((int)itemReceived.ItemId == ExpansionMapping.warehouseKey)
        {
            CoroutineRunner.RunOnMainThread(() =>
            {
                CSingleton<UnlockRoomManager>.Instance.SetUnlockWarehouseRoom(true);
            });

            SoundManager.PlayAudio("SFX_CustomerBuy", 0.6f);
            return;
        }
        if ((int)itemReceived.ItemId == TrashMapping.smallMoney)
        {
            CEventManager.QueueEvent(new CEventPlayer_AddCoin(10 * Math.Min(CPlayerData.m_ShopLevel + 1, 25)));
            return;
        }
        if ((int)itemReceived.ItemId == TrashMapping.mediumMoney)
        {
            CEventManager.QueueEvent(new CEventPlayer_AddCoin(20 * Math.Min(CPlayerData.m_ShopLevel + 1, 25)));
            return;
        }
        if ((int)itemReceived.ItemId == TrashMapping.largeMoney)
        {
            CEventManager.QueueEvent(new CEventPlayer_AddCoin(40 * Math.Min(CPlayerData.m_ShopLevel + 1, 25)));
            return;
        }
        if ((int)itemReceived.ItemId == TrashMapping.smallXp)
        {
            CEventManager.QueueEvent(new CEventPlayer_AddShopExp(Math.Min((int)(CPlayerData.GetExpRequiredToLevelUp() * 0.1), ((CPlayerData.m_ShopLevel + 1) > 20 ? (int)(300 * (CPlayerData.m_ShopLevel + 1) * 0.2) : 400))));
            return;
        }
        if ((int)itemReceived.ItemId == TrashMapping.mediumXp)
        {
            CEventManager.QueueEvent(new CEventPlayer_AddShopExp(Math.Min((int)(CPlayerData.GetExpRequiredToLevelUp() * .17), ((CPlayerData.m_ShopLevel + 1) > 20 ? (int)(600 * (CPlayerData.m_ShopLevel + 1) * 0.2) : 800))));
            return;
        }
        if ((int)itemReceived.ItemId == TrashMapping.largeXp)
        {
            CEventManager.QueueEvent(new CEventPlayer_AddShopExp(Math.Min((int)(CPlayerData.GetExpRequiredToLevelUp() * 0.25), ((CPlayerData.m_ShopLevel + 1) > 20 ? (int)(1000 * (CPlayerData.m_ShopLevel + 1) * 0.2) : 1500))));
            return;
        }
        if ((int)itemReceived.ItemId == TrashMapping.randomcard)
        {
            var packlist = Enum.GetValues(typeof(ECollectionPackType));
            var packType = (ECollectionPackType)packlist.GetValue(UnityEngine.Random.Range(0, Plugin.m_SessionHandler.GetSlotData().CardSanity == 0 ? 8 : Plugin.m_SessionHandler.GetSlotData().CardSanity));
            CardData card = Plugin.m_CardHelper.CardRoller(packType);
            CoroutineRunner.RunOnMainThread(() =>
            {
                CPlayerData.AddCard(card, 1);
            });
            return;
        }
        if ((int)itemReceived.ItemId == TrashMapping.randomNewCard)
        {
            int expansion_limit = 8;
            int border_sanity = 5;
            bool foil_sanity = true;
            if (Plugin.m_SessionHandler.GetSlotData().CardSanity != 0)
            {
                border_sanity = Plugin.m_SessionHandler.GetSlotData().BorderInSanity;
                foil_sanity = Plugin.m_SessionHandler.GetSlotData().FoilInSanity;
                expansion_limit = Plugin.m_SessionHandler.GetSlotData().CardSanity;
            }
            

            ECardExpansionType expansion = UnityEngine.Random.Range(0, expansion_limit) > 4 ? ECardExpansionType.Destiny : ECardExpansionType.Tetramon;

            HashSet<ERarity> desiredRarities = new HashSet<ERarity>();

            if(expansion == ECardExpansionType.Destiny)
            {
                for (int i = 0; i < expansion_limit - 4; i++)
                {
                    desiredRarities.Add((ERarity)i);
                }
            }

            if (expansion == ECardExpansionType.Tetramon)
            {
                for (int i = 0; i < expansion_limit && i < 4; i++)
                {
                    desiredRarities.Add((ERarity)i);
                }
            }

            //Plugin.Log($"Card is: {d.monsterType} and {d.expansionType}");
            CardData card = Plugin.m_CardHelper.RandomNewCard(expansion, desiredRarities, border_sanity,foil_sanity);
            CoroutineRunner.RunOnMainThread(() =>
            {
                CPlayerData.AddCard(card, 1);
            });
            return;

        }
        if (PlayTableMapping.GetFormatFromInt((int)itemReceived.ItemId) != EGameEventFormat.None)
        {
            Plugin.m_SaveManager.setEventUnlocked(PlayTableMapping.GetFormatFromInt((int)itemReceived.ItemId));
            return;
        }

        if((int)itemReceived.ItemId == TrashMapping.ProgressiveCustomerMoney)
        {
            Plugin.m_SaveManager.IncreaseMoneyMult();
            return;
        }

        if ((int)itemReceived.ItemId == TrashMapping.IncreaseCardLuck)
        {
            if (Plugin.m_SaveManager.GetLuck() >= 100)
            {
                CEventManager.QueueEvent(new CEventPlayer_AddCoin(40 * Math.Min(CPlayerData.m_ShopLevel + 1, 25)));
            }
            else
            {
                Plugin.m_SaveManager.IncreaseLuck();
            }
            return;
        }

        if ((int)itemReceived.ItemId == TrashMapping.DecreaseCardLuck)
        {
            Plugin.m_SaveManager.DecreaseLuck();
            return;
        }
        if ((int)itemReceived.ItemId == TrashMapping.CurrencyTrap)
        {
            CSingleton<CGameManager>.Instance.m_CurrencyType = (EMoneyCurrencyType)UnityEngine.Random.RandomRangeInt(0, 8);
            return;
        }

        if ((int)itemReceived.ItemId == TrashMapping.stinkTrap)
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
        if ((int)itemReceived.ItemId == TrashMapping.lightTrap)
        {

            CoroutineRunner.Instance.StartCoroutine(ToggleLightMultipleTimes());
            return;
        }

        if ((int)itemReceived.ItemId == TrashMapping.CreditCardFailure)
        {

            remainingTime += 60f;

            if (!timerRunning)
            {
                cashOnlyCoroutine = CoroutineRunner.Instance.StartCoroutine(CashOnlyTimerCoroutine());
            }
            return;
        }

        if ((int)itemReceived.ItemId == TrashMapping.MarketChangeTrap)
        {
            CSingleton<PriceChangeManager>.Instance.EvaluatePriceChange();
            CSingleton<PriceChangeManager>.Instance.EvaluatePriceCrash();
            CPlayerData.UpdateItemPricePercentChange();
            CPlayerData.UpdatePastCardPricePercentChange();
            return;
        }

        if (LicenseMapping.getKeyValue((int)itemReceived.ItemId).Key != -1)
        {
            var itemMapping = LicenseMapping.getKeyValue((int)itemReceived.ItemId, Plugin.m_SessionHandler.itemCount((int)itemReceived.ItemId));
            RestockItemPanelUI panel = null;
            //update Restock ui
            RestockItemPanelUI[] screen = UnityEngine.Object.FindObjectsOfType<RestockItemPanelUI>();
            foreach (RestockItemPanelUI screenItem in screen)
            {
                if (screenItem.GetIndex() == itemMapping.Key)
                {
                    panel = screenItem;
                    break;
                }
            }
            if (panel == null)
            {
                return;
            }

            RestockItemPanelUIPatches.runLicenseBtnLogic(panel, true, itemMapping.Key);
            //Plugin.Log($"Recieved Item: {(int)itemReceived.ItemId} and {itemMapping.Key}");
            return;
        }
        if ((int)itemReceived.ItemId == ExpansionMapping.progressiveA)
        {
            ExpansionShopUIScreen screen = UnityEngine.Object.FindObjectOfType<ExpansionShopUIScreen>();
            if (screen != null)
            {
                FieldInfo field = typeof(ExpansionShopUIScreen).GetField("m_IsShopB", BindingFlags.NonPublic | BindingFlags.Instance);
                bool isB = (bool)field.GetValue(screen);
                if (!isB)
                {
                    ExpansionShopPanelUI panel = screen.m_ExpansionShopPanelUIList[Plugin.m_SessionHandler.itemCount((int)itemReceived.ItemId) - 1];
                    panel.m_LockPurchaseBtn.gameObject.SetActive(value: false);
                    panel.m_PurchasedBtn.gameObject.SetActive(value: false);
                    //Log($"Recieved Progressive A While panel was open: {(int)itemReceived.ItemId}");
                }

            }
            return;
        }
        if ((int)itemReceived.ItemId == ExpansionMapping.progressiveB)
        {
            ExpansionShopUIScreen screen = UnityEngine.Object.FindObjectOfType<ExpansionShopUIScreen>();
            if (screen != null)
            {
                FieldInfo field = typeof(ExpansionShopUIScreen).GetField("m_IsShopB", BindingFlags.NonPublic | BindingFlags.Instance);
                bool isB = (bool)field.GetValue(screen);
                if (isB)
                {
                    ExpansionShopPanelUI panel = screen.m_ExpansionShopPanelUIList[Plugin.m_SessionHandler.itemCount((int)itemReceived.ItemId) - 1];
                    FieldInfo fieldInfo = typeof(ExpansionShopPanelUI).GetField("m_LevelRequired", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (fieldInfo == null)
                    {
                        return;
                    }

                    int m_LevelRequired = (int)fieldInfo.GetValue(panel);
                    bool atLevel = CPlayerData.m_ShopLevel + 1 < m_LevelRequired;
                    if (atLevel)
                    {
                        panel.m_LockPurchaseBtn.gameObject.SetActive(value: false);
                        panel.m_PurchasedBtn.gameObject.SetActive(value: false);
                        panel.m_LevelRequirementText.gameObject.SetActive(value: false);
                    }
                    else
                    {
                        panel.m_LevelRequirementText.text = $"Shop Level {panel.m_LevelRequired} Required";
                    }
                }
            }
            return;
        }
        //Log($"Before Employee check: {EmployeeMapping.getKeyValue((int)itemReceived.ItemId).Key}");
        if (EmployeeMapping.getKeyValue((int)itemReceived.ItemId).Key != -1)
        {
            var itemMapping = EmployeeMapping.getKeyValue((int)itemReceived.ItemId);
            //cannot run uless level fully loaded
            var screen = UnityEngine.Object.FindObjectOfType<HireWorkerScreen>();

            HireWorkerPanelUI[] allpanels = UnityEngine.Object.FindObjectsOfType<HireWorkerPanelUI>();
            HireWorkerPanelUI panel = null;
            foreach (HireWorkerPanelUI screenItem in allpanels)
            {
                FieldInfo fieldInfo = typeof(HireWorkerPanelUI).GetField("m_Index", BindingFlags.NonPublic | BindingFlags.Instance);
                if (fieldInfo == null)
                {
                    return;
                }

                int m_Index = (int)fieldInfo.GetValue(screenItem);
                if (m_Index == itemMapping.Key)
                {
                    panel = screenItem;
                    break;
                }
            }
            if (panel == null)
            {
                return;
            }
            Plugin.Log("detected Hire Worker Screen");

            Plugin.Log("Found Hire Worker Panel");
            panel.m_HiredText.SetActive(value: false);
            panel.m_PurchaseBtn.SetActive(value: true);
            //panel.Init(screen, itemMapping.Key);
            //EmployeePatches.HireEmployee(panel, itemMapping.Key);
            Plugin.Log($"Recieved Worker While panel was open: {(int)itemReceived.ItemId}");

            //SoundManager.PlayAudio("SFX_CustomerBuy", 0.6f);
            return;
        }

        if (FurnatureMapping.getKeyValue((int)itemReceived.ItemId).Key != -1)
        {
            var itemMapping = FurnatureMapping.getKeyValue((int)itemReceived.ItemId, Plugin.m_SessionHandler.itemCount((int)itemReceived.ItemId));
            FurnitureShopPanelUI panel = null;
            //update Restock ui
            FurnitureShopPanelUI[] screen = UnityEngine.Object.FindObjectsOfType<FurnitureShopPanelUI>();
            foreach (FurnitureShopPanelUI screenItem in screen)
            {
                FieldInfo fieldInfo = typeof(FurnitureShopPanelUI).GetField("m_Index", BindingFlags.NonPublic | BindingFlags.Instance);
                if (fieldInfo == null)
                {
                    return;
                }

                int m_Index = (int)fieldInfo.GetValue(screenItem);
                if (m_Index == itemMapping.Key)
                {
                    panel = screenItem;
                    break;
                }
            }
            if (panel == null)
            {
                return;
            }
            FurnaturePatches.EnableFurnature(panel, itemMapping.Key);
            return;
        }
        if ((int)itemReceived.ItemId == CardMapping.oneghostcard)
        {
            
            addRandomGhost();

            return;
        }
        if ((int)itemReceived.ItemId == CardMapping.twoghostcard)
        {
            for (int i = 0; i < 2; i++)
            {
                addRandomGhost();
            }
            return;
        }
        if ((int)itemReceived.ItemId == CardMapping.threeghostcard)
        {
            for (int i = 0; i < 3; i++)
            {
                addRandomGhost();
            }

            return;
        }
        if ((int)itemReceived.ItemId == CardMapping.fourghostcard)
        {
            for (int i = 0; i < 4; i++)
            {
                addRandomGhost();
            }
            return;
        }
        if ((int)itemReceived.ItemId == CardMapping.fiveghostcard)
        {
            for(int i = 0; i< 5; i++)
            {
                addRandomGhost();
            }
            return;
        }
    }

    private void addRandomGhost()
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
}
