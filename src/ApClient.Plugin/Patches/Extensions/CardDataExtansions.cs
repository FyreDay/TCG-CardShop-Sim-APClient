using System;
using System.Collections.Generic;
using System.Text;

namespace ApClient.data;

public static class CardDataExtensions
{
    public struct CardMask
    {
        public ulong border;
        public ulong packType;
        public ulong foil;
        public ulong grade;
    }
    public static CardMask GetCardMask(this CardData card)
    {
        return new CardMask
        {
            border = 1UL << (int)card.borderType,
            packType = 1UL << (int)card.GetPackType(),
            foil = 1UL << (card.isFoil ? 1 : 0),
            grade = 1UL << card.cardGrade
        };
    }

    public static ECollectionPackType GetPackType(this CardData card)
    {
        return (ECollectionPackType)((int)InventoryBase.GetMonsterData(card.monsterType).Rarity + (4 * (int)card.expansionType));
    }

    //public static int GetUniqueHash(this CardData card)
    //{
    //    return HashCode.Combine(
    //        card.monsterType,
    //        card.borderType,
    //        card.isFoil,
    //        card.isDestiny,
    //        card.isChampionCard
    //    );
    //}

   
}