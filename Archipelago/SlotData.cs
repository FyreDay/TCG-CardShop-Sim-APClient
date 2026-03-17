using ApClient.Archipelago;
using ApClient.ui;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace ApClient.data;

public class SlotData
{
    internal List<int> startingItems;
    public OrderedDictionary pg1IndexMapping { get; private set; }
    public OrderedDictionary pg2IndexMapping { get; private set; }
    public OrderedDictionary pg3IndexMapping { get; private set; }
    public OrderedDictionary ttIndexMapping { get; private set; }
    public List<AchievementData> OpenAchievementData { get; private set; }
    public List<AchievementData> SellAchievementData { get; private set; }
    public List<AchievementData> GradeAchievementData { get; private set; }

    public int MaxLevel { get; private set; }
    public int RequiredLicenses { get; private set; }
    public int Goal { get; private set; }
    public int GhostGoalAmount { get; private set; }
    public int CollectionGoalPercent { get; private set; }
    public bool AutoRenovate { get; private set; }
    public int ExtraStartingItemChecks { get; private set; }
    public int SellCheckAmount { get; private set; }
    public int CardOpeningCheckDifficulty { get; private set; }
    public int CardSellingCheckDifficulty { get; private set; } 
    public int CardGradingCheckDifficulty { get; private set; }
    public int PlayTableChecks { get; private set; }
    public int CardSanity { get; private set; }
    public bool Deathlink { get; private set; }
    
    
    public SlotData(Dictionary<string, object> slotDict)
    {
        MaxLevel = int.Parse(slotDict.GetValueOrDefault("MaxLevel").ToString());
        RequiredLicenses = int.Parse(slotDict.GetValueOrDefault("RequiredLicenses").ToString());
        Goal = int.Parse(slotDict.GetValueOrDefault("Goal").ToString());
        CollectionGoalPercent = int.Parse(slotDict.GetValueOrDefault("CollectionGoalPercent").ToString());
        GhostGoalAmount = int.Parse(slotDict.GetValueOrDefault("GhostGoalAmount").ToString());

        OpenAchievementData = JsonConvert.DeserializeObject<List<AchievementData>>(slotDict.GetValueOrDefault("OpenAchievements").ToString());
        SellAchievementData = JsonConvert.DeserializeObject<List<AchievementData>>(slotDict.GetValueOrDefault("SellAchievements").ToString());
        GradeAchievementData = JsonConvert.DeserializeObject<List<AchievementData>>(slotDict.GetValueOrDefault("GradeAchievements").ToString());

        AutoRenovate = slotDict.GetValueOrDefault("AutoRenovate").ToString() == "1";
        ExtraStartingItemChecks = int.Parse(slotDict.GetValueOrDefault("ExtraStartingItemChecks").ToString());
        SellCheckAmount = int.Parse(slotDict.GetValueOrDefault("SellCheckAmount").ToString());
        CardOpeningCheckDifficulty = int.Parse(slotDict.GetValueOrDefault("CardOpeningCheckDifficulty").ToString());
        CardSellingCheckDifficulty = int.Parse(slotDict.GetValueOrDefault("CardSellingCheckDifficulty").ToString());
        CardGradingCheckDifficulty = int.Parse(slotDict.GetValueOrDefault("CardGradingCheckDifficulty").ToString());
        PlayTableChecks = int.Parse(slotDict.GetValueOrDefault("PlayTableChecks").ToString());

        Deathlink = slotDict.GetValueOrDefault("Deathlink").ToString() == "1";
        CardSanity = int.Parse(slotDict.GetValueOrDefault("CardSanity").ToString());

        pg1IndexMapping = PgStrToDict(slotDict.GetValueOrDefault("ShopPg1Mapping").ToString());
        pg2IndexMapping = PgStrToDict(slotDict.GetValueOrDefault("ShopPg2Mapping").ToString());
        pg3IndexMapping = PgStrToDict(slotDict.GetValueOrDefault("ShopPg3Mapping").ToString());
        ttIndexMapping = PgStrToDict(slotDict.GetValueOrDefault("ShopTTMapping").ToString());
        startingItems = StrToList(slotDict.GetValueOrDefault("StartingIds").ToString());
    }

    public Dictionary<string, List<AchievementData>> GetAchievementDefinitions()
    {
        var defs = new Dictionary<string, List<AchievementData>>
        {
            { Constants.OPEN_ACHIEVEMENT_TYPE, OpenAchievementData },
            { Constants.SELL_ACHIEVEMENT_TYPE, SellAchievementData },
            { Constants.GRADE_ACHIEVEMENT_TYPE, GradeAchievementData }
        };

        return defs;
    }

    private OrderedDictionary PgStrToDict(string str)
    {
        var jObj = JObject.Parse(str);
        var ordered = new OrderedDictionary();
        foreach (var prop in jObj.Properties())
        {
            try
            {
                int name = int.Parse(prop.Name);
                EItemType key = (EItemType)(name == 190 ? 0 : name);
                int value = (int)prop.Value;
                ordered.Add(key, value);
                //Plugin.Log($"{key} : {value}");
            }
            catch
            {
                Plugin.Logger.LogInfo($" FAILED {prop.Name} : {prop.Value}");
            }
        }

        return ordered;
    }

    private List<int> StrToList(string str)
    {
        return str.Trim('[', ']')                 // Remove square brackets
               .Split(',')                      // Split by commas
               .Select(s => s.Trim())           // Trim whitespace and special characters
               .Where(s => int.TryParse(s, out _)) // Ensure valid integers
               .Select(int.Parse)               // Convert to integers
               .ToList();
    }


}
