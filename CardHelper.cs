using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace ApClient;

public class CardHelper
{
    private ERarity[] CardRarities = new ERarity[(int)EMonsterType.MAX];
    private bool initialized = false;

    public CardHelper() 
    {
        initialized = false;
    }

    public void Initialize()
    {
        CardRarities[0] = ERarity.None;
        for (int i = 1; i < CardRarities.Length; i++)
        {
            try
            {
                CardRarities[i] = InventoryBase.GetMonsterData((EMonsterType)i).Rarity;
            }
            catch (Exception e)
            {
                Plugin.Log("Failed to get monster data");
            }
            
        }
        initialized = true;
        foreach (ERarity value in CardRarities)
        {
            Console.WriteLine(value);
        }
        Plugin.Log("initialized card helper");
    }

    public CardData RandomNewCard(ECardExpansionType expansion, HashSet<ERarity> allowedRarities, int borderlimit,bool foils)
    {
        Plugin.Log($"RandomNewCard {expansion.ToString()} {allowedRarities.Count} {borderlimit} {foils}");
        foreach (ERarity value in allowedRarities)
        {
            Console.WriteLine($"allowed: {value}");
        }
        try
        {
            if (!initialized)
            {
                Initialize();
            }

            List<bool> cardCollectedList = CPlayerData.GetIsCardCollectedList(expansion, false);
            int versionsPerCard = foils ? 12 : 6;
            int cardCount = (int)EMonsterType.MAX -1;

            var candidateIndices = new List<int>();

            for (int cardNameIndex = 1; cardNameIndex < cardCount; cardNameIndex++)
            {
                if (!allowedRarities.Contains(CardRarities[cardNameIndex]))
                {
                    continue;
                }

                int baseIndex = cardNameIndex * 12;
                for (int i = 0; i < versionsPerCard; i++)
                {
                    if(i % 6 > borderlimit)
                    {
                        continue;
                    }
                    int fullIndex = baseIndex + i;
                    if (!cardCollectedList[fullIndex])
                    {
                        candidateIndices.Add(fullIndex);
                    }
                }
            }
            if (candidateIndices.Count == 0)
            {
                Plugin.Log("No new cards");
                return CardRoller((ECollectionPackType)UnityEngine.Random.Range(0, 4));
            }

            int cardIndex = candidateIndices[UnityEngine.Random.Range(0, candidateIndices.Count)];
            int versionIndex = cardIndex % 12;
            int monsterTypeIndex = (cardIndex - versionIndex) / 12;
            CardData card = new CardData
            {
                isFoil = versionIndex > 5,
                isDestiny = expansion == ECardExpansionType.Destiny,
                borderType = (ECardBorderType)(versionIndex % 6),
                monsterType = (EMonsterType)monsterTypeIndex,
                expansionType = expansion,
                isChampionCard = false,
                isNew = true
            };
            Plugin.Log($"{card.monsterType} {card.borderType} {card.expansionType} {card.isFoil}");
            return card;
        }
        catch (Exception ex)
        {
            Plugin.Log(ex.Message);
            throw;
        }
    }
    public CardData CardRoller(ECollectionPackType collectionPackType)
    {
        ECardExpansionType expansionType = UnityEngine.Random.Range(0F, 1F) > 0.5 ? ECardExpansionType.Tetramon : ECardExpansionType.Destiny;
        return new CardData
        {
            isFoil = UnityEngine.Random.Range(0F, 1F) > 0.5,
            isDestiny = expansionType == ECardExpansionType.Destiny,
            borderType = (ECardBorderType)UnityEngine.Random.Range(0, 6),
            monsterType = (EMonsterType)UnityEngine.Random.Range(0, (int)EMonsterType.MAX),
            expansionType = expansionType,
            isChampionCard = false,
            isNew = true
        };
    }
}
