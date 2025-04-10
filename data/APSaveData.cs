using System;
using System.Collections.Generic;
using System.Text;

namespace ApClient.data
{
    public class APSaveData
    {
        public int ProcessedIndex { get; set; }
        public string seed { get; set; }

        public List<int> newCards {  get; set; }
    }
}
