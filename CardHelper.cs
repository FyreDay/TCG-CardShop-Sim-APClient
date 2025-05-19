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
    private ERarity[] CardRarities = new ERarity[121];
    private bool initialized = false;

    public CardHelper() 
    {
        initialized = false;
    }

    public void Initialize()
    {
        for (int i = 1; i <= CardRarities.Length; i++)
        {
            CardRarities[i-1] = InventoryBase.GetMonsterData((EMonsterType)i).Rarity;
        }
        initialized = true;
        Plugin.Log("initialized card helper");
    }

    public CardData RandomNewCard(ECardExpansionType expansion, HashSet<ERarity> allowedRarities, int borderlimit,bool foils)
    {
        Plugin.Log("RandomNewCard Start");
        try
        {
            if (!initialized)
            {
                Initialize();
            }

            List<bool> cardCollectedList = CPlayerData.GetIsCardCollectedList(expansion, false);
            int versionsPerCard = foils ? 12 : 6;
            int cardCount = 121;

            var candidateIndices = new List<int>();

            for (int cardNameIndex = 0; cardNameIndex < cardCount; cardNameIndex++)
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
                return CardRoller((ECollectionPackType)UnityEngine.Random.Range(0, 4));
            }

            int cardIndex = candidateIndices[UnityEngine.Random.Range(0, candidateIndices.Count)];
            int monsterTypeIndex = cardIndex / 12;
            int versionIndex = cardIndex % 12;
            return new CardData
            {
                isFoil = versionIndex > 5,
                isDestiny = expansion == ECardExpansionType.Destiny,
                borderType = (ECardBorderType)(versionIndex % 6),
                monsterType = (EMonsterType)monsterTypeIndex,
                expansionType = expansion,
                isChampionCard = false,
                isNew = true
            };
        }
        catch (Exception ex)
        {
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
