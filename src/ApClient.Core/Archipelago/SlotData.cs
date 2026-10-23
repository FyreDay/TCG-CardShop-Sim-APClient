using System.Collections.Generic;


namespace ApClient.Core.Archipelago;

public class SlotData
{
    public string ModVersion { get; private set; }
    public int MaxLevel { get; private set; }
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
    public bool NoFormat { get; internal set; }

    public int BulkBoxChecks { get; internal set; }
    public int SeparatePacksAndBoxes { get; internal set; }
    public int StartingEmployeeIndex { get; internal set; }

    public SlotData(Dictionary<string, object> slotDict)
    {
        ModVersion = slotDict.GetValueOrDefault("ModVersion").ToString();
        MaxLevel = int.Parse(slotDict.GetValueOrDefault("MaxLevel").ToString());
        Goal = int.Parse(slotDict.GetValueOrDefault("Goal").ToString());
        CollectionGoalPercent = int.Parse(slotDict.GetValueOrDefault("CollectionGoalPercent").ToString());
        GhostGoalAmount = int.Parse(slotDict.GetValueOrDefault("GhostGoalAmount").ToString());

        StartingEmployeeIndex = int.Parse(slotDict.GetValueOrDefault("StartingEmployeeIndex").ToString());

        AutoRenovate = slotDict.GetValueOrDefault("AutoRenovate").ToString() == "1";
        SellCheckAmount = int.Parse(slotDict.GetValueOrDefault("SellCheckAmount").ToString());
        BulkBoxChecks = int.Parse(slotDict.GetValueOrDefault("BulkBoxChecks")?.ToString());
        SeparatePacksAndBoxes = int.Parse(slotDict.GetValueOrDefault("SeparatePacksAndBoxes")?.ToString());
        PlayTableChecks = int.Parse(slotDict.GetValueOrDefault("PlayTableChecks").ToString());
        NoFormat = slotDict.GetValueOrDefault("NoFormat").ToString() == "1";

        CardOpeningCheckDifficulty = int.Parse(slotDict.GetValueOrDefault("CardOpeningCheckDifficulty").ToString());
        CardSellingCheckDifficulty = int.Parse(slotDict.GetValueOrDefault("CardSellingCheckDifficulty").ToString());
        CardGradingCheckDifficulty = int.Parse(slotDict.GetValueOrDefault("CardGradingCheckDifficulty").ToString());
        
        CardSanity = int.Parse(slotDict.GetValueOrDefault("CardSanity").ToString());

        Deathlink = slotDict.GetValueOrDefault("Deathlink").ToString() == "1";
    }
}
