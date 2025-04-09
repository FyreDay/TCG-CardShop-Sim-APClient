using System;
using System.Collections.Generic;
using System.Text;

namespace ApClient;

public class SlotData
{
    public int CardSanity { get; set; }
    public bool TradesAreNew { get; set; }
    public int Goal { get; set; }
    public int ShopExpansionGoal { get; set; }
    public int LevelGoal { get; set; }
    public int GhostGoalAmount { get; set; }

    public List<int> pg1IndexMapping { get; set; }
    public List<int> pg2IndexMapping { get; set; }
    public List<int> pg3IndexMapping { get; set; }
    public List<int> ttIndexMapping { get; set; }
}
