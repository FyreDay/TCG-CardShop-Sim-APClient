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
    public static CardData CardRoller(ECollectionPackType collectionPackType)
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
