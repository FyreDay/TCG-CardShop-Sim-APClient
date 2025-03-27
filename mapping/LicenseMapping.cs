using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApClient.mapping
{
    public class LicenseMapping
    {
        public static Dictionary<int, (int itemid, string name, int count, int locid)> mapping = new Dictionary<int, (int itemid, string name, int count, int locid)>
        {
            {1, (0x1F280001, "Progressive Basic Card Pack", 1, 0x1F2800F1) },
            {2, (0x1F280001, "Progressive Basic Card Pack", 2,0x1F2800F2) },
            {3, (0x1F280001, "Progressive Basic Card Pack", 3,0x1F2800F3) },
            {4, (0x1F280002, "Progressive Rare Card Pack",1,0x1F2800F4) },
            {5, (0x1F280002, "Progressive Rare Card Pack",2,0x1F2800F5) },
            {6, (0x1F280002, "Progressive Rare Card Pack",3,0x1F2800F6) },
            {7, (0x1F280002, "Progressive Rare Card Pack",4,0x1F2800F7) },
            {8, (0x1F280002, "Progressive Epic Card Pack",1,0x1F2800F8) },
            {9, (0x1F280002, "Progressive Epic Card Pack",2,0x1F2800F9) },
            {10, (0x1F280002, "Progressive Epic Card Pack",3,0x1F2800FA) },
            {11, (0x1F280002, "Progressive Epic Card Pack",4,0x1F2800FB) },
            {40, (0x1F280011, "Progressive Cleanser",1,0x1F280118) },
            {41, (0x1F280011, "Progressive Cleanser",2,0x1F280119) },
        };

        public static KeyValuePair<int, (int itemid, string name, int count, int locid)> getPair(int id)
        {
            Plugin.Log($"id: {id}");
            return mapping.FirstOrDefault(pair => pair.Value.itemid == id);
        }
    }
}
