using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApClient.mapping
{
    public class EmployeeMapping
    {
        public static Dictionary<int, (int itemid, string name)> mapping = new Dictionary<int, (int itemid, string name)>
        {
            {0, (219, "Zachery") },
            {1, (220, "Terence") },
            {2, (221, "Dennis") },
            {3, (222, "Clark") },
            {4, (223, "Angus") },
            {5, (224, "Benji") },
            {6, (225, "Lauren") },
            {7, (226, "Axel") },
            {8, (231, "Alexander") }
        };

        public static int getindexFromId(int itemid)
        {
            foreach (var kvp in mapping)
            {
                if (kvp.Value.itemid == itemid)
                    return kvp.Key;
            }
            return -1;
        }
    }
}
