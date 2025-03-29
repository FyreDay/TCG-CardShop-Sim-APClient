using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApClient.mapping
{
    public class EmployeeMapping
    {
        public static Dictionary<int, (int itemid, string name, int locid)> mapping = new Dictionary<int, (int itemid, string name, int locid)>
        {
            {0, (0x1F2800B6, "Zachery", 0x1F280187) },
            {1, (0x1F2800B7, "Terence", 0x1F280188) },
            {2, (0x1F2800B8, "Dennis", 0x1F280189) },
            {3, (0x1F2800B9, "Clark", 0x1F28018A) },
            {4, (0x1F2800BA, "Angus", 0x1F28018B) },
            {5, (0x1F2800BB, "Benji", 0x1F28018C) },
            {6, (0x1F2800BC, "Lauren", 0x1F28018D) },
            {7, (0x1F2800BD, "Axel", 0x1F28018E) }
        };

        public static KeyValuePair<int, (int itemid, string name, int locid)> getKeyValue(int itemid)
        {
            Plugin.Log($"id: {itemid}");
            var result = mapping.FirstOrDefault(pair => pair.Value.itemid == itemid);

            var defaultPair = new KeyValuePair<int, (int, string, int)>(-1, (-1, "Unknown", -1));

            if (result.Equals(default(KeyValuePair<int, (int, string, int)>)))
            {
                result = defaultPair;
            }
            return result;
        }
    }
}
