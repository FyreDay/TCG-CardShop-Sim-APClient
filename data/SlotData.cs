using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApClient.data;

public class SlotData
{
    public int CardSanity { get; set; }
    public bool TradesAreNew { get; set; }
    public int Goal { get; set; }
    public int ShopExpansionGoal { get; set; }
    public int LevelGoal { get; set; }
    public int GhostGoalAmount { get; set; }
    public bool FoilInSanity { get; set; }
    public int BorderInSanity { get; set; }
    public int SellCheckAmount { get; set; }
    public bool Deathlink { get; set; }
    public List<int> pg1IndexMapping { get; set; }
    public List<int> pg2IndexMapping { get; set; }
    public List<int> pg3IndexMapping { get; set; }
    public List<int> ttIndexMapping { get; set; }
}
