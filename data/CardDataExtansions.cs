using System;
using System.Collections.Generic;
using System.Text;

namespace ApClient.data;

public static class CardDataExtensions
{
    public static int GetUniqueHash(this CardData card)
    {
        // This creates a consistent hash key that uniquely identifies each
        // logical card variation you care about.
        return HashCode.Combine(
            card.monsterType,
            card.borderType,
            card.isFoil,
            card.isDestiny,
            card.isChampionCard
        );
    }
}