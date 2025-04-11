using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApClient.mapping;

public class FurnatureMapping
{
    public static int[] reorder = [0, 5, 17,  4, 11,18,  1,6,12,  2, 10, 13,  8,14,20,  3,12,19, 7,15,21, 9];
    public static Dictionary<int, (int itemid, string name, int count)> mapping = new Dictionary<int, (int itemid, string name, int count)>
    {
        {0, (-1, "Small Cabinet", 1) },
        {1, (0x1F280045, "Small Metal Rack", 1) },
        {2, (0x1F28004E, "Play Table", 1) },
        {3, (0x1F28004B, "Small Personal Shelf", 1) },
        {4, (0x1F280046, "Single Sided Shelf", 1) },
        {5, (0x1F280049, "Card Table",1) },
        {6, (0x1F28004D, "Small Warehouse Shelf",1) },
        {7, (0x1F28004A, "Small Card Display",1) },
        {8, (0x1F28004C, "Auto Scent M100",1) },
        {9, (0x1F28004F, "Workbench",1) },
        {10, (0x1F280050, "Trash Bin",1) },
        {11, (0x1F280047, "Double Sided Shelf",1) },
        {12, (0x1F28004D, "Big Warehouse Shelf",2) },
        {13, (0x1F280051, "Checkout Counter",1) },
        {14, (0x1F28004C, "Auto Scent G500",2) },
        {15, (0x1F28004A, "Card Display Table",2) },
        {16, (0x1F28004B, "Big Personal Shelf",2) },
        {17, (0x1F280049, "Vintage Card Table",2) },
        {18, (0x1F280048, "Wide Shelf",1) },
        {19, (0x1F28004B, "Huge Personal Shelf",3) },
        {20, (0x1F28004C, "Auto Scent T100",3) },
        {21, (0x1F28004A, "Big Card Display",3) },
       
    };

    public static KeyValuePair<int, (int itemid, string name, int count)> getKeyValue(int itemid, int count = 1)
    {
        //Plugin.Log($"id: {itemid}");
        var result = mapping.FirstOrDefault(pair => pair.Value.itemid == itemid && count == pair.Value.count);

        var defaultPair = new KeyValuePair<int, (int, string, int)>(-1, (-1, "Unknown", 0));

        if (result.Equals(default(KeyValuePair<int, (int, string, int)>)))
        {
            result = defaultPair;
        }
        return result;
    }

    public static (int itemid, string name, int count) getValueOrEmpty(int key)
    {

        return mapping.GetValueOrDefault<int, (int itemid, string name, int count)>(key, (-1, "", 0));
    }

    
}
