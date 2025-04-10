﻿using ApClient.data;
using ApClient.mapping;
using ApClient.patches;
using Archipelago.MultiClient.Net.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine.UIElements;
using static System.Collections.Specialized.BitVector32;

namespace ApClient;

public class ItemHandler
{

    public void processNewItem(ItemInfo itemReceived)
    {
        Plugin.Log(itemReceived.ItemName);
        if (LicenseMapping.getKeyValue((int)itemReceived.ItemId).Key != -1)
        {
            PopupTextPatches.ShowCustomText("New License Unlocked");
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
            Plugin.Log($"Recieved Item While panel was open: {(int)itemReceived.ItemId} and {itemMapping.Key}");
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
        }
        //Log($"Before Employee check: {EmployeeMapping.getKeyValue((int)itemReceived.ItemId).Key}");
        if (EmployeeMapping.getKeyValue((int)itemReceived.ItemId).Key != -1)
        {
            var itemMapping = EmployeeMapping.getKeyValue((int)itemReceived.ItemId);
            Plugin.Log($"worker recieved id: {EmployeeMapping.getKeyValue((int)itemReceived.ItemId).Key}");
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
        }
        if ((int)itemReceived.ItemId == CardMapping.ghostProgressive)
        {
            bool isDestiny = false;
            int total = 0;
            List<bool> collectedlist = CPlayerData.GetIsCardCollectedList(ECardExpansionType.Ghost, isDestiny);
            total = collectedlist.FindAll(i => i == true).Count;
            if (total >= 36)
            {
                isDestiny = true;
                collectedlist = CPlayerData.GetIsCardCollectedList(ECardExpansionType.Ghost, isDestiny);
                total += collectedlist.FindAll(i => i == true).Count;
            }


            if (Plugin.m_SessionHandler.GetSlotData().Goal == 2 && Plugin.m_SessionHandler.GetSlotData().GhostGoalAmount <= total)
            {
                Plugin.m_SessionHandler.SendGoalCompletion();
                PopupTextPatches.ShowCustomText($"Congrats! You Collected {total} Ghost Cards and Completed Your Goal!");
            }
            var list = InventoryBase.GetShownMonsterList(ECardExpansionType.Ghost);

            bool isFoil = false;
            int index = 0;
            for (int i = 0; i < list.Count; i++)
            {
                int dataindex = (int)(i * CPlayerData.GetCardAmountPerMonsterType(ECardExpansionType.Ghost) + ECardBorderType.FullArt);
                if (collectedlist[dataindex])
                {
                    dataindex += CPlayerData.GetCardAmountPerMonsterType(ECardExpansionType.Ghost, includeFoilCount: false);
                    if (!collectedlist[dataindex])
                    {
                        isFoil = true;
                        index = i;
                        break;
                    }
                }
                else
                {
                    index = i;
                    break;
                }
            }


            CPlayerData.AddCard(new CardData
            {
                isFoil = isFoil,
                isDestiny = isDestiny,
                borderType = ECardBorderType.FullArt,
                monsterType = list[index],
                expansionType = ECardExpansionType.Ghost,
                isChampionCard = false,
                isNew = true
            }, 1);

        }



        if ((int)itemReceived.ItemId == ExpansionMapping.warehouseKey)
        {
            CSingleton<UnlockRoomManager>.Instance.SetUnlockWarehouseRoom(isUnlocked: true);
            AchievementManager.OnShopLotBUnlocked();
            CPlayerData.m_GameReportDataCollect.upgradeCost -= 5000;
            CPlayerData.m_GameReportDataCollectPermanent.upgradeCost -= 5000;
            SoundManager.PlayAudio("SFX_CustomerBuy", 0.6f);
            PopupTextPatches.ShowCustomText("Warehouse Key Found");
        }
        if ((int)itemReceived.ItemId == TrashMapping.smallMoney)
        {
            CEventManager.QueueEvent(new CEventPlayer_AddCoin(10 * (CPlayerData.m_ShopLevel + 1)));
        }
        if ((int)itemReceived.ItemId == TrashMapping.mediumMoney)
        {
            CEventManager.QueueEvent(new CEventPlayer_AddCoin(20 * (CPlayerData.m_ShopLevel + 1)));
        }
        if ((int)itemReceived.ItemId == TrashMapping.largeMoney)
        {
            CEventManager.QueueEvent(new CEventPlayer_AddCoin(40 * (CPlayerData.m_ShopLevel + 1)));
        }
        if ((int)itemReceived.ItemId == TrashMapping.smallXp)
        {
            CEventManager.QueueEvent(new CEventPlayer_AddShopExp((int)(CPlayerData.GetExpRequiredToLevelUp() * 0.1)));
        }
        if ((int)itemReceived.ItemId == TrashMapping.mediumXp)
        {
            CEventManager.QueueEvent(new CEventPlayer_AddShopExp((int)(CPlayerData.GetExpRequiredToLevelUp() * 0.2)));
        }
        if ((int)itemReceived.ItemId == TrashMapping.largeXp)
        {
            CEventManager.QueueEvent(new CEventPlayer_AddShopExp((int)(CPlayerData.GetExpRequiredToLevelUp() * 0.4)));
        }
        if ((int)itemReceived.ItemId == TrashMapping.randomcard)
        {
            var packlist = Enum.GetValues(typeof(ECollectionPackType));
            var packType = (ECollectionPackType)packlist.GetValue(UnityEngine.Random.Range(0, Plugin.m_SessionHandler.GetSlotData().CardSanity == 0 ? 8 : Plugin.m_SessionHandler.GetSlotData().CardSanity));
            CPlayerData.AddCard(cardRoller(packType), 1);
        }
        if ((int)itemReceived.ItemId == TrashMapping.randomNewCard)
        {
            CardData d = RandomNewCard();
            Plugin.Log($"Card is: {d.monsterType} and {d.expansionType}");
            CPlayerData.AddCard(d, 1);
            //InteractableCashierCounter

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
        }
        if ((int)itemReceived.ItemId == TrashMapping.lightTrap)
        {
            SoundManager.PlayAudio("SFX_ButtonLightTap", 0.6f, 0.5f);
            //CSingleton<LightManager>.Instance.m_ShoplightGrp.SetActive(true);
            CSingleton<LightManager>.Instance.ToggleShopLight();
        }
    }

    public CardData RandomNewCard()
    {
        Plugin.Log("random new card");
        int border_sanity = 5;
        bool foil_sanity = true;
        if (Plugin.m_SessionHandler.GetSlotData().CardSanity != 0){
            border_sanity = Plugin.m_SessionHandler.GetSlotData().BorderInSanity;
            foil_sanity = Plugin.m_SessionHandler.GetSlotData().FoilInSanity;
        }
        
        List<int> incompletecards = Plugin.m_SaveManager.GetIncompleteCards();
        //Check for old saves
        if(incompletecards.Count == 0)
        {
            Plugin.m_SaveManager.setIncompleteCards(PlayerDataPatches.GetValidTypeIdsForSanity());
            incompletecards = Plugin.m_SaveManager.GetIncompleteCards();
        }

        int cardId = incompletecards[UnityEngine.Random.Range(0, incompletecards.Count)];

        int index = (cardId-1) * CPlayerData.GetCardAmountPerMonsterType(ECardExpansionType.Tetramon);
        List<int> t_falseIndexes = CPlayerData.GetIsCardCollectedList(ECardExpansionType.Tetramon, false).GetRange(index, foil_sanity ? 12 : 6).Select((val, idx) => new { val, idx })
            .Where(x => !x.val && x.idx % 6 <= border_sanity)
            .Select(x => x.idx)
            .ToList();

        index = (cardId - 1) * CPlayerData.GetCardAmountPerMonsterType(ECardExpansionType.Destiny);
        List<int> d_falseIndexes = CPlayerData.GetIsCardCollectedList(ECardExpansionType.Destiny, false).GetRange(index, foil_sanity ? 12 : 6)
            .Select((val, idx) => new { val, idx })
            .Where(x => !x.val && x.idx % 6 <= border_sanity)
            .Select(x => x.idx)
            .ToList();

        int borderId = 0;
        bool isDestiny = false;
        if (t_falseIndexes.Count > 0)
        {
            borderId = t_falseIndexes[UnityEngine.Random.Range(0, t_falseIndexes.Count)];
        }
        else if (d_falseIndexes.Count > 0)
        {
            borderId = d_falseIndexes[UnityEngine.Random.Range(0, d_falseIndexes.Count)];
            isDestiny = true;
            if(d_falseIndexes.Count == 1)
            {
                Plugin.m_SaveManager.CompleteCardId(cardId);
            }
        }
        else
        {
            Plugin.Log($"You have collected all Cards for {(EMonsterType)cardId}");
            Plugin.m_SaveManager.CompleteCardId(cardId);
            return cardRoller(ECollectionPackType.DestinyLegendaryCardPack);
        }
        ECardExpansionType cardExpansionType = isDestiny ? ECardExpansionType.Destiny : ECardExpansionType.Tetramon;
        return new CardData
        {
            isFoil = borderId > 5,
            isDestiny = isDestiny,
            borderType = CPlayerData.GetCardBorderType(borderId % 6, cardExpansionType),
            monsterType = (EMonsterType)cardId,
            expansionType = cardExpansionType,
            isChampionCard = false,
            isNew = true
        };
    }
    private static CardData cardRoller(ECollectionPackType collectionPackType)
    {
        ECardExpansionType expansionType = UnityEngine.Random.Range(0F, 1F) > 0.5 ? ECardExpansionType.Tetramon : ECardExpansionType.Destiny;
        return new CardData
        {
            isFoil = UnityEngine.Random.Range(0F,1F) > 0.5,
            isDestiny = expansionType == ECardExpansionType.Destiny,
            borderType = (ECardBorderType)UnityEngine.Random.Range(0, 7),
            monsterType = (EMonsterType)UnityEngine.Random.Range(0, (int)EMonsterType.MAX),
            expansionType = expansionType,
            isChampionCard = false,
            isNew = true
        };
    }
}
