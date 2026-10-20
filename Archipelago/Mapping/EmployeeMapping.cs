using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApClient.mapping
{
    public class EmployeeMapping
    {

        private const int ITEMS_WORKER_OFFSET = 8000;
        public static Dictionary<int, (int itemid, string name)> mapping = new Dictionary<int, (int itemid, string name)>
        {
            {ITEMS_WORKER_OFFSET + 1, (219, "Zachery") },
            {ITEMS_WORKER_OFFSET + 2, (220, "Terence") },
            {ITEMS_WORKER_OFFSET + 3, (221, "Dennis") },
            {ITEMS_WORKER_OFFSET + 4, (222, "Clark") },
            {ITEMS_WORKER_OFFSET + 5, (223, "Angus") },
            {ITEMS_WORKER_OFFSET + 6, (224, "Benji") },
            {ITEMS_WORKER_OFFSET + 7, (225, "Lauren") },
            {ITEMS_WORKER_OFFSET + 8, (226, "Axel") },
            {ITEMS_WORKER_OFFSET + 9, (227, "Alexander") }
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
