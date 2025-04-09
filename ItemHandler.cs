using ApClient.mapping;
using ApClient.patches;
using Archipelago.MultiClient.Net.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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
            CPlayerData.AddCard(RandomNewCard(), 1);
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

    private List<EMonsterType> m_MonsterTypes = new List<EMonsterType>();
    private List<EMonsterType> m_DestinyMonsterTypes = new List<EMonsterType>();

    public void setRandomTypesForSanity()
    {
        List<EMonsterType> typeList = new List<EMonsterType>();
        ECardExpansionType[] cardExpansionTypes = [ECardExpansionType.Tetramon, ECardExpansionType.Destiny];

        foreach (ECardExpansionType cardExpansionType in cardExpansionTypes)
        {
            for (int i = 0; i < InventoryBase.GetShownMonsterList(cardExpansionType).Count; i++)
            {
                EMonsterType mType = InventoryBase.GetMonsterData(InventoryBase.GetShownMonsterList(cardExpansionType)[i]).MonsterType;
                ERarity rarity = InventoryBase.GetMonsterData(InventoryBase.GetShownMonsterList(cardExpansionType)[i]).Rarity;
                switch (rarity)
                {
                    case ERarity.Legendary:
                        if ((Plugin.m_SessionHandler.GetSlotData().CardSanity >= 8 && cardExpansionType == ECardExpansionType.Destiny)
                            || (Plugin.m_SessionHandler.GetSlotData().CardSanity >= 4 && cardExpansionType == ECardExpansionType.Tetramon))
                        {
                            typeList.Add(mType);
                        }
                        break;
                    case ERarity.Epic:
                        if ((Plugin.m_SessionHandler.GetSlotData().CardSanity >= 7 && cardExpansionType == ECardExpansionType.Destiny)
                            || (Plugin.m_SessionHandler.GetSlotData().CardSanity >= 3 && cardExpansionType == ECardExpansionType.Tetramon))
                        {
                            typeList.Add(mType);
                        }
                        break;
                    case ERarity.Rare:
                        if ((Plugin.m_SessionHandler.GetSlotData().CardSanity >= 6 && cardExpansionType == ECardExpansionType.Destiny)
                            || (Plugin.m_SessionHandler.GetSlotData().CardSanity >= 2 && cardExpansionType == ECardExpansionType.Tetramon))
                        {
                            typeList.Add(mType);
                        }
                        break;
                    default:
                        if ((Plugin.m_SessionHandler.GetSlotData().CardSanity >= 5 && cardExpansionType == ECardExpansionType.Destiny)
                            || (Plugin.m_SessionHandler.GetSlotData().CardSanity >= 1 && cardExpansionType == ECardExpansionType.Tetramon))
                        {
                            typeList.Add(mType);
                        }
                        break;
                }
            }
            if (cardExpansionType == ECardExpansionType.Tetramon)
            {
                m_MonsterTypes = typeList;
            }
            else
            {
                m_DestinyMonsterTypes = typeList;
            }

        }
    }
    public CardData RandomNewCard()
    {
        Plugin.Log("Random New Card Generating");
        System.Random rand = new System.Random();

        ECardExpansionType expansion = (Plugin.m_SessionHandler.GetSlotData().CardSanity < 5 || rand.NextDouble() >= 0.5) ? ECardExpansionType.Tetramon : ECardExpansionType.Destiny;
        List<bool> boolList = CPlayerData.GetIsCardCollectedList(expansion, false);

        // Allowed types as an enum list
        List<EMonsterType> allowedTypes = expansion == ECardExpansionType.Tetramon ? m_MonsterTypes : m_DestinyMonsterTypes;
        // Convert enum list to integer indices
        List<int> allowedIndices = allowedTypes.Select(type => (int)type).ToList();

        // Collect valid False indices within allowed type ranges
        List<int> falseIndices = new List<int>();
        foreach (int typeIndex in allowedIndices)
        {
            int start = typeIndex * 12;
            int end = start + 12;

            for (int i = start; i < end && i < 1452; i++)
            {
                if (!boolList[i])
                    falseIndices.Add(i);
            }
        }

        if (falseIndices.Count == 0)
        {
            expansion = ECardExpansionType.Tetramon == expansion ? ECardExpansionType.Destiny : ECardExpansionType.Tetramon;
            boolList = CPlayerData.GetIsCardCollectedList(expansion, false);

            // Allowed types as an enum list
            allowedTypes = expansion == ECardExpansionType.Tetramon ? m_MonsterTypes : m_DestinyMonsterTypes;
            // Convert enum list to integer indices
            allowedIndices = allowedTypes.Select(type => (int)type).ToList();

            // Collect valid False indices within allowed type ranges
            falseIndices = new List<int>();
            foreach (int typeIndex in allowedIndices)
            {
                int start = typeIndex * 12;
                int end = start + 12;

                for (int i = start; i < end && i < 1452; i++)
                {
                    if (!boolList[i])
                        falseIndices.Add(i);
                }
            }
            if (falseIndices.Count == 0)
            {
                Plugin.Log("You have collected all Cards");
                return cardRoller(ECollectionPackType.DestinyLegendaryCardPack);
            }
        }

        // Randomly pick an index

        int selectedIndex = falseIndices[rand.Next(falseIndices.Count)];
        int type = selectedIndex % 12;
        var monsterType = CPlayerData.GetMonsterTypeFromCardSaveIndex(selectedIndex, expansion);
        Plugin.Log($"Randomly selected False index: {selectedIndex} which is a {monsterType}");

        return new CardData
        {
            isFoil = type > 5,
            isDestiny = expansion == ECardExpansionType.Destiny,
            borderType = CPlayerData.GetCardBorderType(type % 6, expansion),
            monsterType = monsterType,
            expansionType = expansion,
            isChampionCard = false,
            isNew = true
        };
    }
    private static CardData cardRoller(ECollectionPackType collectionPackType)
    {
        List<EMonsterType> commonlist = new List<EMonsterType>();
        List<EMonsterType> rarelist = new List<EMonsterType>();
        List<EMonsterType> epiclist = new List<EMonsterType>();
        List<EMonsterType> legendlist = new List<EMonsterType>();
        ECardExpansionType cardExpansionType = InventoryBase.GetCardExpansionType(collectionPackType);

        for (int i = 0; i < InventoryBase.GetShownMonsterList(cardExpansionType).Count; i++)
        {
            EMonsterType monsterType = InventoryBase.GetMonsterData(InventoryBase.GetShownMonsterList(cardExpansionType)[i]).MonsterType;
            ERarity rarity = InventoryBase.GetMonsterData(InventoryBase.GetShownMonsterList(cardExpansionType)[i]).Rarity;
            switch (rarity)
            {
                case ERarity.Legendary:
                    legendlist.Add(monsterType);
                    break;
                case ERarity.Epic:
                    epiclist.Add(monsterType);
                    break;
                case ERarity.Rare:
                    rarelist.Add(monsterType);
                    break;
                default:
                    commonlist.Add(monsterType);
                    break;
            }
        }
        System.Random randomGenerator = new System.Random();

        var vb = Enum.GetValues(typeof(ECardBorderType));
        var border = (ECardBorderType)vb.GetValue(randomGenerator.Next(vb.Length));

        var monster = EMonsterType.BatA;

        switch (collectionPackType)
        {
            case ECollectionPackType.BasicCardPack:
                monster = commonlist[UnityEngine.Random.Range(0, commonlist.Count)];
                break;
            case ECollectionPackType.DestinyBasicCardPack:
                monster = commonlist[UnityEngine.Random.Range(0, commonlist.Count)];
                break;
            case ECollectionPackType.RareCardPack:
                monster = rarelist[UnityEngine.Random.Range(0, rarelist.Count)];
                break;
            case ECollectionPackType.DestinyRareCardPack:
                monster = rarelist[UnityEngine.Random.Range(0, rarelist.Count)];
                break;
            case ECollectionPackType.EpicCardPack:
                monster = epiclist[UnityEngine.Random.Range(0, epiclist.Count)];
                break;
            case ECollectionPackType.DestinyEpicCardPack:
                monster = epiclist[UnityEngine.Random.Range(0, epiclist.Count)];
                break;
            case ECollectionPackType.LegendaryCardPack:
                monster = legendlist[UnityEngine.Random.Range(0, legendlist.Count)];
                break;
            case ECollectionPackType.DestinyLegendaryCardPack:
                monster = legendlist[UnityEngine.Random.Range(0, legendlist.Count)];
                break;
            default:
                monster = commonlist[UnityEngine.Random.Range(0, commonlist.Count)];
                break;
        }

        return new CardData
        {
            isFoil = randomGenerator.NextDouble() >= 0.5,
            isDestiny = randomGenerator.NextDouble() >= 0.5,
            borderType = border,
            monsterType = monster,
            expansionType = cardExpansionType,
            isChampionCard = false,
            isNew = true
        };
    }
}
