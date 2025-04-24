using System;
using System.Collections.Generic;
using System.Text;

namespace ApClient.data
{
    public class APSaveData
    {
        public int ProcessedIndex { get; set; }
        public string seed { get; set; }
        public float MoneyMultiplier { get; set; }
        public int Luck { get; set; }
        public List<int> newCards {  get; set; }
        public int TetramonChecksFound { get; set; }
        public int DestinyChecksFound { get; set; }
        public int GhostChecksFound { get; set; }
    }
}
