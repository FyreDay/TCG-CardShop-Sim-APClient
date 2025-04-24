using System;
using System.Collections.Generic;
using System.Text;

namespace ApClient.mapping
{
    public class CardMapping
    {
        public static int getId(CardData cardData)
        {
            if((int)cardData.expansionType > 1 || (int)cardData.monsterType >= (int)EMonsterType.MAX)
            {
                return -1;
            }
            //Plugin.Log($"{$"{cardData.monsterType} {cardData.borderType} {(cardData.isFoil ? "Foil" :"NonFoil")} {cardData.expansionType}"} : {0x1F290000 | ((int)cardData.expansionType << 12) | ((int)cardData.borderType << 8) | ((cardData.isFoil ? 1 : 0) << 7) | (int)cardData.monsterType}");
            return 0x1F290000 | ((int)cardData.expansionType << 12) | ((int)cardData.borderType << 8) | ((cardData.isFoil ? 1 : 0) << 7) | (int)cardData.monsterType;
        }

        public static int ghostProgressive = 0x1F2800D7;
    }
}
