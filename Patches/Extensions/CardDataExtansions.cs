using System;
using System.Collections.Generic;
using System.Text;

namespace ApClient.data;

public static class CardDataExtensions
{
    public struct CardMask
    {
        public ulong rarity;
        public ulong border;
        public ulong expansion;
        public ulong foil;
        public ulong grade;
    }
    public static CardMask GetCardMask(this CardData card)
    {
        return new CardMask
        {
            rarity = 1UL << (int)card.monsterType,
            border = 1UL << (int)card.borderType,
            expansion = 1UL << (int)card.expansionType,
            foil = 1UL << (card.isFoil ? 1 : 0),
            grade = 1UL << card.cardGrade
        };
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