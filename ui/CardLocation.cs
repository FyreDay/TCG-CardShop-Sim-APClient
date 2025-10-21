using System;
using System.Collections.Generic;
using System.Text;

namespace ApClient.ui;

public enum CardStatus
{
    Available,
    Unavailable,
    Found
}

public class CardLocation
{
    public string Name { get; set; }
    public bool IsHinted { get; set; }
    public int GoalNum { get; set; }
    public int CurrentNum { get; set; }
    public CardStatus Status { get; set; }
}