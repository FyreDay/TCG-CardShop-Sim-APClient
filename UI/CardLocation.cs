using ApClient.data;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApClient.ui;

[Serializable]
public enum CardStatus
{
    Available,
    Unavailable,
    Found
}

[Serializable]
public class CardLocation
{
    public bool IsHinted { get; set; }
    public int CurrentNum { get; set; }
    public CardStatus Status { get; set; }
    public AchievementData AchievementData { get; set; }
}