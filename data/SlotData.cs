using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace ApClient.data;

public class SlotData
{
    internal List<int> startingItems;

    public int CardSanity { get; set; }
    public bool TradesAreNew { get; set; }
    public int Goal { get; set; }
    public int ShopExpansionGoal { get; set; }
    public int MaxLevel { get; set; }
    public int GhostGoalAmount { get; set; }
    public bool FoilInSanity { get; set; }
    public int BorderInSanity { get; set; }
    public int SellCheckAmount { get; set; }
    public int ChecksPerPack { get; set; }
    public int CardCollectPercentage { get; set; } 
    public int GamesPerCheck { get; set; }
    public int NumberOfGameChecks { get; set; }
    public int NumberOfSellCardChecks { get; set; }
    public int SellCardsPerCheck { get; set; }
    public bool Deathlink { get; set; }
    public bool AutoRenovate { get; set; }
    public OrderedDictionary pg1IndexMapping { get; set; }
    public OrderedDictionary pg2IndexMapping { get; set; }
    public OrderedDictionary pg3IndexMapping { get; set; }
    public OrderedDictionary ttIndexMapping { get; set; }
    public int RequiredLicenses { get; set; }
    public int CollectionGoalPercent { get; internal set; }
    public int ExtraStartingItemChecks { get; internal set; }

}
