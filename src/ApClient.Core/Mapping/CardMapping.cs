using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using UnityEngine.Rendering;

namespace ApClient.mapping
{
    public class CardMapping
    {
        private const long CardMarker = 1L << 51;
        public static long getId(CardData cardData)
        {
            if ((int)cardData.expansionType > 1 || (int)cardData.monsterType >= (int)EMonsterType.MAX)
            {
                return -1;
            }
            //InventoryBase.GetMonsterData((EMonsterType)i).GetName();
            //Plugin.Logger.LogInfo($"{$"{cardData.monsterType} {cardData.borderType} {(cardData.isFoil ? "Foil" : "NonFoil")} {cardData.expansionType}"} : {0x1F290000 | ((int)cardData.expansionType << 12) | ((int)cardData.borderType << 8) | ((cardData.isFoil ? 1 : 0) << 7) | (int)cardData.monsterType}");
            return CardMarker
                | ((long)cardData.expansionType << 12)
                | ((long)cardData.borderType << 8)
                | ((cardData.isFoil ? 1L : 0L) << 7)
                | (long)cardData.monsterType;
        }

        public static CardData getCardFromId(long id)
        {
            if ((id & (1L << 51)) == 0)
                return null;

            var cardData = new CardData();
            cardData.expansionType = (ECardExpansionType)((id >> 12) & 0xF);
            cardData.borderType = (ECardBorderType)((id >> 8) & 0xF);
            cardData.isFoil = ((id >> 7) & 0x1) == 1;
            cardData.monsterType = (EMonsterType)(id & 0x7F);
            cardData.cardGrade = 0;
            cardData.isDestiny = cardData.expansionType == ECardExpansionType.Destiny;
            return cardData;
        }
    }
}
