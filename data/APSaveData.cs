using System;
using System.Collections.Generic;
using System.Text;

namespace ApClient.data
{
    public class APSaveData
    {
        public int ProcessedIndex { get; set; }
        public string seed { get; set; }
        public string slotname { get; set; }
        public float MoneyMultiplier { get; set; }
        public int Luck { get; set; }
        public int TetramonCommonChecksFound { get; set; }
        public int TetramonRareChecksFound { get; set; }
        public int TetramonEpicChecksFound { get; set; }
        public int TetramonLegendaryChecksFound { get; set; }
        public int DestinyCommonChecksFound { get; set; }
        public int DestinyRareChecksFound { get; set; }
        public int DestinyEpicChecksFound { get; set; }
        public int DestinyLegendaryChecksFound { get; set; }
        public int TetramonCommonChecksSold { get; set; }
        public int TetramonRareChecksSold { get; set; }
        public int TetramonEpicChecksSold { get; set; }
        public int TetramonLegendaryChecksSold { get; set; }
        public int DestinyCommonChecksSold { get; set; }
        public int DestinyRareChecksSold { get; set; }
        public int DestinyEpicChecksSold { get; set; }
        public int DestinyLegendaryChecksSold { get; set; }
        public int GhostCardsSold { get; set; }
        public List<EGameEventFormat> UnlockedGameEvents { get; set; }
        public int EventGamesPlayed { get; set; }
        public List<int> newCards { get; set; }
        public Dictionary<EItemType, int> itemLevel { get; set; }
    }
}
