using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using UnityEngine.Rendering;

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
            //InventoryBase.GetMonsterData((EMonsterType)i).GetName();
            //Plugin.Log($"{$"{cardData.monsterType} {cardData.borderType} {(cardData.isFoil ? "Foil" :"NonFoil")} {cardData.expansionType}"} : {0x1F290000 | ((int)cardData.expansionType << 12) | ((int)cardData.borderType << 8) | ((cardData.isFoil ? 1 : 0) << 7) | (int)cardData.monsterType}");
            return 0x10000 | ((int)cardData.expansionType << 12) | ((int)cardData.borderType << 8) | ((cardData.isFoil ? 1 : 0) << 7) | (int)cardData.monsterType;
        }

        public static int oneghostcard = 326;
        public static int twoghostcard = 327;
        public static int threeghostcard = 328;
        public static int fourghostcard = 329;
        public static int fiveghostcard = 330;

        public static int CARD_SELL_START_ID = 500;
        public static int CARD_OPEN_START_ID = 1000;
        public static int getSellCheckId(ECardExpansionType expansionType, int check)
        {
            return CARD_SELL_START_ID + check + (int)expansionType * 50;
        }

        public static int getCheckId(ECollectionPackType packType, int check)
        {
            int expansionid = 0;
            int rarityid = 0;
            switch (packType)
            {
                case ECollectionPackType.BasicCardPack:
                    expansionid = 0;
                    rarityid = 1;
                    break;
                case ECollectionPackType.RareCardPack:
                    expansionid = 0;
                    rarityid = 2;
                    break;
                case ECollectionPackType.EpicCardPack:
                    expansionid = 0;
                    rarityid = 3;
                    break;
                case ECollectionPackType.LegendaryCardPack:
                    expansionid = 0;
                    rarityid = 4;
                    break;
                case ECollectionPackType.DestinyBasicCardPack:
                    expansionid = 1;
                    rarityid = 1;
                    break;
                case ECollectionPackType.DestinyRareCardPack:
                    expansionid = 1;
                    rarityid = 2;
                    break;
                case ECollectionPackType.DestinyEpicCardPack:
                    expansionid = 1;
                    rarityid = 3;
                    break;
                case ECollectionPackType.DestinyLegendaryCardPack:
                    expansionid = 1;
                    rarityid = 4;
                    break;

            }
            return CARD_OPEN_START_ID + check + (((rarityid - 1) + (expansionid * 4)) * 30);
        }
    }
}
