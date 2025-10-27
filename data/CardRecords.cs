using System;
using System.Collections.Generic;
using System.Text;

namespace ApClient.data;

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.UIElements;

[Serializable]
public class CardRecord
{
    [JsonProperty] public int Opened;
    [JsonProperty] public int Sold;
    [JsonProperty] public HashSet<int> Grades = new(); // 0–9
}

[Serializable]
public class PlayerCardProgress
{
    [JsonProperty] public Dictionary<int, CardRecord> Cards { get; set; } = new();
    [JsonProperty] public HashSet<string> CompletedAchievements { get; set; } = new();
}
public class CardAddedDTO
{
    public CardRecord cardRecord { get; set; }
    public long[] newAchievements { get; set; }
}