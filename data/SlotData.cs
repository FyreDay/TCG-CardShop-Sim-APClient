using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace ApClient.data;

public class SlotData
{
    internal List<int> startingItems;
    public OrderedDictionary pg1IndexMapping { get; set; }
    public OrderedDictionary pg2IndexMapping { get; set; }
    public OrderedDictionary pg3IndexMapping { get; set; }
    public OrderedDictionary ttIndexMapping { get; set; }
    public List<AchievementData> OpenAchievementData { get; set; }
    public List<AchievementData> SellAchievementData { get; set; }
    public List<AchievementData> GradeAchievementData { get; set; }

    public int MaxLevel { get; set; }
    public int RequiredLicenses { get; set; }
    public int Goal { get; set; }
    public int GhostGoalAmount { get; set; }
    public int CollectionGoalPercent { get; internal set; }
    public bool AutoRenovate { get; set; }
    public int ExtraStartingItemChecks { get; internal set; }
    public int SellCheckAmount { get; set; }
    public int CardOpeningCheckDifficulty { get; set; }
    public int CardSellingCheckDifficulty { get; set; } 
    public int CardGradingCheckDifficulty { get; set; }
    public int PlayTableChecks { get; set; }
    public int CardSanity { get; set; }
    public bool Deathlink { get; set; }
    
    
    
    

}
